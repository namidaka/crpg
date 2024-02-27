using System.Data.Common;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Crpg.Application.Characters.Models;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Crpg.Application.Users.Models;
using Crpg.Domain.Entities;
using Crpg.Domain.Entities.Characters;
using Crpg.Domain.Entities.Servers;
using Crpg.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Crpg.Application.Characters.Queries;

public record GetLeaderboardQuery : IMediatorRequest<IList<CharacterPublicViewModel>>
{
    public Region? Region { get; set; }
    public CharacterClass? CharacterClass { get; set; }
    public GameMode? GameMode { get; set; }

    internal class Handler : IMediatorRequestHandler<GetLeaderboardQuery, IList<CharacterPublicViewModel>>
    {
        private readonly ICrpgDbContext _db;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public Handler(ICrpgDbContext db, IMapper mapper, IMemoryCache cache)
        {
            _db = db;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<IList<CharacterPublicViewModel>>> Handle(GetLeaderboardQuery req, CancellationToken cancellationToken)
        {
            string cacheKey = GetCacheKey(req);

            if (_cache.TryGetValue(cacheKey, out IList<CharacterPublicViewModel>? results) == false)
            {
                var characters = await _db.Characters
                    .AsNoTracking()
                    .Where(c => (req.Region == null || req.Region == c.User!.Region) &&
                            (req.CharacterClass == null || req.CharacterClass == c.Class))
                    .Select(c => new Character {
                        Id = c.Id,
                        Level = c.Level,
                        Class = c.Class,
                        User = c.User,
                        Statistics = c.Statistics.Where(s => s.GameMode == req.GameMode).ToList(),
                    })
                    .ToArrayAsync(cancellationToken);

                var characterPublicViewModels = characters.Select(c => new CharacterPublicViewModel
                {
                    Id = c.Id,
                    Level = c.Level,
                    Class = c.Class,
                    User = _mapper.Map<UserPublicViewModel>(c.User),
                    Rating = _mapper.Map<CharacterRatingViewModel>(c.Statistics.FirstOrDefault()?.Rating ?? new CharacterRating { CompetitiveValue = 0 }),
                }).ToArray();

                // Todo: use DistinctBy here when EfCore implements it (does not work for now: https://github.com/dotnet/efcore/issues/27470 )
                var topRatedCharactersByRegion = characterPublicViewModels
                    .OrderByDescending(c => c.Rating.CompetitiveValue)
                    .Take(500)
                    .ToArray();

                IList<CharacterPublicViewModel> data = topRatedCharactersByRegion.DistinctBy(c => c.User).Take(50).ToList();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(1));
                _cache.Set(cacheKey, data, cacheOptions);

                return new(data);
            }

            return new(results);
        }

        private string GetCacheKey(GetLeaderboardQuery req)
        {
            List<string> keys = new() { "leaderboard" };

            if (req.Region != null)
            {
                keys.Add(req.Region.ToString()!);
            }

            if (req.CharacterClass != null)
            {
                keys.Add(req.CharacterClass.ToString()!);
            }

            if (req.GameMode != null)
            {
                keys.Add(req.GameMode.ToString()!);
            }

            return string.Join("::", keys);
        }
    }
}
