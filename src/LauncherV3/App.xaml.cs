
namespace LauncherV3;

using System.Configuration;
using System.Data;
using System.Globalization;
using System.Windows;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        // Set culture to invariant
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

        // Alternatively, set culture to the system's default culture
        // CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CurrentCulture;
        // CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CurrentUICulture;

        base.OnStartup(e);
    }
}

