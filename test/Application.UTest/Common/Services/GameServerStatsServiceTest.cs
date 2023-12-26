using Crpg.Application.Common.Services;
using Crpg.Domain.Entities;
using Crpg.Domain.Entities.Servers;
using Crpg.Sdk.Abstractions;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace Crpg.Application.UTest.Common.Services;

public class GameServerStatsServiceTest : TestBase
{
    [Test]
    public async Task Basic()
    {
        Mock<IConfiguration> configurationMock = new();
        configurationMock.SetupGet(x => x[It.Is<string>(s => s == "Datadog:ApiKey")]).Returns("111");
        configurationMock.SetupGet(x => x[It.Is<string>(s => s == "Datadog:ApplicationKey")]).Returns("222");

        Mock<IDateTime> dateTimeMock = new();
        dateTimeMock.Setup(dt => dt.UtcNow).Returns(new DateTime(2023, 12, 23, 0, 20, 0));
        Mock<IGameModeService> gameModeServiceServiceMock = new();

        var mockHttp = new MockHttpMessageHandler();
        var request = mockHttp.When("https://api.datadoghq.com/api/v1/query*")
                .WithHeaders(new Dictionary<string, string>
                {
                    { "DD-API-KEY", "111" },
                    { "DD-APPLICATION-KEY", "222" },
                })
                .WithQueryString(new Dictionary<string, string>
                {
                    { "from", "1703289900" },
                    { "to", "1703290800" }, // https://www.epochconverter.com/
                    { "query", "sum:crpg.users.playing.count{*} by {region, instance}" },
                })
                .Respond("application/json", "{\"status\":\"ok\",\"res_type\":\"time_series\",\"resp_version\":1,\"query\":\"sum:crpg.users.playing.count{*} by {region,instance}\",\"from_date\":1703277766000,\"to_date\":1703278666000,\"series\":[{\"unit\":[{\"family\":\"general\",\"id\":117,\"name\":\"user\",\"short_name\":null,\"plural\":\"users\",\"scale_factor\":1},null],\"query_index\":0,\"aggr\":\"sum\",\"metric\":\"crpg.users.playing.count\",\"tag_set\":[\"instance:crpg01a\",\"region:eu\"],\"expression\":\"sum:crpg.users.playing.count{instance:crpg01a,region:eu}\",\"scope\":\"instance:crpg01a,region:eu\",\"interval\":5,\"length\":62,\"start\":1703277770000,\"end\":1703278634000,\"pointlist\":[[1703277770000,5.5],[1703277790000,4.5],[1703277810000,3.5],[1703278610000,null],[1703278620000,null],[1703278630000,null]],\"display_name\":\"crpg.users.playing.count\",\"attributes\":{}},{\"unit\":[{\"family\":\"general\",\"id\":117,\"name\":\"user\",\"short_name\":null,\"plural\":\"users\",\"scale_factor\":1},null],\"query_index\":0,\"aggr\":\"sum\",\"metric\":\"crpg.users.playing.count\",\"tag_set\":[\"instance:crpg01e\",\"region:eu\"],\"expression\":\"sum:crpg.users.playing.count{instance:crpg01e,region:eu}\",\"scope\":\"instance:crpg01e,region:eu\",\"interval\":5,\"length\":62,\"start\":1703277770000,\"end\":1703278634000,\"pointlist\":[[1703277770000,53.333333333333336],[1703277790000,54],[1703277810000,54.666666666666664],[1703277820000,55],[1703277830000,55.166666666666664],[1703277850000,55.5]],\"display_name\":\"crpg.users.playing.count\",\"attributes\":{}},{\"unit\":[{\"family\":\"general\",\"id\":117,\"name\":\"user\",\"short_name\":null,\"plural\":\"users\",\"scale_factor\":1},null],\"query_index\":0,\"aggr\":\"sum\",\"metric\":\"crpg.users.playing.count\",\"tag_set\":[\"instance:crpg03a\",\"region:as\"],\"expression\":\"sum:crpg.users.playing.count{instance:crpg03a,region:as}\",\"scope\":\"instance:crpg03a,region:as\",\"interval\":5,\"length\":62,\"start\":1703277770000,\"end\":1703278634000,\"pointlist\":[[1703277770000,0],[1703277790000,0],[1703277810000,0],[1703277820000,0],[1703277830000,0],[1703277850000,0]],\"display_name\":\"crpg.users.playing.count\",\"attributes\":{}},{\"unit\":[{\"family\":\"general\",\"id\":117,\"name\":\"user\",\"short_name\":null,\"plural\":\"users\",\"scale_factor\":1},null],\"query_index\":0,\"aggr\":\"sum\",\"metric\":\"crpg.users.playing.count\",\"tag_set\":[\"instance:crpg02a\",\"region:na\"],\"expression\":\"sum:crpg.users.playing.count{instance:crpg02a,region:na}\",\"scope\":\"instance:crpg02a,region:na\",\"interval\":5,\"length\":62,\"start\":1703277770000,\"end\":1703278634000,\"pointlist\":[[1703277770000,2],[1703277790000,2],[1703277810000,2],[1703277820000,2],[1703277830000,2],[1703277850000,2]],\"display_name\":\"crpg.users.playing.count\",\"attributes\":{}}],\"values\":[],\"times\":[],\"message\":\"\",\"group_by\":[\"instance\",\"region\"]}");
        DatadogGameServerStatsService datadogGameServerStatsService = new(configurationMock.Object, mockHttp.ToHttpClient(), dateTimeMock.Object, gameModeServiceServiceMock.Object);

        var res = await datadogGameServerStatsService.GetGameServerStatsAsync(CancellationToken.None);

        Assert.That(res!.Total.PlayingCount, Is.EqualTo(62));
        Assert.That(res!.Regions[Region.Eu][GameMode.CRPGBattle].PlayingCount, Is.EqualTo(5));
        Assert.That(res!.Regions[Region.Eu][GameMode.CRPGDTV].PlayingCount, Is.EqualTo(55));
        Assert.That(res!.Regions[Region.As][GameMode.CRPGBattle].PlayingCount, Is.EqualTo(0));
        Assert.That(res!.Regions[Region.Na][GameMode.CRPGBattle].PlayingCount, Is.EqualTo(2));

        await datadogGameServerStatsService.GetGameServerStatsAsync(CancellationToken.None);
        Assert.That(mockHttp.GetMatchCount(request), Is.EqualTo(1)); // Caching work
    }
}
