#region assembly TaleWorlds.MountAndBlade.ViewModelCollection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.ViewModelCollection.dll
// Decompiled with ICSharpCode.Decompiler 7.1.0.6543
#endregion

using System.Drawing;
using Crpg.Module.Common;
using TaleWorlds.Avatar.PlayerServices;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.MissionRepresentatives;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Armory;

namespace Crpg.Module.GUI.HudExtension
{
    public class CrpgPlayerVM : ViewModel
    {
        private MultiplayerClassDivisions.MPHeroClass? _cachedClass = default!;

        private BasicCultureObject _cachedCulture = default!;

        private readonly MissionMultiplayerGameModeBaseClient _gameMode = default!;

        private readonly MissionRepresentativeBase _missionRepresentative = default!;

        private readonly bool _isInParty;

        private readonly bool _isKnownPlayer;

        private TextObject _genericPlayerName = new TextObject("{=RN6zHak0}Player");

        private const uint _focusedContourColor = 4278255612u;

        private const uint _defaultContourColor = 0u;

        private const uint _invalidColor = 0u;

        private int _gold = default!;

        private int _valuePercent;

        private string _name = default!;

        private string _cultureID = default!;

        private bool _isDead;

        private bool _isValueEnabled;

        private bool _hasSetCompassElement;

        private bool _isSpawnActive;

        private bool _isFocused;

        private MPTeammateCompassTargetVM _compassElement = default!;

        private ImageIdentifierVM _avatar = new ImageIdentifierVM();

        private MPArmoryHeroPreviewVM _preview = default!;

        private MBBindingList<MPPerkVM> _activePerks = default!;

        public MissionPeer Peer { get; private set; } = default!;

        private Team? _playerTeam
        {
            get
            {
                if (!GameNetwork.IsMyPeerReady)
                {
                    return null;
                }

                var component = GameNetwork.MyPeer.GetComponent<MissionPeer>();
                if (component.Team == null || component.Team.Side == BattleSideEnum.None)
                {
                    return null;
                }

                return component.Team;
            }
        }

        [DataSourceProperty]
        public int Gold
        {
            get
            {
                return _gold;
            }
            set
            {
                if (value != _gold)
                {
                    _gold = value;
                    OnPropertyChangedWithValue(value, "Gold");
                }
            }
        }

        [DataSourceProperty]
        public int ValuePercent
        {
            get
            {
                return _valuePercent;
            }
            set
            {
                if (value != _valuePercent)
                {
                    _valuePercent = value;
                    OnPropertyChangedWithValue(value, "ValuePercent");
                }
            }
        }

