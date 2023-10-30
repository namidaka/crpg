using System.Net;
using TaleWorlds.Library;
using TaleWorlds.PlayerServices;
using WindowsFirewallHelper;

namespace Crpg.Module;
public static class Firewall
{
    public static string GetFirewallRuleName(int port)
    {
        return "Firewall " + port.ToString();
    }

    public static IFirewallRule GetFirewallRule(int port, IFirewallRule? cachedFirewallRule)
    {
        if (cachedFirewallRule == null)
        {
            cachedFirewallRule = FirewallManager.Instance.Rules.SingleOrDefault(r => r.Name == GetFirewallRuleName(port));
        }

        return cachedFirewallRule;
    }

    public static IFirewallRule? CreateFirewallRule(int port)
    {
        IFirewallRule? firewallRule = FirewallManager.Instance.CreatePortRule(
        FirewallProfiles.Domain | FirewallProfiles.Private | FirewallProfiles.Public,
        GetFirewallRuleName(port),
        FirewallAction.Allow,
        Convert.ToUInt16(port), FirewallProtocol.UDP);
        firewallRule.IsEnable = true;
        firewallRule.Direction = FirewallDirection.Inbound;
        FirewallManager.Instance.Rules.Add(firewallRule);
        Debug.Print("[Firewall] FirewallRule " + GetFirewallRuleName(port) + " is created for your bannerlord server.", 0, Debug.DebugColor.Green);
        return firewallRule;
    }
}
