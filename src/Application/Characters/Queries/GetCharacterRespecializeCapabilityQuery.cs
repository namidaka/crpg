using Crpg.Application.Characters.Models;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Crpg.Application.Common.Services;
using Microsoft.EntityFrameworkCore;

namespace Crpg.Application.Limitations.Queries;

public record GetCharacterRespecializeCapabilityQuery : IMediatorRequest<CharacterRespecializeCapabilityViewModel>
{
    public int CharacterId { get; init; }
    public int UserId { get; init; }

    internal class Handler : IMediatorRequestHandler<GetCharacterRespecializeCapabilityQuery, CharacterRespecializeCapabilityViewModel>
    {
        private readonly ICrpgDbContext _db;
        private readonly ICharacterService _characterService;

        public Handler(ICrpgDbContext db, ICharacterService characterService)
        {
            _db = db;
            _characterService = characterService;
        }

        public async Task<Result<CharacterRespecializeCapabilityViewModel>> Handle(GetCharacterRespecializeCapabilityQuery req, CancellationToken cancellationToken)
        {
            var character = await _db.Characters
                .AsNoTracking()
                .Include(c => c.Limitations)
                .FirstOrDefaultAsync(c => c.Id == req.CharacterId && c.UserId == req.UserId, cancellationToken);

            if (character == null)
            {
                return new(CommonErrors.CharacterNotFound(req.CharacterId, req.UserId));
            }

            int cost = _characterService.CalculateRespecializationCost(character);

            return new Result<CharacterRespecializeCapabilityViewModel>(new CharacterRespecializeCapabilityViewModel
            {
                Cost = cost,
            });
        }
    }
}
