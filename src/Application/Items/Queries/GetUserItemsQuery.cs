using AutoMapper;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Crpg.Application.Items.Models;
using Microsoft.EntityFrameworkCore;

namespace Crpg.Application.Items.Queries;

public record GetUserItemsQuery : IMediatorRequest<IList<UserItemViewModel>>
{
    public int UserId { get; init; }

    internal class Handler : IMediatorRequestHandler<GetUserItemsQuery, IList<UserItemViewModel>>
    {
        private readonly ICrpgDbContext _db;
        private readonly IMapper _mapper;

        public Handler(ICrpgDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<IList<UserItemViewModel>>> Handle(GetUserItemsQuery req, CancellationToken cancellationToken)
        {
            var userItems = await _db.UserItems
                .Include(ui => ui.Item).ThenInclude(i => i!.PersonalItems)
                .Where(ui =>
                    (ui.Item!.Enabled || ui.Item.PersonalItems.Any(pi => pi.UserId == req.UserId))
                    && (ui.UserId == req.UserId || ui.ClanArmoryBorrowedItem!.BorrowerUserId == req.UserId))
                .Include(ui => ui.ClanArmoryItem)
                .Include(ui => ui.ClanArmoryBorrowedItem)
                .ToArrayAsync(cancellationToken);

            return new(_mapper.Map<IList<UserItemViewModel>>(userItems));
        }
    }
}
