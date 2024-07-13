using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;

namespace Crpg.Module.Common;
internal static class CrpgMapManager
{
    public static readonly Dictionary<string, string> Modes = new()
    {
        { "cRPGBattle", "a_maps.txt" },
        { "cRPGConquest", "b_maps.txt" },
        { "cRPGTeamDeathmatch", "f_maps.txt" },
    };

    public static Dictionary<string, List<string>> Maps { get; private set; } = new()
    {
        { "cRPGBattle", new() },
        { "cRPGConquest", new() },
        { "cRPGTeamDeathmatch", new() },
    };

    public static Dictionary<string, int> MapCounter { get; set; } = new()
    {
        { "cRPGBattle", 0 },
        { "cRPGConquest", 0 },
        { "cRPGTeamDeathmatch", 0 },
    };

    private static readonly Random Random = new();

    public static void AddMaps()
    {
        foreach (KeyValuePair<string, string> mode in Modes)
        {
            string mapconfigfilepath = Path.Combine(Directory.GetCurrentDirectory(), ModuleHelper.GetModuleFullPath("cRPG"), mode.Value);
            if (File.Exists(mapconfigfilepath))
            {
                try
                {
                    string[] maps = File.ReadAllLines(mapconfigfilepath);

                    int startIndex = Random.Next(maps.Length); // Random start index between 0 and maps.Length - 1
                    for (int i = 0; i < maps.Length; i++)
                    {
                        int currentIndex = (startIndex + i) % maps.Length;
                        string map = maps[currentIndex];

                        if (map == string.Empty)
                        {
                            continue;
                        }

                        Maps[mode.Key].Add(map);
                        Debug.Print($"added {map} to {mode.Key} map pool", color: Debug.DebugColor.Red);
                    }
                }
                catch (Exception e)
                {
                    Debug.Print($"could not read the map file {mapconfigfilepath}", color: Debug.DebugColor.Red);
                    Debug.Print($"{e.Message}", color: Debug.DebugColor.Red);
                }
            }
            else
            {
                Debug.Print("No separate map file");
            }

            }

        return;
    }
}
