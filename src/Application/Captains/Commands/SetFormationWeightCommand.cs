using AutoMapper;
using Crpg.Application.Captains.Models;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace Crpg.Application.Captains.Commands;

public record SetFormationWeightCommand : IMediatorRequest
{
    public int UserId { get; init; }
    public int Weight { get; init; }
    public int FormationId { get; init; }

    internal class Handler : IMediatorRequestHandler<SetFormationWeightCommand>
    {
        private readonly ICrpgDbContext _db;
        private readonly IMapper _mapper;

        public Handler(ICrpgDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result> Handle(SetFormationWeightCommand req, CancellationToken cancellationToken)
        {
            var captain = await _db.Captains
                .Where(c => c.UserId == req.UserId)
                .Include(c => c.Formations.Where(f => f.Id == req.FormationId))
                .FirstOrDefaultAsync(cancellationToken);
            if (captain == null)
            {
                return new Result(CommonErrors.CaptainFormationNotFound(req.FormationId, req.UserId));
            }

            var formation = _mapper.Map<CaptainFormationViewModel>(captain);

            formation.Weight = req.Weight;

            await _db.SaveChangesAsync(cancellationToken);
            return new Result();
        }
    }
}
