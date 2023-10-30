using Messages.FromCustomBattleServerManager.ToCustomBattleServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;
using WindowsFirewallHelper.Addresses;
using Crpg.Module;
using HarmonyLib;

namespace Crpg.Module.HarmonyPatches;

public class CustomBattleServerPatch
{
    public static bool Prefix(ClientWantsToConnectCustomGameMessage message)
    {
        var cachedFirewallRule = CrpgSubModule.Instance.GetCachedFirewallRule();
        if (cachedFirewallRule == null)
            return true;
        /**
            * First iterate the connecting players data, get their ip addresses.
            * Check if the ip address is not 0.0.0.0 (If we don't check this and add it to firewall, firewall basically allows anyone)
            * Add the ip addresses to whitelisted ips
            * Apply it to firewall rule
            * */
        foreach (PlayerJoinGameData playerData in message.PlayerJoinGameData)
        {
            if (playerData.IpAddress == "0.0.0.0")
                continue;
            SingleIP firewallIp = SingleIP.Parse(playerData.IpAddress);
            CrpgSubModule.Instance.WhitelistedIps[playerData.PlayerId] = firewallIp;
            Debug.Print("[BannerlordFirewall] " + playerData.IpAddress + " added to whitelisted ip address", 0, Debug.DebugColor.Green);
        }

        Firewall.GetFirewallRule(CrpgSubModule.Instance.Port(), cachedFirewallRule).RemoteAddresses = CrpgSubModule.Instance.WhitelistedIps.Values.ToArray();
        return true;
    }
}
