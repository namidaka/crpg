using Crpg.Application.Clans.Commands;
using Crpg.Application.Clans.Models;
using Crpg.Application.Clans.Queries;
using Crpg.Application.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Crpg.WebApi.Controllers;

[Authorize(Policy = UserPolicy)]
[Route("/Clans/{clanId}/Armory")]
public class ClanArmoryController : BaseController
{
    /// <summary>
    /// Gets the armory items.
    /// </summary>
    /// <param name="clanId">Clan id.</param>
    /// <returns> lsit of items. </returns>
    /// <response code="200">Ok.</response>
    /// <response code="404">Clan was not found.</response>
    [HttpGet]
    public Task<ActionResult<Result<IList<ArmoryItemViewModel>>>> GetList([FromRoute] int clanId)
    {
        return ResultToActionAsync(Mediator.Send(new ArmoryGetListQuery { UserId = CurrentUser.User!.Id }));
    }

    /// <summary>
    /// Add an item to the armory.
    /// </summary>
    /// <param name="clanId">Clan id.</param>
    /// <param name="userItemId">Item id.</param>
    /// <returns> Added item. </returns>
    /// <response code="200">Ok.</response>
    /// <response code="400">Bad request.</response>
    [HttpPost]
    public Task<ActionResult<Result<ArmoryItemViewModel>>> Add([FromRoute] int clanId, [FromBody] int userItemId)
    {
        var req = new ArmoryAddCommand { UserItemId = userItemId, UserId = CurrentUser.User!.Id };
        return ResultToActionAsync(Mediator.Send(req));
    }

    /// <summary>
    /// Remove an item from the armory.
    /// </summary>
    /// <param name="clanId">Clan id.</param>
    /// <param name="userItemId">Item id.</param>
    /// <response code="200">Ok.</response>
    /// <response code="400">Bad request.</response>
    [HttpDelete("{userItemId}")]
    public Task<ActionResult> Remove([FromRoute] int clanId, [FromRoute] int userItemId)
    {
        var req = new ArmoryRemoveCommand { UserItemId = userItemId, UserId = CurrentUser.User!.Id };
        return ResultToActionAsync(Mediator.Send(req));
    }

    /// <summary>
    /// Borrow an item from the armory.
    /// </summary>
    /// <param name="clanId">Clan id.</param>
    /// <param name="userItemId">Item id.</param>
    /// <returns> Borrowed item. </returns>
    /// <response code="200">Ok.</response>
    /// <response code="400">Bad request.</response>
    [HttpPut("{userItemId}/borrow")]
    public Task<ActionResult<Result<ArmoryBorrowViewModel>>> Borrow([FromRoute] int clanId, [FromRoute] int userItemId)
    {
        var req = new ArmoryBorrowCommand { UserItemId = userItemId, UserId = CurrentUser.User!.Id };
        return ResultToActionAsync(Mediator.Send(req));
    }

    /// <summary>
    /// Return an item to the armory.
    /// </summary>
    /// <param name="clanId">Clan id.</param>
    /// <param name="userItemId">Item id.</param>
    /// <response code="200">Ok.</response>
    /// <response code="400">Bad request.</response>
    [HttpPut("{userItemId}/return")]
    public Task<ActionResult> Return([FromRoute] int clanId, [FromRoute] int userItemId)
    {
        var req = new ArmoryReturnCommand { UserItemId = userItemId, UserId = CurrentUser.User!.Id };
        return ResultToActionAsync(Mediator.Send(req));
    }
}
