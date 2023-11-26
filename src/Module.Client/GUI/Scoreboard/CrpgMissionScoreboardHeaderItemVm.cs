#region assembly TaleWorlds.MountAndBlade.ViewModelCollection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.ViewModelCollection.dll
// Decompiled with ICSharpCode.Decompiler 7.1.0.6543
#endregion

using Crpg.Module.Gui;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Scoreboard
{
    public class CrpgMissionScoreboardHeaderItemVM : BindingListStringItem
    {
        private readonly CrpgScoreboardSideVM _side;

        private string _headerID = string.Empty;

        private bool _isIrregularStat;

        private bool _isAvatarStat;

        [DataSourceProperty]
        public string HeaderID
        {
            get
            {
                return _headerID;
            }
            set
            {
                if (value != _headerID)
                {
                    _headerID = value;
                    OnPropertyChangedWithValue(value, "HeaderID");
                }
            }
        }

        [DataSourceProperty]
        public bool IsIrregularStat
        {
            get
            {
                return _isIrregularStat;
            }
            set
            {
                if (value != _isIrregularStat)
                {
                    _isIrregularStat = value;
                    OnPropertyChangedWithValue(value, "IsIrregularStat");
                }
            }
        }

        [DataSourceProperty]
        public bool IsAvatarStat
        {
            get
            {
                return _isAvatarStat;
            }
            set
            {
                if (value != _isAvatarStat)
                {
                    _isAvatarStat = value;
                    OnPropertyChangedWithValue(value, "IsAvatarStat");
                }
            }
        }

        [DataSourceProperty]
        public MissionScoreboardPlayerSortControllerVM PlayerSortController => _side.PlayerSortController;

        public CrpgMissionScoreboardHeaderItemVM(CrpgScoreboardSideVM side, string headerID, string value, bool isAvatarStat, bool isIrregularStat)
            : base(value)
        {
            _side = side;
            HeaderID = headerID;
            IsAvatarStat = isAvatarStat;
            IsIrregularStat = isIrregularStat;
        }
    }
}
