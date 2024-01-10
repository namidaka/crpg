using AutoMapper;
using Crpg.Application.Characters.Models;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace Crpg.Application.Characters.Queries;

public record GetUserCharacterRatingQuery : IMediatorRequest<IList<CharacterRatingViewModel>>
{
    public int CharacterId { get; init; }
    public int UserId { get; init; }

    internal class Handler : IMediatorRequestHandler<GetUserCharacterRatingQuery, IList<CharacterRatingViewModel>>
    {
        private readonly ICrpgDbContext _db;
        private readonly IMapper _mapper;

        public Handler(ICrpgDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<IList<CharacterRatingViewModel>>> Handle(GetUserCharacterRatingQuery req, CancellationToken cancellationToken)
        {
            var characterRating = await _db.Characters
                .AsNoTracking()
                .Where(c => c.Id == req.CharacterId && c.UserId == req.UserId)
                .Select(c => c.Rating)
                .FirstOrDefaultAsync(cancellationToken);

            return characterRating == null
                ? new(CommonErrors.CharacterNotFound(req.CharacterId, req.UserId))
                : new(_mapper.Map<IList<CharacterRatingViewModel>>(characterRating));
        }
    }
}
