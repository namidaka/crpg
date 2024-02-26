using AutoMapper;
using Crpg.Application.Captains.Models;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace Crpg.Application.Captains.Queries;

public record GetUserCaptainFormationQuery : IMediatorRequest<CaptainFormationViewModel>
{
    public int UserId { get; init; }
    public int Number { get; init; }

    internal class Handler : IMediatorRequestHandler<GetUserCaptainFormationQuery, CaptainFormationViewModel>
    {
        private readonly ICrpgDbContext _db;
        private readonly IMapper _mapper;

        public Handler(ICrpgDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<CaptainFormationViewModel>> Handle(GetUserCaptainFormationQuery req, CancellationToken cancellationToken)
        {
            var formation = await _db.Captains
                .Where(c => c.UserId == req.UserId)
                .Include(c => c.Formations.Where(f => f.Number == req.Number))
                .FirstOrDefaultAsync(cancellationToken);

            return formation == null
                ? new(CommonErrors.CaptainFormationNotFound(req.Number, req.UserId))
                : new(_mapper.Map<CaptainFormationViewModel>(formation));
        }
    }
}
