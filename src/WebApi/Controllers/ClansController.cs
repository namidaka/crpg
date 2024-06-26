﻿using Crpg.Application.Clans.Commands;
using Crpg.Application.Clans.Commands.Armory;
using Crpg.Application.Clans.Models;
using Crpg.Application.Clans.Queries;
using Crpg.Application.Common.Results;
using Crpg.Application.Items.Models;
using Crpg.Domain.Entities.Clans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Crpg.WebApi.Controllers;

[Authorize(Policy = UserPolicy)]
public class ClansController : BaseController
{
    /// <summary>
    /// Gets a clan from its id.
    /// </summary>
    /// <response code="200">Ok.</response>
    /// <response code="404">Clan was not found.</response>
    [HttpGet("{id}")]
    public Task<ActionResult<Result<ClanViewModel>>> GetClan([FromRoute] int id) =>
        ResultToActionAsync(Mediator.Send(new GetClanQuery { ClanId = id }));

    /// <summary>
    /// Gets the members of a clan.
    /// </summary>
    /// <response code="200">Ok.</response>
    /// <response code="404">Clan was not found.</response>
    [HttpGet("{id}/members")]
    public Task<ActionResult<Result<IList<ClanMemberViewModel>>>> GetClanMembers([FromRoute] int id) =>
        ResultToActionAsync(Mediator.Send(new GetClanMembersQuery { ClanId = id }));

    /// <summary>
    /// Update a clan member.
    /// </summary>
    /// <param name="clanId">Clan id.</param>
    /// <param name="userId">User id.</param>
    /// <param name="req">The entire member with the updated values.</param>
    /// <returns>The updated member.</returns>
    /// <response code="200">Updated.</response>
    /// <response code="400">Bad Request.</response>
    [HttpPut("{clanId}/members/{userId}")]
    public Task<ActionResult<Result<ClanMemberViewModel>>> UpdateClanMember([FromRoute] int clanId,
        [FromRoute] int userId, [FromBody] UpdateClanMemberCommand req)
    {
        req = req with { UserId = CurrentUser.User!.Id, ClanId = clanId, MemberId = userId };
        return ResultToActionAsync(Mediator.Send(req));
    }

    /// <summary>
    /// Gets all clans.
    /// </summary>
    /// <response code="200">Ok.</response>
    [HttpGet]
    public Task<ActionResult<Result<IList<ClanWithMemberCountViewModel>>>> GetClans() =>
        ResultToActionAsync(Mediator.Send(new GetClansQuery()));

    /// <summary>
    /// Creates a clan.
    /// </summary>
    /// <param name="clan">Clan info.</param>
    /// <returns>The created clan.</returns>
    /// <response code="201">Created.</response>
    /// <response code="400">Bad Request.</response>
    [HttpPost]
    public Task<ActionResult<Result<ClanViewModel>>> CreateClan([FromBody] CreateClanCommand clan)
    {
        clan = clan with { UserId = CurrentUser.User!.Id };
        return ResultToCreatedAtActionAsync(nameof(GetClan), null, b => new { id = b.Id },
            Mediator.Send(clan));
    }

    /// <summary>
    /// Updates a clan.
    /// </summary>
    /// <param name="clanId">Clan id.</param>
    /// <param name="clan">The clan update.</param>
    /// <returns>The updated clan.</returns>
    /// <response code="200">Updated.</response>
    /// <response code="400">Bad Request.</response>
    [HttpPut("{clanId}")]
    public Task<ActionResult<Result<ClanViewModel>>> UpdateClan([FromRoute] int clanId, [FromBody] UpdateClanCommand clan)
    {
       clan = clan with { UserId = CurrentUser.User!.Id, ClanId = clanId };
       return ResultToActionAsync(Mediator.Send(clan));
    }

    /// <summary>
    /// Kick a clan member of leave a clan.
    /// </summary>
    /// <returns>The created clan.</returns>
    /// <response code="204">Kicked or left.</response>
    /// <response code="400">Bad Request.</response>
    [HttpDelete("{clanId}/members/{userId}")]
    public Task<ActionResult> KickClanMember(int clanId, int userId)
    {
        return ResultToActionAsync(Mediator.Send(new KickClanMemberCommand
        {
            UserId = CurrentUser.User!.Id,
            ClanId = clanId,
            KickedUserId = userId,
        }, CancellationToken.None));
    }

    /// <summary>
    /// Get users invited to the clan or users requesting to join the clan.
    /// </summary>
    /// <returns>The invitations.</returns>
    /// <response code="200">Ok.</response>
    /// <response code="400">Bad Request.</response>
    [HttpGet("{clanId}/invitations")]
    public Task<ActionResult<Result<IList<ClanInvitationViewModel>>>> GetClanInvitations([FromRoute] int clanId,
        [FromQuery(Name = "type[]")] ClanInvitationType[] types,
        [FromQuery(Name = "status[]")] ClanInvitationStatus[] statuses)
    {
        return ResultToActionAsync(Mediator.Send(new GetClanInvitationsQuery
        {
            UserId = CurrentUser.User!.Id,
            ClanId = clanId,
            Types = types,
            Statuses = statuses,
        }));
    }

