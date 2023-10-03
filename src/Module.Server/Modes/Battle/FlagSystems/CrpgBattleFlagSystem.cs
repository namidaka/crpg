using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Objects;
using Timer = TaleWorlds.Core.Timer;

namespace Crpg.Module.Modes.Battle.FlagSystems;
internal class CrpgBattleFlagSystem : AbstractFlagSystem
{
    private int initiallySpawnedPlayerCount;

    public CrpgBattleFlagSystem(Mission mission, MultiplayerGameNotificationsComponent notificationsComponent, CrpgBattleClient battleClient)
        : base(mission, notificationsComponent, battleClient)
    {
    }

    public override void CheckForManipulationOfFlags()
    {
        if (WereFlagsManipulated())
        {
            return;
        }

        Timer checkFlagRemovalTimer = GetCheckFlagRemovalTimer(Mission.CurrentTime, GetBattleClient().FlagManipulationTime);
        if (!checkFlagRemovalTimer.Check(Mission.CurrentTime))
        {
            return;
        }

        var randomFlag = GetRandomFlag();
        SpawnFlag(randomFlag);

        SetWereFlagsManipulated(true);

        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage(new CrpgBattleSpawnFlagMessage
        {
            FlagChar = randomFlag.FlagChar,
        });
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);

        GetBattleClient().ChangeNumberOfFlags();
        Debug.Print("Random Flag has been spawned");
    }

    public void IncrementInitiallySpawnedPlayerCount()
    {
        initiallySpawnedPlayerCount++;
    }

    public void CheckForDeadPlayerFlagSpawnThreshold()
    {
        float currentSpawnedPlayerCount = GetCurrentSpawnedPlayerCount();
        if (currentSpawnedPlayerCount / initiallySpawnedPlayerCount <= 0.4f)
        {
            GetCheckFlagRemovalTimer(Mission.CurrentTime, GetBattleClient().FlagManipulationTime).Reset(Mission.CurrentTime, 0);
        }
    }

    public override FlagCapturePoint GetRandomFlag()
    {
        var uncapturedFlags = GetAllFlags().Where(f => GetFlagOwner(f) == null).ToArray();
        return uncapturedFlags.GetRandomElement();
    }

    public override void ResetFlags()
    {
        base.ResetFlags();
        initiallySpawnedPlayerCount = 0;
    }

    protected override bool IsAgentCountingAroundFlag(Agent agent) => !agent.IsActive() || !agent.IsHuman || agent.HasMount;

    protected override void ResetFlag(FlagCapturePoint flag) => flag.RemovePointAsServer();

    private int SpawnFlag(FlagCapturePoint flag)
    {
        flag.ResetPointAsServer(TeammateColorsExtensions.NEUTRAL_COLOR, TeammateColorsExtensions.NEUTRAL_COLOR2);
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage(new FlagDominationCapturePointMessage(flag.FlagIndex, null));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
        return flag.FlagIndex;
    }

    private int GetCurrentSpawnedPlayerCount()
    {
        int attackerCount = Mission.AttackerTeam.ActiveAgents.Count;
        int defenderCount = Mission.DefenderTeam.ActiveAgents.Count;

        return attackerCount + defenderCount;
    }
}
