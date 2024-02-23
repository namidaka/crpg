using AutoMapper;
using Crpg.Application.Captains.Models;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Crpg.Domain.Entities.Captains;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Crpg.Application.Captains.Commands;

/// <summary>
/// Get or create a user with its character.
/// </summary>
public record GetUserCaptainCommand : IMediatorRequest<CaptainViewModel>
{
    public int UserId { get; init; }
    public class Validator : AbstractValidator<CaptainViewModel>
    {
        public Validator()
        {
        }
    }

    internal class Handler : IMediatorRequestHandler<GetUserCaptainCommand, CaptainViewModel>
    {
        private readonly ICrpgDbContext _db;
        private readonly IMapper _mapper;

        public Handler(ICrpgDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<CaptainViewModel>> Handle(GetUserCaptainCommand req, CancellationToken cancellationToken)
        {
            var captain = await _db.Captains
                .Where(c => c.UserId == req.UserId)
                .FirstOrDefaultAsync(c => c.UserId == req.UserId,
                    cancellationToken);

            if (captain == null)
            {
                captain = CreateCaptain(req.UserId);
                _db.Captains.Add(captain);

                await _db.SaveChangesAsync(cancellationToken);
            }

            var gameUser = _mapper.Map<CaptainViewModel>(captain);
            return new(gameUser);
        }

        private Captain CreateCaptain(int userId)
        {
            Captain captain = new()
            {
                UserId = userId,
                Formations = new List<CaptainFormation>()
                {
                    new() { UserId = userId, Weight = 0.33f },
                    new() { UserId = userId, Weight = 0.33f },
                    new() { UserId = userId, Weight = 0.33f },
                },
            };

            return captain;
        }
    }
}
