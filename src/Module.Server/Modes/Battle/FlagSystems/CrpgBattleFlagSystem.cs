using Crpg.Module.Helpers;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Objects;
using Timer = TaleWorlds.Core.Timer;

namespace Crpg.Module.Modes.Battle.FlagSystems;
internal class CrpgBattleFlagSystem : AbstractFlagSystem
{
    public CrpgBattleFlagSystem(Mission mission, MultiplayerGameNotificationsComponent notificationsComponent, CrpgBattleClient battleClient)
        : base(mission, notificationsComponent, battleClient)
    {
    }

    public override void CheckForManipulationOfFlags()
    {
        if (HasFlagCountChanged())
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

        SetHasFlagCountChanged(true);

        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage(new CrpgBattleSpawnFlagMessage
        {
            FlagChar = randomFlag.FlagChar,
        });
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);

        GetBattleClient().ChangeNumberOfFlags();
        Debug.Print("Random Flag has been spawned");
    }

    public void CheckForDeadPlayerFlagSpawnThreshold()
    {
        float attackerCount = Mission.AttackerTeam.ActiveAgents.Count;
        float defenderCount = Mission.DefenderTeam.ActiveAgents.Count;
        // TODO: Create a key for it in server configuration
        float overpowerThreshold = 2f;

        if (attackerCount == 1 || defenderCount == 1)
        {
            ResetFlagSpawnTimer();
        }

        if (Math.Min(attackerCount, defenderCount) > 7)
        {
            return;
        }

        if (MathHelper.Within(attackerCount / defenderCount, 1 / overpowerThreshold, overpowerThreshold))
        {
            return;
        }

        ResetFlagSpawnTimer();
    }

    public override FlagCapturePoint GetRandomFlag()
    {
        var uncapturedFlags = GetAllFlags().Where(f => GetFlagOwner(f) == null).ToArray();
        return uncapturedFlags.GetRandomElement();
    }

    protected override bool CanAgentCaptureFlag(Agent agent) => !agent.IsActive() || !agent.IsHuman || agent.HasMount;

    protected override void ResetFlag(FlagCapturePoint flag) => flag.RemovePointAsServer();

    private void ResetFlagSpawnTimer()
    {
        GetCheckFlagRemovalTimer(Mission.CurrentTime, GetBattleClient().FlagManipulationTime).Reset(Mission.CurrentTime, 0);
    }

    private void SpawnFlag(FlagCapturePoint flag)
    {
        flag.ResetPointAsServer(TeammateColorsExtensions.NEUTRAL_COLOR, TeammateColorsExtensions.NEUTRAL_COLOR2);
    }
}
