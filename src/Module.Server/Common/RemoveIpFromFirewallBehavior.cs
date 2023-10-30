using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using WindowsFirewallHelper;

namespace Crpg.Module.Common;

public class RemoveIpFromFirewallBehavior : MissionNetwork
{

    public override void OnBehaviorInitialize()
    {
        base.OnBehaviorInitialize();
        Debug.Print("Firewall RemoveIpBehavior has been initialized.", 0, Debug.DebugColor.Purple);
    }

    public override void OnPlayerDisconnectedFromServer(NetworkCommunicator networkPeer)
    {
        var cachedFirewallRule = CrpgSubModule.Instance.GetCachedFirewallRule();
        if (cachedFirewallRule == null)
        {
            return;
        }

        if (CrpgSubModule.Instance.WhitelistedIps.ContainsKey(networkPeer.PlayerConnectionInfo.PlayerID))
        {
            CrpgSubModule.Instance.WhitelistedIps.Remove(networkPeer.PlayerConnectionInfo.PlayerID);
            IAddress[] addresses = CrpgSubModule.Instance.WhitelistedIps.Values.ToArray();
            Debug.Print("[BannerlordFirewall] " + networkPeer.UserName + " was removed from the firewall whitelist, whitelisted ip count: " + addresses.Length.ToString(), 0, Debug.DebugColor.Red);
            Firewall.GetFirewallRule(CrpgSubModule.Instance.Port(), cachedFirewallRule).RemoteAddresses = addresses;
        }
    }
}
