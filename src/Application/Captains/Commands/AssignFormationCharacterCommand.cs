using AutoMapper;
using Crpg.Application.Captains.Models;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace Crpg.Application.Captains.Commands;

public record AssignFormationCharacterCommand : IMediatorRequest<CaptainFormationViewModel>
{
    public int? CharacterId { get; init; }
    public int UserId { get; init; }
    public int Number { get; init; }

    internal class Handler : IMediatorRequestHandler<AssignFormationCharacterCommand, CaptainFormationViewModel>
    {
        private readonly ICrpgDbContext _db;

        private readonly IMapper _mapper;

        public Handler(ICrpgDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<CaptainFormationViewModel>> Handle(AssignFormationCharacterCommand req, CancellationToken cancellationToken)
        {
            var captain = await _db.Captains
                .Where(c => c.UserId == req.UserId)
                .Select(c => new
                {
                    Formation = c.Formations.FirstOrDefault(f => f.Number == req.Number),
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (captain?.Formation == null)
            {
                return new(CommonErrors.CaptainFormationNotFound(req.Number, req.UserId));
            }

            if (req.CharacterId == null){
                captain.Formation.CharacterId = null;
                await _db.SaveChangesAsync(cancellationToken);
                return new(_mapper.Map<CaptainFormationViewModel>(captain.Formation));
            }

            var character = await _db.Characters
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == req.UserId && c.Id == req.CharacterId.Value, cancellationToken);
            if (character == null)
            {
                return new(CommonErrors.CharacterNotFound(req.CharacterId.Value, req.UserId));
            }

            if (character.ForTournament)
            {
                return new(CommonErrors.CharacterForTournament(req.CharacterId.Value));
            }

            captain.Formation.CharacterId = req.CharacterId.Value;

            await _db.SaveChangesAsync(cancellationToken);
            return new(_mapper.Map<CaptainFormationViewModel>(captain.Formation));
        }
    }
}
