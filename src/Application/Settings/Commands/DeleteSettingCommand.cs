using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Microsoft.Extensions.Logging;
using LoggerFactory = Crpg.Logging.LoggerFactory;

namespace Crpg.Application.Settlements.Commands;

public record DeleteSettingCommand : IMediatorRequest
{
    public int Id { get; set; }

    internal class Handler : IMediatorRequestHandler<DeleteSettingCommand>
    {
        private static readonly ILogger Logger = LoggerFactory.CreateLogger<DeleteSettingCommand>();
        private readonly ICrpgDbContext _db;

        public Handler(ICrpgDbContext db)
        {
            _db = db;
        }

        public async Task<Result> Handle(DeleteSettingCommand req, CancellationToken cancellationToken)
        {
            var setting = await _db.Settings.FindAsync(req.Id);

            if (setting == null)
            {
                return new Result(CommonErrors.SettingNotFound(req.Id));
            }

            _db.Settings.Remove(setting);
            await _db.SaveChangesAsync(cancellationToken);
            Logger.LogInformation("Setting has been deleted. Id: '{0}', Key: '{1}'", setting.Id, setting.Key);
            return Result.NoErrors;
        }
    }
}
