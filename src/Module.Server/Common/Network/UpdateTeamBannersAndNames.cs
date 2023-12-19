using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace Crpg.Module.Common.Network;

[DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
internal sealed class UpdateTeamBannersAndNames : GameNetworkMessage
{
    public BannerCode AttackerBanner { get; set; } = BannerCode.CreateFrom(string.Empty);
    public BannerCode DefenderBanner { get; set; } = BannerCode.CreateFrom(string.Empty);
    public string AttackerName { get; set; } = string.Empty;
    public string DefenderName { get; set; } = string.Empty;

    protected override void OnWrite()
    {
        WriteBannerCodeToPacket(AttackerBanner.Code);
        WriteBannerCodeToPacket(DefenderBanner.Code);
        WriteStringToPacket(AttackerName);
        WriteStringToPacket(DefenderName);
    }

    protected override bool OnRead()
    {
        bool bufferReadValid = true;
        AttackerBanner = BannerCode.CreateFrom(ReadBannerCodeFromPacket(ref bufferReadValid));
        DefenderBanner = BannerCode.CreateFrom(ReadBannerCodeFromPacket(ref bufferReadValid));
        AttackerName = ReadStringFromPacket(ref bufferReadValid);
        DefenderName = ReadStringFromPacket(ref bufferReadValid);
        return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter()
    {
        return MultiplayerMessageFilter.GameMode;
    }

    protected override string OnGetLogFormat()
    {
        return "Update Team Banner And Names";
    }
}
