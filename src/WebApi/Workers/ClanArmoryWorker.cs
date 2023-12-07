using Crpg.Application.Clans.Commands.Armory;
using MediatR;

namespace Crpg.WebApi.Workers;

public class ClanArmoryWorker : BackgroundService
{
    private static readonly ILogger Logger = Logging.LoggerFactory.CreateLogger<ClanArmoryWorker>();

    private readonly TimeSpan _timeout = TimeSpan.FromDays(3);

    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ClanArmoryWorker(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var cmd = new ReturnUnusedItemsToClanArmoryCommand { Timeout = _timeout };
                await mediator.Send(cmd, stoppingToken);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "An error occured while returning unused clan armory items");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
