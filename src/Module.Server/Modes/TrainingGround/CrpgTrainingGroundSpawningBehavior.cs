using Crpg.Module.Common;
using TaleWorlds.MountAndBlade;

namespace Crpg.Module.Modes.TrainingGround;

internal class CrpgTrainingGroundSpawningBehavior : CrpgSpawningBehaviorBase
{
    private readonly CrpgTrainingGroundServer _server;

    public CrpgTrainingGroundSpawningBehavior(CrpgConstants constants, CrpgTrainingGroundServer server)
        : base(constants)
    {
        IsSpawningEnabled = true;
        _server = server;
    }

    public override void OnTick(float dt)
    {
        if (IsSpawningEnabled && _spawnCheckTimer.Check(Mission.CurrentTime))
        {
            SpawnAgents();
        }

        base.OnTick(dt);
    }

    protected override bool IsPlayerAllowedToSpawn(NetworkCommunicator networkPeer)
    {
        MissionPeer missionPeer = networkPeer.GetComponent<MissionPeer>();
        return missionPeer.Culture != null
               && missionPeer.Representative is CrpgTrainingGroundMissionRepresentative
               && missionPeer.SpawnTimer.Check(Mission.CurrentTime);
    }

    protected override bool IsRoundInProgress()
    {
        return Mission.CurrentState == Mission.State.Continuing;
    }

    protected override void OnPeerSpawned(Agent agent)
    {
        base.OnPeerSpawned(agent);
        _ = agent.MissionPeer.Representative; // Get initializes the representative

        var networkPeer = agent.MissionPeer?.GetNetworkPeer();
        if (networkPeer == null)
        {
            return;
        }

    }
}
