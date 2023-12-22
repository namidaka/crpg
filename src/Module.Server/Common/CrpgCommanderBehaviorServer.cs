using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Crpg.Module.Common;
internal class CrpgCommanderBehaviorServer : MissionBehavior
{
    public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

    private class Command
    {
        private Agent? _commander;
        public BattleSideEnum Side { get; }
        public Agent? Commander
        {
            get => _commander ?? null;
            set
            {
                if (value != _commander)
                {
                    _commander = value;
                }
            }
        }

        public Command(BattleSideEnum side)
        {
            Side = side;
        }

    }
}
