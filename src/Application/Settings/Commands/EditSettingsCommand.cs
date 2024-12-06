using System.Text.RegularExpressions;
using AutoMapper;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Crpg.Application.Settings.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Crpg.Application.Settings.Commands;

public record EditSettingsCommand : IMediatorRequest<SettingsViewModel>
{
    public string? Discord { get; set; }
    public string? Steam { get; set; }
    public string? Patreon { get; set; }
    public string? Github { get; set; }
    public string? Reddit { get; set; }
    public string? ModDb { get; set; }
    public string? HappyHours { get; set; }

    public class Validator : AbstractValidator<EditSettingsCommand>
    {
        public Validator()
        {
            RuleFor(c => c.HappyHours)
                .Matches(new Regex(@"(\w+)\|(\d{2}:\d{2})\|(\d{2}:\d{2})\|([\w/]+)", RegexOptions.Compiled));
        }
    }

    internal class Handler : IMediatorRequestHandler<EditSettingsCommand, SettingsViewModel>
    {
        private readonly ICrpgDbContext _db;
        private readonly IMapper _mapper;

        public Handler(ICrpgDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<SettingsViewModel>> Handle(EditSettingsCommand req, CancellationToken cancellationToken)
        {
            var existingSettings = await _db.Settings.FirstOrDefaultAsync(cancellationToken);

            if (existingSettings == null)
            {
                return new(CommonErrors.SettingsNotFound(1));
            }

            existingSettings.Discord = req.Discord ?? existingSettings.Discord;
            existingSettings.Steam = req.Steam ?? existingSettings.Steam;
            existingSettings.Patreon = req.Patreon ?? existingSettings.Patreon;
            existingSettings.Github = req.Github ?? existingSettings.Github;
            existingSettings.Reddit = req.Reddit ?? existingSettings.Reddit;
            existingSettings.ModDb = req.ModDb ?? existingSettings.ModDb;
            existingSettings.HappyHours = req.HappyHours ?? existingSettings.HappyHours;

            await _db.SaveChangesAsync(cancellationToken);

            return new(_mapper.Map<SettingsViewModel>(existingSettings));
        }
    }
}
