using Crpg.Application.Common.Results;
using Crpg.Application.Settings.Queries;
using Crpg.Application.Settlements.Commands;
using Crpg.Application.Settlements.Models;
using Crpg.Domain.Entities.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Crpg.WebApi.Controllers;

public class SettingsController : BaseController
{
    /// <summary>
    /// Get settings.
    /// </summary>
    /// <returns>The settings.</returns>
    /// <response code="200">Ok.</response>
    [HttpGet]
    [Authorize(Policy = UserPolicy)]
    [ResponseCache(Duration = 1 * 60 * 1)] // 1 minutes
    public Task<ActionResult<Result<SettingViewModel>>> GetSettings()
    {
        return ResultToActionAsync(Mediator.Send(new GetSettingsQuery()));
    }

    /// <summary>
    /// Edit setting.
    /// </summary>
    /// <param name="settings">Settings payload.</param>
    /// <returns>The setting object.</returns>
    /// <response code="201">Created.</response>
    /// <response code="400">Bad Request.</response>
    [HttpPatch]
    [Authorize(Policy = AdminPolicy)]
    public Task<ActionResult<Result<SettingViewModel>>> EditSetting([FromBody] EditSettingCommand settings)
    {
        return ResultToCreatedAtActionAsync(nameof(GetSettings), null, null, Mediator.Send(settings));
    }
}
