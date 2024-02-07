using AutoMapper;
using Crpg.Application.Captains.Models;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

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
            var captain = await _db.Captains
                .Where(c => c.UserId == req.UserId)
                .Select(c => new
                {
                    c.Formations,
                })
                .FirstOrDefaultAsync(cancellationToken);

            return captain == null
                ? new(CommonErrors.CaptainNotFound(req.UserId))
                : new(_mapper.Map<IList<CaptainFormationViewModel>>(captain.Formations));
        }
    }
}
