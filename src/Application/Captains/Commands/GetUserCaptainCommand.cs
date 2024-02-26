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
            var user = await _db.Users
                .Where(u => u.Id == req.UserId)
                .Include(u => u.Captain)
                .FirstOrDefaultAsync(cancellationToken);
            if (user == null)
            {
                return new(CommonErrors.UserNotFound(req.UserId));
            }

            if (user.Captain == null)
            {
                user.Captain = CreateCaptain(req.UserId);
                _db.Captains.Add(user.Captain);

                await _db.SaveChangesAsync(cancellationToken);
            }

            var gameUser = _mapper.Map<CaptainViewModel>(user.Captain);
            return new(gameUser);
        }

        private Captain CreateCaptain(int userId)
        {
            Captain captain = new()
            {
                UserId = userId,
                Formations = new List<CaptainFormation>()
                {
                    new() { Number = 1, Weight = 33 },
                    new() { Number = 2, Weight = 33 },
                    new() { Number = 3, Weight = 33 },
                },
            };

            return captain;
        }
    }
}
