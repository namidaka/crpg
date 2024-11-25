using AutoMapper;
using Crpg.Application.Characters.Models;
using Crpg.Application.Common;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Crpg.Application.Common.Services;
using Crpg.Domain.Entities.Characters;
using Crpg.Sdk.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LoggerFactory = Crpg.Logging.LoggerFactory;

namespace Crpg.Application.Characters.Commands;

public record RespecializeCharacterCommand : IMediatorRequest<CharacterViewModel>
{
    public int CharacterId { get; init; }
    public int UserId { get; init; }

    internal class Handler : IMediatorRequestHandler<RespecializeCharacterCommand, CharacterViewModel>
    {
        private static readonly ILogger Logger = LoggerFactory.CreateLogger<RespecializeCharacterCommand>();

        private readonly ICrpgDbContext _db;
        private readonly IMapper _mapper;
        private readonly ICharacterService _characterService;
        private readonly IUserService _userService;
        private readonly IActivityLogService _activityLogService;
        private readonly IDateTime _dateTime;
        private readonly Constants _constants;

        public Handler(ICrpgDbContext db, IMapper mapper, ICharacterService characterService, IUserService userService,
         IActivityLogService activityLogService, IDateTime dateTime, Constants constants)
        {
            _db = db;
            _mapper = mapper;
            _characterService = characterService;
            _userService = userService;
            _activityLogService = activityLogService;
            _dateTime = dateTime;
            _constants = constants;
        }

        public async Task<Result<CharacterViewModel>> Handle(RespecializeCharacterCommand req, CancellationToken cancellationToken)
        {
            var character = await _db.Characters
                .Include(c => c.User)
                .Include(c => c.Limitations)
                .FirstOrDefaultAsync(c => c.Id == req.CharacterId && c.UserId == req.UserId, cancellationToken);
            if (character == null)
            {
                return new(CommonErrors.CharacterNotFound(req.CharacterId, req.UserId));
            }

            bool isRecentUser = await _userService.CheckIsRecentUser(_db, character.User!);

            int price = 0;
            if (!character.ForTournament && !isRecentUser && !IsFreeRespecializationPostWindow(character))
            {
                price = _characterService.CalculateRespecializationCost(character);
                if (character.User!.Gold < price)
                {
                    return new(CommonErrors.NotEnoughGold(price, character.User.Gold));
                }

                character.User.Gold -= price;
                character.Limitations!.LastRespecializeAt = _dateTime.UtcNow;
            }

            _characterService.ResetCharacterCharacteristics(character, true);

            _db.ActivityLogs.Add(_activityLogService.CreateCharacterRespecializedLog(character.UserId, character.Id, price));

            await _db.SaveChangesAsync(cancellationToken);

            Logger.LogInformation("User '{0}' respecialized character '{1}'", req.UserId, req.CharacterId);
            return new(_mapper.Map<CharacterViewModel>(character));
        }

        private bool IsFreeRespecializationPostWindow(Character character)
        {
            return character.Limitations!.LastRespecializeAt + TimeSpan.FromHours(_constants.FreeRespecializePostWindowHours) > _dateTime.UtcNow;
        }
    }
}
