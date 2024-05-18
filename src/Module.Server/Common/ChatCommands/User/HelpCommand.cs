using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Crpg.Module.Common.ChatCommands.User;

internal class HelpCommand : ChatCommand
{
    public HelpCommand(ChatCommandsComponent chatComponent)
        : base(chatComponent)
    {
        Name = "h";
        Overloads = new CommandOverload[]
        {
            new(new ChatCommandParameterType[] { ChatCommandParameterType.String }, ExecuteSuccess),
        };
        Description = $"!{Name} <message> - Send a help request to the admins.";
    }

    private static readonly Color ColorAdmin = new(1f, 0f, 0f);

    private void ExecuteSuccess(NetworkCommunicator fromPeer, object[] arguments)
    {
        var crpgUser = fromPeer.GetComponent<CrpgPeer>()?.User;
        if (crpgUser == null)
        {
            ChatComponent.ServerSendMessageToPlayer(fromPeer, ColorFatal, "Unable to identify your user information.");
            return;
        }

        string fullMessage = arguments.Length > 0 ? string.Join(" ", arguments) : "Help request sent with no additional message.";
        string userMessage = $"[HELP REQUEST] Message from {fromPeer.UserName}: {fullMessage}";
        ChatComponent.ServerSendMessageToAdmins(ColorAdmin, userMessage);
        ChatComponent.ServerSendMessageToPlayer(fromPeer, ColorSuccess, "Admins have received your message!");
    }
}
