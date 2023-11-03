using AutoMapper;
using Crpg.Application.Clans.Models;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace Crpg.Application.Clans.Queries;

public record ArmoryGetListQuery : IMediatorRequest<IList<ArmoryItemViewModel>>
{
    public int UserId { get; set; }

    internal class Handler : IMediatorRequestHandler<ArmoryGetListQuery, IList<ArmoryItemViewModel>>
    {
        private readonly ICrpgDbContext _db;
        private readonly IMapper _mapper;

        public Handler(ICrpgDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<IList<ArmoryItemViewModel>>> Handle(ArmoryGetListQuery req, CancellationToken cancellationToken)
        {
            var user = await _db.Users.AsNoTracking()
                .Include(e => e.ClanMembership)
                .FirstOrDefaultAsync(e => e.Id == req.UserId, cancellationToken);
            if (user == null)
            {
                return new(CommonErrors.UserNotFound(req.UserId));
            }

            if (user.ClanMembership == null)
            {
                return new(CommonErrors.UserNotInAClan(req.UserId));
            }

            var clan = await _db.Clans.AsNoTracking()
                .Include(e => e.ArmoryItems).ThenInclude(e => e.UserItem!).ThenInclude(e => e.Item)
                .Include(e => e.ArmoryItems).ThenInclude(e => e.Borrow)
                .FirstOrDefaultAsync(e => e.Id == user.ClanMembership.ClanId, cancellationToken);
            if (clan == null)
            {
                return new(CommonErrors.ClanNotFound(user.ClanMembership.ClanId));
            }

            return new(_mapper.Map<IList<ArmoryItemViewModel>>(clan.ArmoryItems));
        }
    }
}
