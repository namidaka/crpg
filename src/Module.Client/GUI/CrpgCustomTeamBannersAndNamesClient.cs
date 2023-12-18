using System;
using System.Collections.Generic;
using System.Text;
using Crpg.Module.Api.Models.Clans;
using Crpg.Module.Common;
using Crpg.Module.Common.Network;
using Crpg.Module.GUI.HudExtension;
using Messages.FromBattleServer.ToBattleServerManager;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace Crpg.Module;
internal class CrpgCustomTeamBannersAndNamesClient : MissionNetwork
{
    private Dictionary<int, (int count, CrpgClan clan)> attackerClanNumber = new();
    private Dictionary<int, (int count, CrpgClan clan)> defenderClanNumber = new();
    private int previousAttackerClanId;
    private int previousDefenderClanId;
    public delegate void BannerNameChangedEventHandler(BannerCode attackerBanner, BannerCode defenderBanner, string attackerName, string defenderName);
    public event BannerNameChangedEventHandler? BannersChanged;
    public BannerCode AttackerBanner { get; private set; } = BannerCode.CreateFrom(string.Empty);
    public BannerCode DefenderBanner { get; private set; } = BannerCode.CreateFrom(string.Empty);
    public string AttackerName { get; private set; } = string.Empty;
    public string DefenderName { get; private set; } = string.Empty;
    private IRoundComponent? _roundComponent;
    private MissionTimer? _tickTimer;
    internal CrpgCustomTeamBannersAndNamesClient()
    {
    }

    public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;
    protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
    {
        base.AddRemoveMessageHandlers(registerer);
        registerer.Register<UpdateTeamBannersAndNames>(HandleUpdateTeamBannersAndNames);
    }

    private void HandleUpdateTeamBannersAndNames(UpdateTeamBannersAndNames message)
    {
        AttackerBanner = message.AttackerBanner;
        DefenderBanner = message.DefenderBanner;
        AttackerName = message.AttackerName;
        DefenderName = message.DefenderName;
        BannersChanged?.Invoke(AttackerBanner, DefenderBanner, AttackerName, DefenderName);
    }
}