        [DataSourceProperty]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    OnPropertyChangedWithValue(value, "Name");
                }
            }
        }

        [DataSourceProperty]
        public string CultureID
        {
            get
            {
                return _cultureID;
            }
            set
            {
                if (value != _cultureID)
                {
                    _cultureID = value;
                    OnPropertyChangedWithValue(value, "CultureID");
                }
            }
        }

        [DataSourceProperty]
        public bool IsDead
        {
            get
            {
                return _isDead;
            }
            set
            {
                if (value != _isDead)
                {
                    _isDead = value;
                    OnPropertyChangedWithValue(value, "IsDead");
                }
            }
        }

        [DataSourceProperty]
        public bool IsValueEnabled
        {
            get
            {
                return _isValueEnabled;
            }
            set
            {
                if (value != _isValueEnabled)
                {
                    _isValueEnabled = value;
                    OnPropertyChangedWithValue(value, "IsValueEnabled");
                }
            }
        }

        [DataSourceProperty]
        public bool HasSetCompassElement
        {
            get
            {
                return _hasSetCompassElement;
            }
            set
            {
                if (value != _hasSetCompassElement)
                {
                    _hasSetCompassElement = value;
                    OnPropertyChangedWithValue(value, "HasSetCompassElement");
                }
            }
        }

        [DataSourceProperty]
        public bool IsSpawnActive
        {
            get
            {
                return _isSpawnActive;
            }
            set
            {
                if (value != _isSpawnActive)
                {
                    _isSpawnActive = value;
                    OnPropertyChangedWithValue(value, "IsSpawnActive");
                }
            }
        }

        [DataSourceProperty]
        public bool IsFocused
        {
            get
            {
                return _isFocused;
            }
            set
            {
                if (value != _isFocused)
                {
                    _isFocused = value;
                    OnPropertyChangedWithValue(value, "IsFocused");
                }
            }
        }

        [DataSourceProperty]
        public MPTeammateCompassTargetVM CompassElement
        {
            get
            {
                return _compassElement;
            }
            set
            {
                if (value != _compassElement)
                {
                    _compassElement = value;
                    OnPropertyChangedWithValue(value, "CompassElement");
                }
            }
        }

        [DataSourceProperty]
        public ImageIdentifierVM Avatar
        {
            get
            {
                return _avatar;
            }
            set
            {
                if (value != _avatar)
                {
                    _avatar = value;
                    OnPropertyChangedWithValue(value, "Avatar");
                }
            }
        }

        [DataSourceProperty]
        public MPArmoryHeroPreviewVM Preview
        {
            get
            {
                return _preview;
            }
            set
            {
                if (value != _preview)
                {
                    _preview = value;
                    OnPropertyChangedWithValue(value, "Preview");
                }
            }
        }

        [DataSourceProperty]
        public MBBindingList<MPPerkVM> ActivePerks
        {
            get
            {
                return _activePerks;
            }
            set
            {
                if (value != _activePerks)
                {
                    _activePerks = value;
                    OnPropertyChangedWithValue(value, "ActivePerks");
                }
            }
        }

        public CrpgPlayerVM(Agent agent)
        {
            if (agent != null)
            {
                var iconType = MultiplayerClassDivisions.GetMPHeroClassForCharacter(agent.Character)?.IconType ?? TargetIconType.None;
                uint num = agent.Team?.Color ?? 0;
                uint color = agent.Team?.Color2 ?? 0;
                BannerCode bannercode = BannerCode.CreateFrom(new Banner(agent.Team?.Banner.Serialize(), num, color));
                CompassElement = new MPTeammateCompassTargetVM(iconType, num, color, bannercode, isAlly: false);
            }
            else
            {
                CompassElement = new MPTeammateCompassTargetVM(TargetIconType.Monster, 0u, 0u, BannerCode.CreateFrom(Banner.CreateOneColoredEmptyBanner(0)), isAlly: false);
            }
        }

        public CrpgPlayerVM(MissionPeer peer)
        {
            Peer = peer;
            _gameMode = Mission.Current.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
            _missionRepresentative = peer.GetComponent<MissionRepresentativeBase>();
            if (Peer == null)
            {
                CompassElement = new MPTeammateCompassTargetVM(TargetIconType.None, 0u, 0u, null, isAlly: false);
                return;
            }

            _isInParty = NetworkMain.GameClient.IsInParty;
            _isKnownPlayer = NetworkMain.GameClient.IsKnownPlayer(Peer.Peer.Id);
            RefreshAvatar();
            Name = peer.DisplayedName;
            ActivePerks = new MBBindingList<MPPerkVM>();
            RefreshValues();
        }

        public void UpdateDisabled()
        {
            IsDead = !Peer.IsControlledAgentActive;
        }

        public void RefreshDivision(bool useCultureColors = false)
        {
            if (Peer == null || Peer.Culture == null)
            {
                return;
            }

            var mPHeroClassForPeer = MultiplayerClassDivisions.GetMPHeroClassForPeer(Peer);
            var targetIconType = mPHeroClassForPeer?.IconType ?? TargetIconType.None;
            if (_cachedClass == null || _cachedClass != mPHeroClassForPeer || _cachedCulture == null || _cachedCulture != Peer.Culture)
            {
                _cachedClass = mPHeroClassForPeer;
                _cachedCulture = Peer.Culture;
                uint color1 = 0;
                uint color2 = 0;
                var crpgPeer = Peer.GetComponent<CrpgPeer>();
                if (crpgPeer != null && crpgPeer.Clan != null)
                {
                    color1 = crpgPeer.Clan.PrimaryColor;
                    color2 = crpgPeer.Clan.SecondaryColor;
                }

                BannerCode bannercode = BannerCode.CreateFrom(new Banner(Peer.Peer.BannerCode, 0, 0));
                CompassElement = new MPTeammateCompassTargetVM(targetIconType, 0, 0, bannercode, Peer.Team?.IsPlayerAlly ?? false);
                HasSetCompassElement = true;
                Name = Peer.DisplayedName;
                RefreshActivePerks();
                CultureID = _cachedCulture.StringId;
            }

            CompassElement.RefreshTargetIconType(targetIconType);
        }

        public void RefreshGold()
        {
            if (Peer != null && _gameMode.IsGameModeUsingGold)
            {
                FlagDominationMissionRepresentative? flagDominationMissionRepresentative;
                if ((flagDominationMissionRepresentative = _missionRepresentative as FlagDominationMissionRepresentative) != null)
                {
                    Gold = flagDominationMissionRepresentative.Gold;
                    IsSpawnActive = Gold >= 100;
                }
            }
            else
            {
                IsSpawnActive = false;
            }
        }

        public void RefreshTeam()
        {
            if (Peer != null)
            {
                uint color1 = 0;
                uint color2 = 0;
                var crpgPeer = Peer.GetComponent<CrpgPeer>();
                if (crpgPeer != null && crpgPeer.Clan != null)
                {
                    color1 = crpgPeer.Clan.PrimaryColor;
                    color2 = crpgPeer.Clan.SecondaryColor;
                }

                BannerCode bannerCode = BannerCode.CreateFrom(new Banner(Peer.Peer.BannerCode,0,0));
                CompassElement.RefreshTeam(bannerCode, Peer.Team?.IsPlayerAlly ?? false);
                CompassElement.RefreshColor(Peer.Team?.Color ?? 0, Peer.Team?.Color2 ?? 0);
            }
        }

        public void RefreshProperties()
        {
            bool flag = MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue() > 0;
            IsValueEnabled = Peer?.Team != null && Peer.Team == _playerTeam || flag;
            if (IsValueEnabled)
            {
                if (flag)
                {
                    ValuePercent = Peer?.BotsUnderControlTotal != 0 ? (int)(Peer!.BotsUnderControlAlive / (float)Peer.BotsUnderControlTotal * 100f) : 0;
                }
                else
                {
                    ValuePercent = Peer!.ControlledAgent != null ? MathF.Ceiling(Peer.ControlledAgent.Health / Peer.ControlledAgent.HealthLimit * 100f) : 0;
                }
            }
        }

        public void RefreshPreview(BasicCharacterObject character, DynamicBodyProperties dynamicBodyProperties, bool isFemale)
        {
            Preview = new MPArmoryHeroPreviewVM();
            Preview.SetCharacter(character, dynamicBodyProperties, character.Race, isFemale);
        }

        public void RefreshActivePerks()
        {
            ActivePerks.Clear();
            var mPHeroClassForPeer = MultiplayerClassDivisions.GetMPHeroClassForPeer(Peer);
            if (Peer == null || Peer.Culture == null || mPHeroClassForPeer == null)
            {
                return;
            }

            foreach (var selectedPerk in Peer.SelectedPerks)
            {
                ActivePerks.Add(new MPPerkVM(null, selectedPerk, isSelectable: false, 0));
            }
        }

        public void RefreshAvatar()
        {
            int num = -1;
            Avatar = new ImageIdentifierVM(forcedAvatarIndex: NetworkMain.GameClient.HasUserGeneratedContentPrivilege ? !BannerlordConfig.EnableGenericAvatars || _isKnownPlayer ? -1 : AvatarServices.GetForcedAvatarIndexOfPlayer(Peer.Peer.Id) : AvatarServices.GetForcedAvatarIndexOfPlayer(Peer.Peer.Id), id: Peer.Peer.Id);
        }

        public void ExecuteFocusBegin()
        {
            SetFocusState(isFocused: true);
        }

        public void ExecuteFocusEnd()
        {
            SetFocusState(isFocused: false);
        }

        private void SetFocusState(bool isFocused)
        {
            uint value = isFocused ? 4278255612u : 0u;
            if (Peer != null)
            {
                Peer.GetAgentVisualForPeer(0)?.GetCopyAgentVisualsData().AgentVisuals.SetContourColor(value);
            }

            IsFocused = isFocused;
        }
    }
}
