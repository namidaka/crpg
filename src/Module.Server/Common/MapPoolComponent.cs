using Crpg.Module.Helpers;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.DedicatedCustomServer;
using TaleWorlds.MountAndBlade.ListedServer;
using TaleWorlds.MountAndBlade.Multiplayer.NetworkComponents;

namespace Crpg.Module.Common;

/// <summary>
/// If voting is enabled, allow voting for the X next maps of the pool; otherwise shuffle once the pool and picks the maps sequentially.
/// </summary>
/// <remarks>
/// I could not find a way to branch to game start/end so I branched to <see cref="OnEndMission"/>. It means that the
/// intermission screen when the game start, will show all maps. It's ok since usually nobody is connected when the
/// server starts.
/// </remarks>
internal class MapPoolComponent : MissionLogic
{
    private static int nextMapId;
    private string _nextMode = string.Empty;
    private string? _forcedNextMap;

    public void ForceNextMap(string map)
    {
        if (ListedServerCommandManager.ServerSideIntermissionManager.AutomatedMapPool.Contains(map))
        {
            return;
        }

        _forcedNextMap = map;
    }

    protected override void OnEndMission()
    {
        if (CrpgServerConfiguration.ShuffleGameMode)
        {
            _nextMode = MultiplayerOptions.OptionType.GameType.GetStrValue() == "cRPGBattle" ? "cRPGTeamDeathmatch" : "cRPGBattle";
            CrpgGamemodeManager.LoadGameConfig(_nextMode);
            CrpgGamemodeManager.MapCounter[_nextMode] = (CrpgGamemodeManager.MapCounter[_nextMode] + 1) % CrpgGamemodeManager.Maps[_nextMode].Count;
            string nextMap = _forcedNextMap ?? CrpgGamemodeManager.Maps[_nextMode][CrpgGamemodeManager.MapCounter[_nextMode]];

            MultiplayerOptions.OptionType.Map.SetValue(nextMap, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
            Environment.SetEnvironmentVariable("CRPG_INSTANCE", CrpgGamemodeManager.Modes[_nextMode][0].ToString());
            _forcedNextMap = null;
        }
        else
        {
            nextMapId = (nextMapId + 1) % ListedServerCommandManager.ServerSideIntermissionManager.AutomatedMapPool.Count;
            string nextMap = _forcedNextMap ?? ListedServerCommandManager.ServerSideIntermissionManager.AutomatedMapPool[nextMapId];
            MultiplayerOptions.OptionType.Map.SetValue(nextMap, MultiplayerOptions.MultiplayerOptionsAccessMode.NextMapOptions);
            _forcedNextMap = null;
        }
    }
}
