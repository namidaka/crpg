using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Crpg.Module.Modes.TrainingGround;
internal class CrpgTrainingGroundSpawnFrameBehavior : SpawnFrameBehaviorBase
{
    private const string AreaSpawnPointTagExpression = "spawnpoint_area(_\\d+)*";

    private const string AreaSpawnPointTagPrefix = "spawnpoint_area_";

    private List<GameEntity>[] _duelAreaSpawnPoints = default!;

    private bool[] _spawnPointSelectors = default!;

    public override void Initialize()
    {
        base.Initialize();
        _duelAreaSpawnPoints = new List<GameEntity>[16];
        _spawnPointSelectors = new bool[16];
        foreach (GameEntity item in Mission.Current.Scene.FindEntitiesWithTagExpression("spawnpoint_area(_\\d+)*"))
        {
            int num = int.Parse(item.Tags.Single((string tag) => tag.StartsWith("spawnpoint_area_")).Replace("spawnpoint_area_", "")) - 1;
            if (_duelAreaSpawnPoints[num] == null)
            {
                _duelAreaSpawnPoints[num] = new List<GameEntity>();
            }

            _duelAreaSpawnPoints[num].Add(item);
        }
    }

    public override MatrixFrame GetSpawnFrame(Team team, bool hasMount, bool isInitialSpawn)
    {
        int duelAreaIndexIfDuelTeam = 0;
        List<GameEntity> list = (duelAreaIndexIfDuelTeam >= 0) ? _duelAreaSpawnPoints[duelAreaIndexIfDuelTeam].ToList() : SpawnPoints.ToList();
        if (duelAreaIndexIfDuelTeam >= 0)
        {
            list.RemoveAt((!_spawnPointSelectors[duelAreaIndexIfDuelTeam]) ? 1 : 0);
            _spawnPointSelectors[duelAreaIndexIfDuelTeam] = !_spawnPointSelectors[duelAreaIndexIfDuelTeam];
        }

        return GetSpawnFrameFromSpawnPoints(list, team, hasMount);
    }
}
