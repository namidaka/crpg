using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
    private static readonly string[] CommanderSuicideStrings =
    {
        "{=}The Commander, {COMMANDER} has died!",
        "{=}Commander {COMMANDER} managed to kill themself... somehow.",
        "{=}Commander {COMMANDER}, died spectacularly.",
        "{=}Commander {COMMANDER} has fallen! ",
        "{=}Commander {COMMANDER} didn't stand a chance!",
    };

    private static readonly string[] CommanderKilledStrings =
    {
        "{=}The Commander, {COMMANDER} has died!",
        "{=}Commander {COMMANDER} has been killed by {AGENT}!",
        "{=}{AGENT} has killed {COMMANDER}, The Commander!",
        "{=}Commander {COMMANDER} has been vanquished by {AGENT}, a fine display!",
        "{=}Commander {COMMANDER} didn't stand a chance! {AGENT} made sure of that.",
        "{=}{AGENT} defeated Commander {COMMANDER} in fair combat!",
        "{=}{AGENT} has killed Commander {COMMANDER} in the heat of battle!",
    };

    private Dictionary<BattleSideEnum, NetworkCommunicator?> _commanders = new();
    private Dictionary<BattleSideEnum, BasicCharacterObject?> _commanderCharacters = new();

    public event Action<BattleSideEnum> OnCommanderUpdated = default!;

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

    public bool IsPeerCommander(MissionPeer peer)
    {
        NetworkCommunicator? networkCommunicator = peer.GetNetworkPeer();
        if (networkCommunicator != null)
        {
            foreach (KeyValuePair<BattleSideEnum, NetworkCommunicator?> keyValuePair in _commanders)
            {
                if (keyValuePair.Value == networkCommunicator)
                {
                    return true;
                }
            }
        }

        return false;

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

        OnCommanderUpdated?.Invoke(message.Side);
    }

    private void HandleCommanderKilled(CommanderKilled message)
    {
        var killerAgent = Mission.MissionNetworkHelper.GetAgentFromIndex(message.AgentKillerIndex, true);
        var commanderAgent = Mission.MissionNetworkHelper.GetAgentFromIndex(message.AgentCommanderIndex, true);
        BattleSideEnum commanderSide = commanderAgent.MissionPeer.Team.Side;
        BattleSideEnum mySide = GameNetwork.MyPeer.GetComponent<MissionPeer>().Team.Side;

        TextObject textObject;

        if (message.AgentKillerIndex == message.AgentCommanderIndex)
        {
            textObject = new(CommanderSuicideStrings.GetRandomElement(),
            new Dictionary<string, object> { ["COMMANDER"] = commanderAgent?.Name ?? string.Empty });
        }
        else
        {
            textObject = new(CommanderKilledStrings.GetRandomElement(),
            new Dictionary<string, object> { ["AGENT"] = killerAgent?.Name ?? string.Empty, ["COMMANDER"] = commanderAgent?.Name ?? string.Empty });
        }

        InformationManager.DisplayMessage(new InformationMessage
        {
            Information = textObject.ToString(),
            Color = commanderSide == mySide ? new Color(0.90f, 0.25f, 0.25f) : new Color(0.1f, 1f, 0f),
            SoundEventPath = commanderSide == mySide ? "event:/ui/notification/alert" : "event:/ui/mission/arena_victory",
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
