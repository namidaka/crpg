using Crpg.Application.Clans.Commands.Armory;
using MediatR;

namespace Crpg.WebApi.Workers;

public class ClanArmoryWorker : BackgroundService
{
    private readonly TimeSpan _timeout = TimeSpan.FromDays(3);

    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ClanArmoryWorker(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var cmd = new ReturnUnusedClanArmoryItemsCommand { Timeout = _timeout };
            await mediator.Send(cmd, stoppingToken);
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
