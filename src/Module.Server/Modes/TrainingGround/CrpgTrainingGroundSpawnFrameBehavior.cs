using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Crpg.Module.Modes.TrainingGround;
internal class CrpgTrainingGroundSpawnFrameBehavior : SpawnFrameBehaviorBase
{
    public override void Initialize()
    {
        base.Initialize();
    }

    public override MatrixFrame GetSpawnFrame(Team team, bool hasMount, bool isInitialSpawn)
    {
        List<GameEntity> list = SpawnPoints.ToList();
        return GetSpawnFrameFromSpawnPoints(list, team, hasMount);
    }
}
