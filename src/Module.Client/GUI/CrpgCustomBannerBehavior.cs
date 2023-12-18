using System;
using System.Collections.Generic;
using System.Text;
using Crpg.Module.Api.Models.Clans;
using Crpg.Module.Common;
using Crpg.Module.GUI.HudExtension;
using Messages.FromBattleServer.ToBattleServerManager;
using TaleWorlds.CampaignSystem.LogEntries;
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
    public delegate void BannerNameChangedEventHandler(Banner? attackerBanner, Banner? defenderBanner, string attackerName, string defenderName);
    public event BannerNameChangedEventHandler? BannersChanged;
    public Banner? AttackerBanner { get; private set; }
    public Banner? DefenderBanner { get; private set; }
    public string AttackerName { get; private set; }
    public string DefenderName { get; private set; }
    private IRoundComponent? _roundComponent;
    private MissionTimer? _tickTimer;
    internal CrpgCustomBannerBehavior()
    {
        AttackerName = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)).Name.ToString();
        DefenderName = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)).Name.ToString();
        AttackerBanner = Mission.Current?.Teams.Attacker?.Banner;
        DefenderBanner = Mission.Current?.Teams.Defender?.Banner;
    }

    public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;
    public void UpdateBanner()
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
            Banner? attackerBanner = Mission.Current?.Teams.Attacker?.Banner;
            Banner? defenderBanner = Mission.Current?.Teams.Defender?.Banner;

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

            AttackerBanner = attackerBanner;
            DefenderBanner = defenderBanner;
            BannersChanged?.Invoke(AttackerBanner, DefenderBanner, attackerTeamName, defenderTeamName);
            MissionMultiplayerGameModeBaseClient missionBehavior = base.Mission.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
            _roundComponent = ((missionBehavior != null) ? missionBehavior.RoundComponent : null);
    }

    private void InitializeClanDictionaries()
    {
        foreach (var networkPeer in GameNetwork.NetworkPeers)
        {
            var crpgPeer = networkPeer?.GetComponent<CrpgPeer>();
            var missionPeer = networkPeer?.GetComponent<MissionPeer>();

            if (missionPeer == null || crpgPeer?.User == null || crpgPeer?.Clan == null || missionPeer.Team == null)
            {
                continue;
            }

            if (missionPeer.Team.Side == BattleSideEnum.None)
            {
                continue;
            }

            Dictionary<int, (int count, CrpgClan clan)> ClanNumber;
            int peerClanId = crpgPeer!.Clan!.Id;


            if (missionPeer.Team.Side == BattleSideEnum.Attacker)
            {
                ClanNumber = attackerClanNumber;
            }
            else
            {
                ClanNumber = defenderClanNumber;
            }

            if (ClanNumber.TryGetValue(peerClanId, out var clan))
            {
                clan.count++;
                ClanNumber[peerClanId] = clan;
            }
            else
            {
                ClanNumber.Add(peerClanId, (1, crpgPeer.Clan));
            }
        }
    }
    public override void OnBehaviorInitialize()
    {
        base.OnBehaviorInitialize();
        var missionNetworkComponent = Mission.GetMissionBehavior<MissionNetworkComponent>();
        missionNetworkComponent.OnMyClientSynchronized += InitializeClanDictionaries;
        missionNetworkComponent.OnMyClientSynchronized += UpdateBanner;
        if (_roundComponent != null)
        {
            _roundComponent.OnRoundStarted += InitializeClanDictionaries;
            _roundComponent.OnRoundStarted += UpdateBanner;
        }
    }
    public override void OnRemoveBehavior()
    {
        base.OnRemoveBehavior();
        var missionNetworkComponent = Mission.GetMissionBehavior<MissionNetworkComponent>();
        if (_roundComponent != null)
        {
            _roundComponent.OnRoundStarted -= InitializeClanDictionaries;
            _roundComponent.OnRoundStarted -= UpdateBanner;
        }
    }
}
