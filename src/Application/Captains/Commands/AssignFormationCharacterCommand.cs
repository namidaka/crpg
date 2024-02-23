using AutoMapper;
using Crpg.Application.Captains.Models;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace Crpg.Application.Captains.Commands;

public record AssignFormationCharacterCommand : IMediatorRequest
{
    public int CharacterId { get; init; }
    public int UserId { get; init; }
    public int FormationId { get; init; }
    public bool Active { get; init; }

    internal class Handler : IMediatorRequestHandler<AssignFormationCharacterCommand>
    {
        private readonly ICrpgDbContext _db;

        private readonly IMapper _mapper;

        public Handler(ICrpgDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result> Handle(AssignFormationCharacterCommand req, CancellationToken cancellationToken)
        {
            var character = await _db.Characters
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == req.UserId && c.Id == req.CharacterId, cancellationToken);
            if (character == null)
            {
                return new Result(CommonErrors.CharacterNotFound(req.CharacterId, req.UserId));
            }

            if (character.ForTournament)
            {
                return new Result(CommonErrors.CharacterForTournament(req.CharacterId));
            }

            var captain = await _db.Captains
                .Where(c => c.UserId == req.UserId)
                .Include(c => c.Formations.Where(f => f.Id == req.FormationId))
                .FirstOrDefaultAsync(cancellationToken);
            if (captain == null)
            {
                return new Result(CommonErrors.CaptainFormationNotFound(req.FormationId, req.UserId));
            }

            var formation = _mapper.Map<CaptainFormationViewModel>(captain);

            formation.Troop = req.Active ? character : null;

            await _db.SaveChangesAsync(cancellationToken);
            return new Result();
        }
    }
}
