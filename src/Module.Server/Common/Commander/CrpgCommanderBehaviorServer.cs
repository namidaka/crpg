using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Crpg.Module.Common;
internal class CrpgCommanderBehaviorServer : MissionBehavior
{
    public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;
    private List<Command> _commands = new();

    public void CreateCommand(NetworkCommunicator commander)
    {
        Command command = new(commander.ControlledAgent.Team.Side, commander);
        _commands.Add(command);
    }

    public Command? GetCommandBySide(BattleSideEnum side)
    {
        return _commands.FirstOrDefault(c => c.Side == side) ?? null;
    }

    public Command? GetCommandByNetworkCommunicator(NetworkCommunicator networkPeer)
    {
        return _commands.FirstOrDefault(c => c.Commander == networkPeer) ?? null;
    }

    public class Command
    {
        private NetworkCommunicator _commander;
        public BattleSideEnum Side { get; }
        public NetworkCommunicator Commander
        {
            get => _commander;
            set
            {
                if (value != _commander)
                {
                    _commander = value;
                }
            }
        }

        public Command(BattleSideEnum side, NetworkCommunicator commander)
        {
            Side = side;
            _commander = commander;
        }
    }
}
