using AutoMapper;
using Crpg.Application.Captains.Models;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace Crpg.Application.Captains.Queries;

public record GetUserCaptainQuery : IMediatorRequest<CaptainViewModel>
{
    public int UserId { get; init; }

    internal class Handler : IMediatorRequestHandler<GetUserCaptainQuery, CaptainViewModel>
    {
        private readonly ICrpgDbContext _db;
        private readonly IMapper _mapper;

        public Handler(ICrpgDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<CaptainViewModel>> Handle(GetUserCaptainQuery req, CancellationToken cancellationToken)
        {
            var captain = await _db.Captains
                .Where(c => c.UserId == req.UserId)
                .FirstOrDefaultAsync(cancellationToken);

            return captain == null
                ? new(CommonErrors.CaptainNotFound(req.UserId))
                : new(_mapper.Map<CaptainViewModel>(captain));
        }
    }
}
