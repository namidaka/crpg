namespace LauncherV3.LauncherHelper;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Media;
using static LauncherV3.MainViewModel;

internal static class Config
{
    public static Dictionary<Platform, GameInstallationFolderResolver.GameInstallationInfo?> GameLocations { get; private set; } = new() ;
    public static bool DevMode { get; set; }
    public static Platform LastPlatform { get; set; }

    public static bool WriteConfig(string folderPath, string fileName)
    {
        var configData = new ConfigData
        {
            GameLocations = GameLocations,
            DevMode = DevMode,
            LastPlatform = LastPlatform,
        };

        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(configData, options);
        try
        {
            File.WriteAllText(Path.Combine(folderPath, fileName), jsonString);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public static bool ReadConfig(string folderPath, string fileName)
    {
        if (!File.Exists(Path.Combine(folderPath, fileName)))
        {
            CrpgHashMethods.WriteToConsole("No config file found");
            return false;
        }

        string jsonString;
        ConfigData? configData = null;
        try
        {
            jsonString = File.ReadAllText(Path.Combine(folderPath, fileName));
            configData = JsonSerializer.Deserialize<ConfigData>(jsonString);
        }
        catch (Exception ex)
        {
            MainWindow.Instance?.WriteToConsole("Incorrect config file");
            MainWindow.Instance?.WriteToConsole(ex.Message);
        }

        if (configData != null)
        {
            Config.GameLocations = configData.GameLocations;
            DevMode = configData.DevMode;
            LastPlatform = configData.LastPlatform;
        }

        return true;
    }

    public static void ClearLocations()
    {
        GameLocations = new();
    }

    public class ConfigData
    {
        public Dictionary<Platform, GameInstallationFolderResolver.GameInstallationInfo?> GameLocations { get; set; } = new();
        public bool DevMode { get; set; }
        public Platform LastPlatform { get; set; }
    }


}
