using AutoMapper;
using AutoMapper.QueryableExtensions;
using Crpg.Application.Characters.Models;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Crpg.Domain.Entities;
using Crpg.Domain.Entities.Characters;
using Microsoft.EntityFrameworkCore;

namespace Crpg.Application.Characters.Queries;

public record GetLeaderboardQuery : IMediatorRequest<IList<CharacterPublicViewModel>>
{
    public Region? Region { get; set; }

    public CharacterClass? CharacterClass { get; set; }

    internal class Handler : IMediatorRequestHandler<GetLeaderboardQuery, IList<CharacterPublicViewModel>>
    {
        private readonly ICrpgDbContext _db;
        private readonly IMapper _mapper;

        public Handler(ICrpgDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<IList<CharacterPublicViewModel>>> Handle(GetLeaderboardQuery req, CancellationToken cancellationToken)
        {
            var topRatedCharactersByRegion = await _db.Characters
                .OrderByDescending(c => c.Rating.CompetitiveValue)
                .Where(c => (req.Region == null || req.Region == c.User!.Region)
                            && (req.CharacterClass == null || req.CharacterClass == c.Class))
                .ProjectTo<CharacterPublicViewModel>(_mapper.ConfigurationProvider)
                .ToArrayAsync(cancellationToken);

            var results = new List<CharacterPublicViewModel>();

            // DistinctBy don't work https://github.com/dotnet/efcore/issues/27470, let's do it manually.
            foreach (var result in topRatedCharactersByRegion)
            {
                if (results.Count >= 50)
                {
                    break;
                }

                if (results.Count(c => c.User.Id == result.User.Id) > 0)
                {
                    continue;
                }

                results.Add(result);
            }

            return new(results);
        }
    }
}
