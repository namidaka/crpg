using Crpg.Application.Common;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Crpg.Application.Common.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LoggerFactory = Crpg.Logging.LoggerFactory;

namespace Crpg.Application.Characters.Commands;

public record RewardRecentUserCommand : IMediatorRequest
{
    internal class Handler : IMediatorRequestHandler<RewardRecentUserCommand>
    {
        private static readonly ILogger Logger = LoggerFactory.CreateLogger<RewardRecentUserCommand>();

        private readonly ICrpgDbContext _db;
        private readonly Constants _constants;
        private readonly IExperienceTable _experienceTable;
        private readonly ICharacterService _characterService;

        public Handler(
            ICrpgDbContext db,
            Constants constants,
            ICharacterService characterService,
            IExperienceTable experienceTable)
        {
            _db = db;
            _constants = constants;
            _characterService = characterService;
            _experienceTable = experienceTable;
        }

        public async Task<Result> Handle(RewardRecentUserCommand req, CancellationToken cancellationToken)
        {
            var users = await _db.Users
                .Include(u => u.Characters)
                .Where(u => u.ExperienceMultiplier == _constants.DefaultExperienceMultiplier
                            && u.Characters.All(c => c.Level != _constants.StartedCharacterLevel && c.Level < _constants.StartedCharacterLevel)
                            && u.Characters.Sum(c => c.Experience) < 12000000)
                .ToListAsync(cancellationToken);

            foreach (var user in users)
            {
                user.Gold = Math.Max(user.Gold + 25000, 0);
                Logger.LogInformation("Recent User '{0}' rewarded with 25000 gold", user.Id);

                var highestLevelCharacter = user.Characters.OrderByDescending(c => c.Experience).FirstOrDefault();

                if (highestLevelCharacter != null)
                {
                   int experienceToGive = _experienceTable.GetExperienceForLevel(_constants.StartedCharacterLevel) - highestLevelCharacter.Experience;
                   _characterService.GiveExperience(highestLevelCharacter, experienceToGive, useExperienceMultiplier: false);
                   Logger.LogInformation("Character {0} has been rewarded with {1} experience and is now level 30",  highestLevelCharacter.Id, experienceToGive);
                }
            }

            await _db.SaveChangesAsync(cancellationToken);
            return new Result();
        }
    }
}