    /// <summary>
    /// Invite user to clan or request to join a clan.
    /// </summary>
    /// <returns>The created or existing invitation.</returns>
    /// <response code="201">Invitation created.</response>
    /// <response code="400">Bad Request.</response>
    [HttpPost("{clanId}/invitations")]
    public Task<ActionResult<Result<ClanInvitationViewModel>>> InviteToClan([FromRoute] int clanId, [FromBody] InviteClanMemberCommand invite)
    {
        invite = invite with { UserId = CurrentUser.User!.Id, ClanId = clanId };
        return ResultToActionAsync(Mediator.Send(invite));
    }

    /// <summary>
    /// Accept/Decline request/offer to join a clan.
    /// </summary>
    /// <returns>The created or existing invitation.</returns>
    /// <response code="200">Responded successfully.</response>
    /// <response code="400">Bad Request.</response>
    [HttpPut("{clanId}/invitations/{invitationId}/response")]
    public Task<ActionResult<Result<ClanInvitationViewModel>>> RespondToClanInvitation([FromRoute] int clanId,
        [FromRoute] int invitationId, [FromBody] RespondClanInvitationCommand invite)
    {
        invite = invite with { UserId = CurrentUser.User!.Id, ClanId = clanId, ClanInvitationId = invitationId };
        return ResultToActionAsync(Mediator.Send(invite));
    }

    /// <summary>
    /// Gets the armory items.
    /// </summary>
    /// <param name="clanId">Clan id.</param>
    /// <returns>List of clan armory items.</returns>
    /// <response code="200">Ok.</response>
    /// <response code="404">Clan was not found.</response>
    [HttpGet("{clanId}/armory")]
    public Task<ActionResult<Result<IList<ClanArmoryItemViewModel>>>> GetClanArmory([FromRoute] int clanId)
    {
        return ResultToActionAsync(Mediator.Send(new GetClanArmoryQuery { UserId = CurrentUser.User!.Id, ClanId = clanId }));
    }

    /// <summary>
    /// Add an item to the armory.
    /// </summary>
    /// <param name="clanId">Clan id.</param>
    /// <param name="req">Item id.</param>
    /// <returns>Added item.</returns>
    /// <response code="201">Item added to clan armory.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="409">Conflict.</response>
    [HttpPost("{clanId}/armory")]
    public Task<ActionResult<Result<ClanArmoryItemViewModel>>> AddClanArmoryItem([FromRoute] int clanId, [FromBody] AddItemToClanArmoryCommand req)
    {
        req = req with { UserId = CurrentUser.User!.Id, ClanId = clanId };
        return ResultToActionAsync(Mediator.Send(req));
    }

    /// <summary>
    /// Remove an item from the armory.
    /// </summary>
    /// <param name="clanId">Clan id.</param>
    /// <param name="userItemId">Item id.</param>
    /// <response code="204">Item removed from clan armory.</response>
    /// <response code="400">Bad request.</response>
    [HttpDelete("{clanId}/armory/{userItemId}")]
    public Task<ActionResult> RemoveClanArmoryItem([FromRoute] int clanId, [FromRoute] int userItemId)
    {
        var req = new RemoveItemFromClanArmoryCommand { UserItemId = userItemId, UserId = CurrentUser.User!.Id, ClanId = clanId };
        return ResultToActionAsync(Mediator.Send(req));
    }

    /// <summary>
    /// Borrow an item from the armory.
    /// </summary>
    /// <param name="clanId">Clan id.</param>
    /// <param name="userItemId">Item id.</param>
    /// <returns> Borrowed item.</returns>
    /// <response code="200">Ok.</response>
    /// <response code="400">Bad request.</response>
    [HttpPut("{clanId}/armory/{userItemId}/borrow")]
    public Task<ActionResult<Result<ClanArmoryBorrowedItemViewModel>>> BorrowClanArmoryItem([FromRoute] int clanId, [FromRoute] int userItemId)
    {
        var req = new BorrowItemFromClanArmoryCommand { UserItemId = userItemId, UserId = CurrentUser.User!.Id, ClanId = clanId };
        return ResultToActionAsync(Mediator.Send(req));
    }

    /// <summary>
    /// Return an item to the armory.
    /// </summary>
    /// <param name="clanId">Clan id.</param>
    /// <param name="userItemId">Item id.</param>
    /// <response code="200">Ok.</response>
    /// <response code="400">Bad request.</response>
    [HttpPut("{clanId}/armory/{userItemId}/return")]
    public Task<ActionResult> ReturnClanArmoryItem([FromRoute] int clanId, [FromRoute] int userItemId)
    {
        var req = new ReturnItemToClanArmoryCommand { UserItemId = userItemId, UserId = CurrentUser.User!.Id, ClanId = clanId };
        return ResultToActionAsync(Mediator.Send(req));
    }
}
