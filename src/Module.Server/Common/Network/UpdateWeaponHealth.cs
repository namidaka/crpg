using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace Crpg.Module.Common.Network;

[DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
internal sealed class UpdateWeaponHealth : GameNetworkMessage
{
    public Agent Agent { get; set; } = default!;
    public int WeaponHealth { get; set; }
    public int LastRoll { get; set; }
    public int LastBlow { get; set; }
    public EquipmentIndex EquipmentIndex { get; set; }

    protected override void OnWrite()
    {
        WriteAgentReferenceToPacket(Agent);
        WriteIntToPacket(WeaponHealth, CompressionBasic.DebugIntNonCompressionInfo);
        WriteIntToPacket(LastRoll, CompressionBasic.DebugIntNonCompressionInfo);
        WriteIntToPacket(LastBlow, CompressionBasic.DebugIntNonCompressionInfo);
        WriteIntToPacket((int)EquipmentIndex, CompressionMission.ItemSlotCompressionInfo);
    }

    protected override bool OnRead()
    {
        bool bufferReadValid = true;
        Agent = ReadAgentReferenceFromPacket(ref bufferReadValid, true);
        WeaponHealth = ReadIntFromPacket(CompressionBasic.DebugIntNonCompressionInfo, ref bufferReadValid);
        LastRoll = ReadIntFromPacket(CompressionBasic.DebugIntNonCompressionInfo, ref bufferReadValid);
        LastBlow = ReadIntFromPacket(CompressionBasic.DebugIntNonCompressionInfo, ref bufferReadValid);
        EquipmentIndex = (EquipmentIndex)ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref bufferReadValid);
        return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter()
    {
        return MultiplayerMessageFilter.GameMode;
    }

    protected override string OnGetLogFormat()
    {
        return "Update Weapon Health";
    }
}
