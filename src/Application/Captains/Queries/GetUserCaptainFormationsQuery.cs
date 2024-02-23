using AutoMapper;
using Crpg.Application.Captains.Models;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace Crpg.Application.Captains.Queries;

public record GetUserCaptainFormationsQuery : IMediatorRequest<IList<CaptainFormationViewModel>>
{
    public int UserId { get; init; }

    internal class Handler : IMediatorRequestHandler<GetUserCaptainFormationsQuery, IList<CaptainFormationViewModel>>
    {
        private readonly ICrpgDbContext _db;
        private readonly IMapper _mapper;

        public Handler(ICrpgDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<IList<CaptainFormationViewModel>>> Handle(GetUserCaptainFormationsQuery req, CancellationToken cancellationToken)
        {
            var formations = await _db.Captains
                .Where(c => c.UserId == req.UserId)
                .Include(c => c.Formations.OrderByDescending(f => f.Id))
                .FirstOrDefaultAsync(cancellationToken);

            return formations == null
                ? new(CommonErrors.CaptainNotFound(req.UserId))
                : new(_mapper.Map<IList<CaptainFormationViewModel>>(formations));
        }
    }
}
