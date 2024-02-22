using Crpg.Module.Scripts;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.FlagMarker.Targets;

public class CrpgPortcullisMarkerTargetVm : MissionMarkerTargetVM
{
    private readonly GameEntity _portcullis;
    public readonly BattleSideEnum Side;

    public override Vec3 WorldPosition
    {
        get
        {
            if (!(_portcullis != null))
            {
                return Vec3.One;
            }

            return _portcullis.GlobalPosition;
        }
    }

    protected override float HeightOffset => 2.5f;

    public CrpgPortcullisMarkerTargetVm(CrpgPortcullis portcullis)
        : base(MissionMarkerType.SiegeEngine)
    {
        _portcullis = portcullis.GameEntity;
        Side = BattleSideEnum.Defender;
        RefreshColor((Side == BattleSideEnum.Attacker) ? Mission.Current.AttackerTeam.Color : Mission.Current.DefenderTeam.Color, (Side == BattleSideEnum.Attacker) ? Mission.Current.AttackerTeam.Color2 : Mission.Current.DefenderTeam.Color2);
    }
}
