﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Crpg.Application.Characters.Models;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Crpg.Domain.Entities;
using Crpg.Domain.Entities.Characters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Crpg.Application.Characters.Queries;

public record GetLeaderboardQuery : IMediatorRequest<IList<CharacterPublicViewModel>>
{
    public Region? Region { get; set; }
    public CharacterClass? CharacterClass { get; set; }

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
                var topRatedCharactersByRegion = await _db.Characters
                .OrderByDescending(c => c.Rating.CompetitiveValue)
                .Where(c => (req.Region == null || req.Region == c.User!.Region)
                            && (req.CharacterClass == null || req.CharacterClass == c.Class))
                .ProjectTo<CharacterPublicViewModel>(_mapper.ConfigurationProvider)
                .ToArrayAsync(cancellationToken);

                IList<CharacterPublicViewModel> data = new List<CharacterPublicViewModel>();

                // DistinctBy don't work https://github.com/dotnet/efcore/issues/27470, let's do it manually.
                foreach (var result in topRatedCharactersByRegion)
                {
                    if (data.Count >= 50)
                    {
                        break;
                    }

                    if (data.Count(c => c.User.Id == result.User.Id) > 0)
                    {
                        continue;
                    }

                    data.Add(result);
                }

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(1));
                _cache.Set(cacheKey, data, cacheOptions);

                return new(data);
            }

            return new(results);
        }

        private string GetCacheKey(GetLeaderboardQuery req)
        {
            return $"leaderboard::{req.Region}::{req.CharacterClass}";
        }
    }
}
