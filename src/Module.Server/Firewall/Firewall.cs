using System.Net;
using TaleWorlds.Library;
using TaleWorlds.PlayerServices;
using WindowsFirewallHelper;
namespace Crpg.Module.Firewall;
public class Firewall
{
    public Dictionary<PlayerId, IAddress> WhitelistedIps = new();

    private IFirewallRule? _cachedFirewallRule;
    public string GetFirewallRuleName(int port)
    {
        return "Bannerlord Firewall " + port.ToString();
    }

    public IFirewallRule GetFirewallRule(int port)
    {
        if (_cachedFirewallRule == null)
        {
            _cachedFirewallRule = FirewallManager.Instance.Rules.SingleOrDefault(r => r.Name == GetFirewallRuleName(port));
        }

        return _cachedFirewallRule;
    }

    public void CreateFirewallRule(int port)
    {
        _cachedFirewallRule = FirewallManager.Instance.CreatePortRule(
        FirewallProfiles.Domain | FirewallProfiles.Private | FirewallProfiles.Public,
        GetFirewallRuleName(port),
        FirewallAction.Allow,
        Convert.ToUInt16(port), FirewallProtocol.UDP);
        _cachedFirewallRule.IsEnable = true;
        _cachedFirewallRule.Direction = FirewallDirection.Inbound;
        FirewallManager.Instance.Rules.Add(_cachedFirewallRule);
        Debug.Print("[BannerlordFirewall] FirewallRule " + GetFirewallRuleName(port) + " is created for your bannerlord server.", 0, Debug.DebugColor.Green);
    }
}
