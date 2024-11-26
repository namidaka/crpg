using Crpg.Application.Common.Results;
using Crpg.Application.Settings.Queries;
using Crpg.Application.Settlements.Commands;
using Crpg.Application.Settlements.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Crpg.WebApi.Controllers;

// [Authorize(Policy = AdminPolicy)]
[Authorize(Policy = UserPolicy)]
public class SettingsController : BaseController
{
    /// <summary>
    /// TODO:
    /// </summary>
    /// <returns>TODO: </returns>
    /// <response code="200">Ok.</response>
    [HttpGet]
    [ResponseCache(Duration = 1 * 60 * 1)] // 1 minutes
    public Task<ActionResult<Result<IList<SettingViewModel>>>> GetSettings()
    {
        return ResultToActionAsync(Mediator.Send(new GetSettingsQuery()));
    }

    /// <summary>
    /// TODO:
    /// </summary>
    /// <param name="setting">TODO</param>
    /// <returns>TODO:</returns>
    /// <response code="201">Created.</response>
    /// <response code="400">Bad Request.</response>
    [HttpPost]
    public Task<ActionResult<Result<SettingViewModel>>> SetSetting([FromBody] SetSettingCommand setting)
    {
        return ResultToCreatedAtActionAsync(nameof(GetSettings), null, null, Mediator.Send(setting));
    }

    /// <summary>
    /// TODO:
    /// </summary>
    /// <returns>TODO:</returns>
    /// <response code="200">Ok.</response>
    [HttpDelete("settings/{id}")]
    public Task<ActionResult> DeleteSetting(int id)
    {
        return ResultToActionAsync(Mediator.Send(new DeleteSettingCommand { Id = id, }, CancellationToken.None));
    }
}
