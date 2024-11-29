using AutoMapper;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Crpg.Application.Settlements.Models;
using Crpg.Domain.Entities.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LoggerFactory = Crpg.Logging.LoggerFactory;

namespace Crpg.Application.Settlements.Commands;

public record EditSettingCommand : IMediatorRequest<SettingViewModel>
{
    private readonly SettingEdition updatedSettings = default!;

    internal class Handler : IMediatorRequestHandler<EditSettingCommand, SettingViewModel>
    {
        private static readonly ILogger Logger = LoggerFactory.CreateLogger<AddSettlementItemCommand>();
        private readonly ICrpgDbContext _db;
        private readonly IMapper _mapper;

        public Handler(ICrpgDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<SettingViewModel>> Handle(EditSettingCommand req, CancellationToken cancellationToken)
        {
            var existingSettings = await _db.Settings.FirstOrDefaultAsync();

            if (existingSettings == null)
            {
                return new(CommonErrors.SettingsNotFound(1));
            }

            foreach (var setting in typeof(EditSettingCommand).GetProperties())
            {
                object? newValue = setting.GetValue(req.updatedSettings);
                if (newValue != null)
                {
                    var targetProperty = typeof(Setting).GetProperty(setting.Name);
                    targetProperty?.SetValue(existingSettings, newValue);
                }
            }

            await _db.SaveChangesAsync(cancellationToken);

            return new(_mapper.Map<SettingViewModel>(existingSettings));
        }
    }
}
