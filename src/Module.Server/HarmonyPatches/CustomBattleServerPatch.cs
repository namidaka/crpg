using Messages.FromCustomBattleServerManager.ToCustomBattleServer;

namespace Crpg.Module.HarmonyPatches;

public class CustomBattleServerPatch
{
    public static bool Prefix(ClientWantsToConnectCustomGameMessage message)
    {
        /*
        var cachedFirewallRule = CrpgSubModule.Instance.GetCachedFirewallRule();
        if (cachedFirewallRule == null)
        {
            Debug.Print("cached Firewall Rule was Null");
            return true;
        }

        /*  *
            * First iterate the connecting players data, get their ip addresses.
            * Check if the ip address is not 0.0.0.0 (If we don't check this and add it to firewall, firewall basically allows anyone)
            * Add the ip addresses to whitelisted ips
            * Apply it to firewall rule
            * 
        foreach (PlayerJoinGameData playerData in message.PlayerJoinGameData)
        {
            if (playerData.IpAddress == "0.0.0.0")
            {
                continue;
            }

            SingleIP firewallIp = SingleIP.Parse(playerData.IpAddress);
            CrpgSubModule.Instance.WhitelistedIps[playerData.PlayerId] = firewallIp;
            Debug.Print("[Firewall] " + playerData.IpAddress + " added to whitelisted ip address", 0, Debug.DebugColor.Green);
        }

        cachedFirewallRule.RemoteAddresses = CrpgSubModule.Instance.WhitelistedIps.Values.ToArray();*/
        return true;
    }
}
