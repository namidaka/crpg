using AutoMapper;
using Crpg.Application.ActivityLogs.Models;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Crpg.Domain.Entities.ActivityLogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LoggerFactory = Crpg.Logging.LoggerFactory;

namespace Crpg.Application.Characters.Queries;

public record GetUserCharacterStatisticsChartsQuery : IMediatorRequest<IList<ActivityLogViewModel>>
{
    public int CharacterId { get; init; }
    public int UserId { get; init; }

    internal class Handler : IMediatorRequestHandler<GetUserCharacterStatisticsChartsQuery, IList<ActivityLogViewModel>>
    {
        private readonly ICrpgDbContext _db;
        private readonly IMapper _mapper;
        private static readonly ILogger Logger = LoggerFactory.CreateLogger<GetUserCharacterStatisticsChartsQuery>();

        public Handler(ICrpgDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<IList<ActivityLogViewModel>>> Handle(GetUserCharacterStatisticsChartsQuery req,
            CancellationToken cancellationToken)
        {
            var activityLogs = await _db.ActivityLogs
                .Include(l => l.Metadata)
                .Where(l =>
                    l.UserId == req.UserId
                    && int.Parse(l.Metadata.First(m => m.Key == "characterId").Value) == req.CharacterId
                    && l.CreatedAt >= DateTime.UtcNow.AddDays(-14)
                    && l.CreatedAt <= DateTime.UtcNow
                    && l.Type == ActivityLogType.CharacterEarned)
                .ToArrayAsync(cancellationToken);

            // Logger.LogInformation("dDDDDDDDDDDDDDDdDDDDDDDDDDDDDDdDDDDDDDDDDDDDDdDDDDDDDDDDDDDDdDDDDDDDDDDDDDDdDDDDDDDDDDDDDDdDDDDDDDDDDDDDD '{0}' dDDDDDDDDDDDDDD", int.Parse(activityLogs[0].Metadata.First(m => m.Key == "characterId").Value));
            return new(_mapper.Map<IList<ActivityLogViewModel>>(activityLogs));
        }
    }
}
