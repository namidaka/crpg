using AutoMapper;
using AutoMapper.QueryableExtensions;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Crpg.Application.Settlements.Models;
using Microsoft.EntityFrameworkCore;

namespace Crpg.Application.Settings.Queries;

public record GetSettingsQuery : IMediatorRequest<IList<SettingViewModel>>
{
    internal class Handler : IMediatorRequestHandler<GetSettingsQuery, IList<SettingViewModel>>
    {
        private readonly ICrpgDbContext _db;
        private readonly IMapper _mapper;

        public Handler(ICrpgDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<IList<SettingViewModel>>> Handle(GetSettingsQuery req,
            CancellationToken cancellationToken)
        {
            return new(await _db.Settings
                .ProjectTo<SettingViewModel>(_mapper.ConfigurationProvider)
                .ToArrayAsync(cancellationToken));
        }
    }
}
