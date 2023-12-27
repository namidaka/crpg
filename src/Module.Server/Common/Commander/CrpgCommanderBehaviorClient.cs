using System;
using System.Collections.Generic;
using System.Text;
using Crpg.Module.Common.Network;
using Crpg.Module.Helpers;
using Crpg.Module.Modes.Dtv;
using TaleWorlds.Core;
using TaleWorlds.Diamond;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace Crpg.Module.Common.Commander;
internal class CrpgCommanderBehaviorClient : MissionNetwork
{
    private Dictionary<BattleSideEnum, NetworkCommunicator?> _commanders = new();
    private Dictionary<BattleSideEnum, BasicCharacterObject?> _commanderCharacters = new();

    public CrpgCommanderBehaviorClient()
    {
        _commanders.Add(BattleSideEnum.Attacker, null);
        _commanders.Add(BattleSideEnum.Defender, null);
        _commanders.Add(BattleSideEnum.None, null);

        _commanderCharacters.Add(BattleSideEnum.Attacker, null);
        _commanderCharacters.Add(BattleSideEnum.Defender, null);
        _commanderCharacters.Add(BattleSideEnum.None, null);
    }

    public override void OnBehaviorInitialize()
    {
        base.OnBehaviorInitialize();
        //RequestCommanderUpdate();
    }

    private void RequestCommanderUpdate()
    {
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage(new RequestCommanderUpdate());
        GameNetwork.EndModuleEventAsClient();
    }

    protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
    {
        base.AddRemoveMessageHandlers(registerer);
        registerer.Register<UpdateCommander>(HandleUpdateCommander);
        registerer.Register<CommanderKilled>(HandleCommanderKilled);
    }

    public NetworkCommunicator? GetCommanderBySide(BattleSideEnum side)
    {
        return _commanders[side];
    }

    public BasicCharacterObject? GetCommanderCharacterObjectBySide(BattleSideEnum side)
    {
        return _commanderCharacters[side];
    }

    private void HandleUpdateCommander(UpdateCommander message)
    {
        _commanders[message.Side] = message.Commander;
        _commanderCharacters[message.Side] = BuildCommanderCharacterObject(message.Side);
        TextObject textObject;

        if (message.Commander != null)
        {
            textObject = new("{=}The {SIDE}s have promoted {COMMANDER} to be their commander!",
            new Dictionary<string, object> { ["SIDE"] = message.Side.ToString(), ["COMMANDER"] = message.Commander.UserName });
        }
        else
        {
            textObject = new("{=}The {SIDE}s' commander has resigned!",
            new Dictionary<string, object> { ["SIDE"] = message.Side.ToString()});
        }

        InformationManager.DisplayMessage(new InformationMessage
        {
            Information = textObject.ToString(),
            Color = new Color(0.1f, 1f, 0f),
            SoundEventPath = "event:/ui/notification/war_declared",
        });
    }

    private void HandleCommanderKilled(CommanderKilled message)
    {
        var killerAgent = Mission.MissionNetworkHelper.GetAgentFromIndex(message.AgentKillerIndex, true);
        var commanderAgent = Mission.MissionNetworkHelper.GetAgentFromIndex(message.AgentCommanderIndex, true);

        TextObject textObject = new("{=}The Commander, {COMMANDER} has been slain in the heat of battle by {AGENT}!",
        new Dictionary<string, object> { ["AGENT"] = killerAgent?.Name ?? string.Empty, ["COMMANDER"] = commanderAgent?.Name ?? string.Empty });
        InformationManager.DisplayMessage(new InformationMessage
        {
            Information = textObject.ToString(),
            Color = new Color(0.90f, 0.25f, 0.25f),
            SoundEventPath = "event:/ui/notification/alert",
        });
    }

    private BasicCharacterObject? BuildCommanderCharacterObject(BattleSideEnum side)
    {
        if (_commanders[side] != null)
        {
            MissionPeer missionPeer = _commanders[side].GetComponent<MissionPeer>();
            BasicCharacterObject commanderCharacterObject = MBObjectManager.Instance.GetObject<BasicCharacterObject>("mp_character");
            commanderCharacterObject.UpdatePlayerCharacterBodyProperties(missionPeer.Peer.BodyProperties, missionPeer.Peer.Race, missionPeer.Peer.IsFemale);
            commanderCharacterObject.Age = missionPeer.Peer.BodyProperties.Age;
            commanderCharacterObject.GetName();

            var crpgUser = missionPeer.Peer.GetComponent<CrpgPeer>()?.User;

            if (crpgUser != null)
            {
                var equipment = CrpgCharacterBuilder.CreateCharacterEquipment(crpgUser.Character.EquippedItems);
                MBEquipmentRoster equipmentRoster = new();
                ReflectionHelper.SetField(equipmentRoster, "_equipments", new MBList<Equipment> { equipment });
                ReflectionHelper.SetField(commanderCharacterObject, "_equipmentRoster", equipmentRoster);
                ReflectionHelper.SetField(commanderCharacterObject, "_basicName", new TextObject(missionPeer.Name));
            }

            return commanderCharacterObject;
        }

        return null;
    }
}
