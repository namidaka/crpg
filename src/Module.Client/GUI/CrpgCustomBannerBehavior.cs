using System;
using System.Collections.Generic;
using System.Text;
using Crpg.Module.Api.Models.Clans;
using Crpg.Module.Common;
using Crpg.Module.GUI.HudExtension;
using Messages.FromBattleServer.ToBattleServerManager;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace Crpg.Module;
internal class CrpgCustomBannerBehavior : MissionNetwork
{
    private Dictionary<int, (int count, CrpgClan clan)> attackerClanNumber = new();
    private Dictionary<int, (int count, CrpgClan clan)> defenderClanNumber = new();
    private int previousAttackerClanId;
    private int previousDefenderClanId;
    public delegate void BannerNameChangedEventHandler(ImageIdentifierVM attackerBanner, ImageIdentifierVM defenderBanner, string attackerName, string defenderName);
    public event BannerNameChangedEventHandler? BannersChanged;
    public ImageIdentifierVM AttackerBanner { get; private set; }
    public ImageIdentifierVM DefenderBanner { get; private set; }
    public string AttackerName { get; private set; }
    public string DefenderName { get; private set; }
    private MissionTimer? _tickTimer;
    internal CrpgCustomBannerBehavior()
    {
        AttackerName = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)).Name.ToString();
        DefenderName = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)).Name.ToString();
        Banner attackerBanner = Mission.Current.Teams.Attacker.Banner;
        Banner defenderBanner = Mission.Current.Teams.Defender.Banner;
        AttackerBanner = new(attackerBanner);
        DefenderBanner = new(defenderBanner);
    }

    public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;
    public override void OnPlayerDisconnectedFromServer(NetworkCommunicator networkPeer) => base.OnPlayerDisconnectedFromServer(networkPeer);
    public override void OnMissionTick(float dt)
    {
        _tickTimer ??= new MissionTimer(1);
        if (!_tickTimer.Check(reset: true))
        {
            var attackerMaxClan = attackerClanNumber.DefaultIfEmpty().MaxBy(c => c.Value.count).Value.clan;
            var defenderMaxClan = defenderClanNumber.DefaultIfEmpty().MaxBy(c => c.Value.count).Value.clan;
            int attackerMaxClanId = attackerMaxClan?.Id ?? -1;
            int defenderMaxClanId = defenderMaxClan?.Id ?? -1;
            if (attackerMaxClanId == previousAttackerClanId && defenderMaxClanId == previousDefenderClanId)
            {
                return;
            }

            string attackerTeamName = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)).Name.ToString();
            string defenderTeamName = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)).Name.ToString();
            Banner attackerBanner = Mission.Current.Teams.Attacker.Banner;
            Banner defenderBanner = Mission.Current.Teams.Defender.Banner;

            if (attackerMaxClan != null)
            {
                attackerBanner = new(new Banner(attackerMaxClan.BannerKey));
                attackerTeamName = attackerMaxClan.Name;
            }

            if (defenderMaxClan != null)
            {
                defenderBanner = new(new Banner(defenderMaxClan.BannerKey));
                defenderTeamName = defenderMaxClan.Name;
            }

            ImageIdentifierVM attackerBannerImageId = new(attackerBanner);
            ImageIdentifierVM defenderBannerImageId = new(defenderBanner);
            BannersChanged?.Invoke(attackerBannerImageId, defenderBannerImageId, attackerTeamName, defenderTeamName);
        }
    }

    public override void OnAgentTeamChanged(Team prevTeam, Team newTeam, Agent agent)
    {
        var crpgPeer = agent.MissionPeer.GetComponent<CrpgPeer>();
        var missionPeer = agent.MissionPeer.GetComponent<MissionPeer>();

        if (missionPeer == null || crpgPeer?.User == null || crpgPeer?.Clan == null)
        {
            return;
        }

        Dictionary<int, (int count, CrpgClan clan)> prevTeamClanNumber;
        Dictionary<int, (int count, CrpgClan clan)> newTeamClanNumber;
        int peerClanId = crpgPeer!.Clan!.Id;
        if (prevTeam.IsAttacker)
        {
            prevTeamClanNumber = attackerClanNumber;
        }
        else
        {
            prevTeamClanNumber = defenderClanNumber;
        }

        if (newTeam.IsAttacker)
        {
            newTeamClanNumber = attackerClanNumber;
        }
        else
        {
            newTeamClanNumber = defenderClanNumber;
        }

        if (prevTeam.Side != BattleSideEnum.None)
        {
            if (prevTeamClanNumber.TryGetValue(peerClanId, out var clan))
            {
                clan.count = Math.Max(clan.count - 1, 0);
                prevTeamClanNumber[peerClanId] = clan;
            }
        }

        if (newTeam.Side != BattleSideEnum.None)
        {
            if (newTeamClanNumber.TryGetValue(peerClanId, out var clan))
            {
                clan.count++;
                newTeamClanNumber[peerClanId] = clan;
            }
            else
            {
                newTeamClanNumber.Add(peerClanId, (1, crpgPeer.Clan));
            }
        }
    }

    public override void OnBehaviorInitialize()
    {
    }

}
