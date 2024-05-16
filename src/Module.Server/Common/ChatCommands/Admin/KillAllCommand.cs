using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Crpg.Module.Common.ChatCommands.Admin;

internal class KillAllCommand : AdminCommand
{
    public KillAllCommand(ChatCommandsComponent chatComponent)
        : base(chatComponent)
    {
        Name = "killall";
        Description = $"'{ChatCommandsComponent.CommandPrefix}{Name}' to kill all agents.";
        Overloads = new CommandOverload[]
        {
            new(new[] { ChatCommandParameterType.String }, Execute),
            new(Array.Empty<ChatCommandParameterType>(), Execute),
        };
    }

    private void Execute(NetworkCommunicator fromPeer, object[] arguments)
    {
        var agentsToKill = Mission.Current.Agents.ToList();
        foreach (var agent in agentsToKill)
        {
            if (agent != null)
            {
                DamageHelper.DamageAgent(agent, (int)agent.Health + 2);
            }
        }
    }
}
