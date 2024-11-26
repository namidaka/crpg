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

public record SetSettingCommand : IMediatorRequest<SettingViewModel>
{
    public string Name { get; init; } = string.Empty;
    public string Key { get; set; } = default!;
    public string Value { get; set; } = default!;
    public string? Description { get; set; }
    public SettingDataType DataType { get; set; }

    internal class Handler : IMediatorRequestHandler<SetSettingCommand, SettingViewModel>
    {
        private static readonly ILogger Logger = LoggerFactory.CreateLogger<AddSettlementItemCommand>();
        private readonly ICrpgDbContext _db;
        private readonly IMapper _mapper;

        public Handler(ICrpgDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<SettingViewModel>> Handle(SetSettingCommand req, CancellationToken cancellationToken)
        {
            var setting = await _db.Settings.FirstOrDefaultAsync(s => s.Key == req.Key);

            if (setting != null)
            {
                setting.Value = req.Value;
                setting.DataType = req.DataType;
                setting.Description = req.Description ?? setting.Description;
                Logger.LogInformation(
                        "Setting has been updated. Key: '{0}', Value: '{1}', DataType: '{2}'",
                        req.Key, req.Value.ToString(), req.DataType.ToString());
            }
            else
            {
                setting = new Setting
                {
                    Key = req.Key,
                    Value = req.Value,
                    DataType = req.DataType,
                    Description = req.Description,
                };
                _db.Settings.Add(setting);
                Logger.LogInformation(
                        "Setting has been added. Key: '{0}', Value: '{1}', DataType: '{2}'",
                        req.Key, req.Value.ToString(), req.DataType.ToString());
            }

            await _db.SaveChangesAsync(cancellationToken);

            return new(_mapper.Map<SettingViewModel>(setting));
        }
    }
}
