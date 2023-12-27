using System;
using System.Collections.Generic;
using System.Text;
using Crpg.Module.Common.Network;
using Crpg.Module.Helpers;
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

    public CrpgCommanderBehaviorClient()
    {
        _commanders.Add(BattleSideEnum.Attacker, null);
        _commanders.Add(BattleSideEnum.Defender, null);
        _commanders.Add(BattleSideEnum.None, null);
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
    }

    public NetworkCommunicator? GetCommanderBySide(BattleSideEnum side)
    {
        return _commanders[side];
    }

    private void HandleUpdateCommander(UpdateCommander message)
    {
        _commanders[message.Side] = message.Commander;
        Debug.Print($"Added Commander {message.Commander?.UserName ?? string.Empty} to {message.Side} side.");
    }

    public BasicCharacterObject? GetCommanderCharacterObject(BattleSideEnum side) // save this character object when we get a new commander?
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
