using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Crpg.Application.Games.Models;
using Crpg.Domain.Entities;
using Crpg.Domain.Entities.Servers;
using Crpg.Sdk.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using LoggerFactory = Crpg.Logging.LoggerFactory;

namespace Crpg.Application.Common.Services;

public interface IGameServerStatsService
{
    Task<GameServerStats?> GetGameServerStatsAsync(CancellationToken cancellationToken);
}

public class DatadogGameServerStatsService : IGameServerStatsService
{
    private static readonly ILogger Logger = LoggerFactory.CreateLogger<DatadogGameServerStatsService>();
    private readonly HttpClient? _ddHttpClient;

    private readonly Dictionary<GameModeAlias, GameMode> gameModeByInstanceAlias = new()
    {
        { GameModeAlias.A, GameMode.CRPGBattle },
        { GameModeAlias.B, GameMode.CRPGConquest },
        { GameModeAlias.C, GameMode.CRPGDuel },
        { GameModeAlias.E, GameMode.CRPGDTV },
        { GameModeAlias.D, GameMode.CRPGSkirmish },
    };

    private readonly IDateTime _dateTime;
    private DateTime _lastUpdate = DateTime.MinValue;
    private GameServerStats? _serverStats;
    public DatadogGameServerStatsService(IConfiguration configuration, HttpClient httpClient, IDateTime dateTime)
    {
        _dateTime = dateTime;
        string? ddApiKey = configuration["Datadog:ApiKey"];
        string? ddApplicationKey = configuration["Datadog:ApplicationKey"];
        if (ddApiKey != null && ddApplicationKey != null)
        {
            _ddHttpClient = httpClient;
            _ddHttpClient.BaseAddress = new Uri("https://api.datadoghq.com/");
            _ddHttpClient.DefaultRequestHeaders.Add("DD-API-KEY", ddApiKey);
            _ddHttpClient.DefaultRequestHeaders.Add("DD-APPLICATION-KEY", ddApplicationKey);
        }
    }

    public async Task<GameServerStats?> GetGameServerStatsAsync(CancellationToken cancellationToken)
    {
        if (_ddHttpClient == null)
        {
            return null;
        }

        if (_dateTime.UtcNow < _lastUpdate + TimeSpan.FromMinutes(1))
        {
            return _serverStats;
        }

        var to = _dateTime.UtcNow.Subtract(DateTime.UnixEpoch);
        var from = to - TimeSpan.FromMinutes(15); // Adjust to a 15-minute window
        FormUrlEncodedContent query = new(new[]
        {
            KeyValuePair.Create("from", from.TotalSeconds.ToString()),
            KeyValuePair.Create("to", to.TotalSeconds.ToString()),
            KeyValuePair.Create("query", "sum:crpg.users.playing.count{*} by {region, instance}"), // The query itself does not change
        });
        string queryStr = await query.ReadAsStringAsync(cancellationToken);

        GameServerStats? serverStats;
        try
        {
            serverStats = new()
            {
                Total = new GameStats { PlayingCount = 0 },
                Regions = new Dictionary<Region, Dictionary<GameMode, GameStats>>(),
            };

            var res = await _ddHttpClient.GetFromJsonAsync<DatadogQueryResponse>("api/v1/query?" + queryStr, cancellationToken);

            double latestTimestamp;

            foreach (var serie in res!.Series)
            {
                latestTimestamp = serie.PointList.Max(point => (int)point[0]!);
                string regionStr = serie.Scope.Split(',').Last().Split(':').Last();
                string instanceAliasStr = serie.Scope.Split(',').First().Split(':').Last();
                instanceAliasStr = instanceAliasStr[^1..];

                if (Enum.TryParse(regionStr, ignoreCase: true, out Region region))
                {
                    if (Enum.TryParse(instanceAliasStr, ignoreCase: true, out GameModeAlias instanceAlias))
                    {
                        var pointsInLast15Minutes = serie.PointList
                            .Where(point => point[1] != null && latestTimestamp - point[0] <= 600 * 1000)
                            .Select(point => (int)point[1]!);

                        int maxPlayingCount = pointsInLast15Minutes.Any() ? pointsInLast15Minutes.Max() : 0;

                        serverStats.Total.PlayingCount += maxPlayingCount;

                        if (!serverStats.Regions.ContainsKey(region))
                        {
                            serverStats.Regions[region] = new Dictionary<GameMode, GameStats>();
                        }

                        serverStats.Regions[region][gameModeByInstanceAlias[instanceAlias]] = new GameStats { PlayingCount = maxPlayingCount };
                    }
                }
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Could not get server stats");
            serverStats = null;
        }

        // Both fields can be updated by several threads but the results is the same.
        _lastUpdate = _dateTime.UtcNow;
        _serverStats = serverStats;
        return _serverStats;
    }

    private class DatadogQueryResponse
    {
        public string Status { get; set; } = string.Empty;
        [JsonPropertyName("res_type")]
        public string ResType { get; set; } = string.Empty;
        [JsonPropertyName("resp_version")]
        public int RespVersion { get; set; }
        public string Query { get; set; } = string.Empty;
        [JsonPropertyName("from_date")]
        public long FromDate { get; set; }
        [JsonPropertyName("to_date")]
        public long ToDate { get; set; }
        public DatadogSeries[] Series { get; set; } = Array.Empty<DatadogSeries>();
        public object[] Values { get; set; } = Array.Empty<object>();
        public object[] Times { get; set; } = Array.Empty<object>();
        public string Message { get; set; } = string.Empty;
        [JsonPropertyName("group_by")]
        public string[] GroupBy { get; set; } = Array.Empty<string>();
    }

    private class DatadogSeries
    {
        public object Unit { get; set; } = string.Empty;
        [JsonPropertyName("query_index")]
        public int QueryIndex { get; set; }
        public string Aggr { get; set; } = string.Empty;
        public string Metric { get; set; } = string.Empty;
        [JsonPropertyName("tag_set")]
        public string[] TagSet { get; set; } = Array.Empty<string>();
        public string Expression { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;
        public int Interval { get; set; }
        public int Length { get; set; }
        public long Start { get; set; }
        public long End { get; set; }
        public double?[][] PointList { get; set; } = Array.Empty<double?[]>();
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; } = string.Empty;
        public Dictionary<string, object> Attributes { get; set; } = new();
    }
}
