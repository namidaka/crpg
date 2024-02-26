using AutoMapper;
using Crpg.Application.Captains.Models;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace Crpg.Application.Captains.Commands;

public record UpdateFormationWeightCommand : IMediatorRequest<CaptainFormationViewModel>
{
    public int UserId { get; init; }
    public int Weight { get; init; }
    public int Number { get; init; }

    internal class Handler : IMediatorRequestHandler<UpdateFormationWeightCommand, CaptainFormationViewModel>
    {
        private readonly ICrpgDbContext _db;
        private readonly IMapper _mapper;

        public Handler(ICrpgDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<CaptainFormationViewModel>> Handle(UpdateFormationWeightCommand req, CancellationToken cancellationToken)
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

            captain.Formation.Weight = req.Weight;

            await _db.SaveChangesAsync(cancellationToken);
            return new(_mapper.Map<CaptainFormationViewModel>(captain.Formation));
        }
    }
}
