using Crpg.Module.Modes.TrainingGround;
using System.Diagnostics;
using System.Text;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.MissionRepresentatives;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.HUDExtensions;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.KillFeed;

namespace Crpg.Module.GUI.TrainingGround;
internal class CrpgTrainingGroundVm : ViewModel
{
    public struct DuelArenaProperties
    {
        public GameEntity FlagEntity;

        public int Index;

        public TroopType ArenaTroopType;

        public DuelArenaProperties(GameEntity flagEntity, int index, TroopType arenaTroopType)
        {
            FlagEntity = flagEntity;
            Index = index;
            ArenaTroopType = arenaTroopType;
        }
    }

    private const string ArenaFlagTag = "area_flag";
    private const string AremaTypeFlagTagBase = "flag_";
    private readonly CrpgTrainingGroundMissionMultiplayerClient _client;
    private readonly MissionMultiplayerGameModeBaseClient _gameMode;
    private bool _isMyRepresentativeAssigned;
    private List<DuelArenaProperties> _duelArenaProperties;
    private TextObject _scoreWithSeparatorText;
    private bool _isAgentBuiltForTheFirstTime = true;
    private bool _hasPlayerChangedArenaPreferrence;
    private string _cachedPlayerClassID = string.Empty;
    private bool _showSpawnPoints;
    private Camera _missionCamera;
    private bool _isEnabled;
    private bool _areOngoingDuelsActive;
    private bool _isPlayerInDuel;
    private int _playerBounty;
    private int _playerPreferredArenaType;
    private string _playerScoreText = string.Empty;
    private string _remainingRoundTime = string.Empty;
    private CrpgTrainingGroundMarkersVm _markers = default!;
    private DuelMatchVM _playerDuelMatch = default!;
    private MBBindingList<DuelMatchVM> _ongoingDuels = default!;
    private MBBindingList<MPDuelKillNotificationItemVM> _killNotifications = default!;
    [DataSourceProperty]
    public bool IsEnabled
    {
        get
        {
            return _isEnabled;
        }
        set
        {
            if (value != _isEnabled)
            {
                _isEnabled = value;
                OnPropertyChangedWithValue(value, "IsEnabled");
            }
        }
    }

    [DataSourceProperty]
    public bool AreOngoingDuelsActive
    {
        get
        {
            return _areOngoingDuelsActive;
        }
        set
        {
            if (value != _areOngoingDuelsActive)
            {
                _areOngoingDuelsActive = value;
                OnPropertyChangedWithValue(value, "AreOngoingDuelsActive");
            }
        }
    }

    [DataSourceProperty]
    public bool IsPlayerInDuel
    {
        get
        {
            return _isPlayerInDuel;
        }
        set
        {
            if (value != _isPlayerInDuel)
            {
                _isPlayerInDuel = value;
                OnPropertyChangedWithValue(value, "IsPlayerInDuel");
            }
        }
    }

    [DataSourceProperty]
    public int PlayerBounty
    {
        get
        {
            return _playerBounty;
        }
        set
        {
            if (value != _playerBounty)
            {
                _playerBounty = value;
                OnPropertyChangedWithValue(value, "PlayerBounty");
            }
        }
    }

    [DataSourceProperty]
    public int PlayerPrefferedArenaType
    {
        get
        {
            return _playerPreferredArenaType;
        }
        set
        {
            if (value != _playerPreferredArenaType)
            {
                _playerPreferredArenaType = value;
                OnPropertyChangedWithValue(value, "PlayerPrefferedArenaType");
            }
        }
    }

    [DataSourceProperty]
    public string PlayerScoreText
    {
        get
        {
            return _playerScoreText;
        }
        set
        {
            if (value != _playerScoreText)
            {
                _playerScoreText = value;
                OnPropertyChangedWithValue(value, "PlayerScoreText");
            }
        }
    }

    [DataSourceProperty]
    public string RemainingRoundTime
    {
        get
        {
            return _remainingRoundTime;
        }
        set
        {
            if (value != _remainingRoundTime)
            {
                _remainingRoundTime = value;
                OnPropertyChangedWithValue(value, "RemainingRoundTime");
            }
        }
    }

    [DataSourceProperty]
    public CrpgTrainingGroundMarkersVm Markers
    {
        get
        {
            return _markers;
        }
        set
        {
            if (value != _markers)
            {
                _markers = value;
                OnPropertyChangedWithValue(value, "Markers");
            }
        }
    }

    [DataSourceProperty]
    public DuelMatchVM PlayerDuelMatch
    {
        get
        {
            return _playerDuelMatch;
        }
        set
        {
            if (value != _playerDuelMatch)
            {
                _playerDuelMatch = value;
                OnPropertyChangedWithValue(value, "PlayerDuelMatch");
            }
        }
    }

    [DataSourceProperty]
    public MBBindingList<DuelMatchVM> OngoingDuels
    {
        get
        {
            return _ongoingDuels;
        }
        set
        {
            if (value != _ongoingDuels)
            {
                _ongoingDuels = value;
                OnPropertyChangedWithValue(value, "OngoingDuels");
            }
        }
    }

    [DataSourceProperty]
    public MBBindingList<MPDuelKillNotificationItemVM> KillNotifications
    {
        get
        {
            return _killNotifications;
        }
        set
        {
            if (value != _killNotifications)
            {
                _killNotifications = value;
                OnPropertyChangedWithValue(value, "KillNotifications");
            }
        }
    }

    public CrpgTrainingGroundVm(Camera missionCamera, CrpgTrainingGroundMissionMultiplayerClient client)
    {
        _missionCamera = missionCamera;
        _client = client;
        CrpgTrainingGroundMissionMultiplayerClient client2 = _client;
        client2.OnMyRepresentativeAssigned = (Action)Delegate.Combine(client2.OnMyRepresentativeAssigned, new Action(OnMyRepresentativeAssigned));
        _gameMode = Mission.Current.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
        PlayerDuelMatch = new DuelMatchVM();
        OngoingDuels = new MBBindingList<DuelMatchVM>();
        _duelArenaProperties = new List<DuelArenaProperties>();
        List<GameEntity> list = new List<GameEntity>();
        list.AddRange(Mission.Current.Scene.FindEntitiesWithTagExpression("area_flag(_\\d+)*"));
        foreach (GameEntity item in list)
        {
            DuelArenaProperties arenaPropertiesOfFlagEntity = GetArenaPropertiesOfFlagEntity(item);
            _duelArenaProperties.Add(arenaPropertiesOfFlagEntity);
        }

        Markers = new CrpgTrainingGroundMarkersVm(missionCamera, _client);
        KillNotifications = new MBBindingList<MPDuelKillNotificationItemVM>();
        _scoreWithSeparatorText = new TextObject("{=J5rb5YVV}/ {SCORE}");
        RefreshValues();
    }

    public override void RefreshValues()
    {
        base.RefreshValues();
        PlayerDuelMatch.RefreshValues();
        Markers.RefreshValues();
    }

    private void OnMyRepresentativeAssigned()
    {
        CrpgTrainingGroundMissionRepresentative myRepresentative = _client.MyRepresentative;
        myRepresentative.OnDuelPrepStartedEvent = (Action<MissionPeer, int>)Delegate.Combine(myRepresentative.OnDuelPrepStartedEvent, new Action<MissionPeer, int>(OnDuelPrepStarted));
        CrpgTrainingGroundMissionRepresentative myRepresentative2 = _client.MyRepresentative;
        myRepresentative2.OnAgentSpawnedWithoutDuelEvent = (Action)Delegate.Combine(myRepresentative2.OnAgentSpawnedWithoutDuelEvent, new Action(OnAgentSpawnedWithoutDuel));
        CrpgTrainingGroundMissionRepresentative myRepresentative3 = _client.MyRepresentative;
        myRepresentative3.OnDuelPreparationStartedForTheFirstTimeEvent = (Action<MissionPeer, MissionPeer, int>)Delegate.Combine(myRepresentative3.OnDuelPreparationStartedForTheFirstTimeEvent, new Action<MissionPeer, MissionPeer, int>(OnDuelStarted));
        CrpgTrainingGroundMissionRepresentative myRepresentative4 = _client.MyRepresentative;
        myRepresentative4.OnDuelEndedEvent = (Action<MissionPeer>)Delegate.Combine(myRepresentative4.OnDuelEndedEvent, new Action<MissionPeer>(OnDuelEnded));
        CrpgTrainingGroundMissionRepresentative myRepresentative5 = _client.MyRepresentative;
        myRepresentative5.OnDuelRoundEndedEvent = (Action<MissionPeer>)Delegate.Combine(myRepresentative5.OnDuelRoundEndedEvent, new Action<MissionPeer>(OnDuelRoundEnded));
        CrpgTrainingGroundMissionRepresentative myRepresentative6 = _client.MyRepresentative;
        myRepresentative6.OnMyPreferredZoneChanged = (Action<TroopType>)Delegate.Combine(myRepresentative6.OnMyPreferredZoneChanged, new Action<TroopType>(OnPlayerPreferredZoneChanged));
        ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Combine(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(OnManagedOptionChanged));
        Markers.RegisterEvents();
        UpdatePlayerScore();
        _isMyRepresentativeAssigned = true;
    }

    public void Tick(float dt)
    {
        if (_gameMode.CheckTimer(out var remainingTime, out var _))
        {
            RemainingRoundTime = TimeSpan.FromSeconds(remainingTime).ToString("mm':'ss");
        }

        Markers.Tick(dt);
        if (PlayerDuelMatch.IsEnabled)
        {
            PlayerDuelMatch.Tick(dt);
        }
    }

    [Conditional("DEBUG")]
    private void DebugTick()
    {
        if (Input.IsKeyReleased(InputKey.Numpad3))
        {
            _showSpawnPoints = !_showSpawnPoints;
        }

        if (!_showSpawnPoints)
        {
            return;
        }

        string expression = "spawnpoint_area(_\\d+)*";
        foreach (GameEntity item in Mission.Current.Scene.FindEntitiesWithTagExpression(expression))
        {
            Vec3 worldPoint = new Vec3(item.GlobalPosition.x, item.GlobalPosition.y, item.GlobalPosition.z);
            Vec3 vec = _missionCamera.WorldPointToViewPortPoint(ref worldPoint);
            vec.y = 1f - vec.y;
            if (vec.z < 0f)
            {
                vec.x = 1f - vec.x;
                vec.y = 1f - vec.y;
                vec.z = 0f;
                float num = 0f;
                num = ((vec.x > num) ? vec.x : num);
                num = ((vec.y > num) ? vec.y : num);
                num = ((vec.z > num) ? vec.z : num);
                vec /= num;
            }

            if (float.IsPositiveInfinity(vec.x))
            {
                vec.x = 1f;
            }
            else if (float.IsNegativeInfinity(vec.x))
            {
                vec.x = 0f;
            }

            if (float.IsPositiveInfinity(vec.y))
            {
                vec.y = 1f;
            }
            else if (float.IsNegativeInfinity(vec.y))
            {
                vec.y = 0f;
            }

            vec.x = MathF.Clamp(vec.x, 0f, 1f) * Screen.RealScreenResolutionWidth;
            vec.y = MathF.Clamp(vec.y, 0f, 1f) * Screen.RealScreenResolutionHeight;
        }
    }

    public override void OnFinalize()
    {
        base.OnFinalize();
        CrpgTrainingGroundMissionMultiplayerClient client = _client;
        client.OnMyRepresentativeAssigned = (Action)Delegate.Remove(client.OnMyRepresentativeAssigned, new Action(OnMyRepresentativeAssigned));
        if (_isMyRepresentativeAssigned)
        {
            CrpgTrainingGroundMissionRepresentative myRepresentative = _client.MyRepresentative;
            myRepresentative.OnDuelPrepStartedEvent = (Action<MissionPeer, int>)Delegate.Remove(myRepresentative.OnDuelPrepStartedEvent, new Action<MissionPeer, int>(OnDuelPrepStarted));
            CrpgTrainingGroundMissionRepresentative myRepresentative2 = _client.MyRepresentative;
            myRepresentative2.OnAgentSpawnedWithoutDuelEvent = (Action)Delegate.Remove(myRepresentative2.OnAgentSpawnedWithoutDuelEvent, new Action(OnAgentSpawnedWithoutDuel));
            CrpgTrainingGroundMissionRepresentative myRepresentative3 = _client.MyRepresentative;
            myRepresentative3.OnDuelPreparationStartedForTheFirstTimeEvent = (Action<MissionPeer, MissionPeer, int>)Delegate.Remove(myRepresentative3.OnDuelPreparationStartedForTheFirstTimeEvent, new Action<MissionPeer, MissionPeer, int>(OnDuelStarted));
            CrpgTrainingGroundMissionRepresentative myRepresentative4 = _client.MyRepresentative;
            myRepresentative4.OnDuelEndedEvent = (Action<MissionPeer>)Delegate.Remove(myRepresentative4.OnDuelEndedEvent, new Action<MissionPeer>(OnDuelEnded));
            CrpgTrainingGroundMissionRepresentative myRepresentative5 = _client.MyRepresentative;
            myRepresentative5.OnDuelRoundEndedEvent = (Action<MissionPeer>)Delegate.Remove(myRepresentative5.OnDuelRoundEndedEvent, new Action<MissionPeer>(OnDuelRoundEnded));
            CrpgTrainingGroundMissionRepresentative myRepresentative6 = _client.MyRepresentative;
            myRepresentative6.OnMyPreferredZoneChanged = (Action<TroopType>)Delegate.Remove(myRepresentative6.OnMyPreferredZoneChanged, new Action<TroopType>(OnPlayerPreferredZoneChanged));
            ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Remove(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(OnManagedOptionChanged));
            Markers.UnregisterEvents();
        }
    }

    private void OnManagedOptionChanged(ManagedOptions.ManagedOptionsType changedManagedOptionsType)
    {
        if (changedManagedOptionsType == ManagedOptions.ManagedOptionsType.EnableGenericNames)
        {
            _ongoingDuels.ApplyActionOnAllItems(delegate (DuelMatchVM d)
            {
                d.RefreshNames(changeGenericNames: true);
            });
        }
    }

    private void OnDuelPrepStarted(MissionPeer opponentPeer, int duelStartTime)
    {
        PlayerDuelMatch.OnDuelPrepStarted(opponentPeer, duelStartTime);
        AreOngoingDuelsActive = false;
        Markers.IsEnabled = false;
    }

    private void OnAgentSpawnedWithoutDuel()
    {
        Markers.OnAgentSpawnedWithoutDuel();
        AreOngoingDuelsActive = true;
    }

    private void OnPlayerPreferredZoneChanged(TroopType zoneType)
    {
        if (zoneType != (TroopType)PlayerPrefferedArenaType)
        {
            PlayerPrefferedArenaType = (int)zoneType;
            Markers.OnPlayerPreferredZoneChanged((int)zoneType);
            _hasPlayerChangedArenaPreferrence = true;
            GameTexts.SetVariable("ARENA_TYPE", GetArenaTypeLocalizedName(zoneType));
            InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=nLdQvaRK}Arena preference updated to {ARENA_TYPE}.").ToString()));
        }
        else
        {
            InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=YLZV7dxI}This arena type is already the preferred one.").ToString()));
        }
    }

    private void OnDuelStarted(MissionPeer firstPeer, MissionPeer secondPeer, int flagIndex)
    {
        Markers.OnDuelStarted(firstPeer, secondPeer);
        DuelArenaProperties duelArenaProperties = _duelArenaProperties.First((DuelArenaProperties f) => f.Index == flagIndex);
        if (firstPeer == _client.MyRepresentative.MissionPeer || secondPeer == _client.MyRepresentative.MissionPeer)
        {
            AreOngoingDuelsActive = false;
            IsPlayerInDuel = true;
            PlayerDuelMatch.OnDuelStarted(firstPeer, secondPeer, (int)duelArenaProperties.ArenaTroopType);
        }
        else
        {
            DuelMatchVM duelMatchVM = new DuelMatchVM();
            duelMatchVM.OnDuelStarted(firstPeer, secondPeer, (int)duelArenaProperties.ArenaTroopType);
            OngoingDuels.Add(duelMatchVM);
        }
    }

    private void OnDuelEnded(MissionPeer winnerPeer)
    {
        if (PlayerDuelMatch.FirstPlayerPeer == winnerPeer || PlayerDuelMatch.SecondPlayerPeer == winnerPeer)
        {
            AreOngoingDuelsActive = true;
            IsPlayerInDuel = false;
            Markers.IsEnabled = true;
            Markers.SetMarkerOfPeerEnabled(PlayerDuelMatch.FirstPlayerPeer, isEnabled: true);
            Markers.SetMarkerOfPeerEnabled(PlayerDuelMatch.SecondPlayerPeer, isEnabled: true);
            PlayerDuelMatch.OnDuelEnded();
            PlayerBounty = _client.MyRepresentative.Bounty;
            UpdatePlayerScore();
        }

        DuelMatchVM duelMatchVM = OngoingDuels.FirstOrDefault((DuelMatchVM d) => d.FirstPlayerPeer == winnerPeer || d.SecondPlayerPeer == winnerPeer);
        if (duelMatchVM != null)
        {
            Markers.SetMarkerOfPeerEnabled(duelMatchVM.FirstPlayerPeer, isEnabled: true);
            Markers.SetMarkerOfPeerEnabled(duelMatchVM.SecondPlayerPeer, isEnabled: true);
            OngoingDuels.Remove(duelMatchVM);
        }
    }

    private void OnDuelRoundEnded(MissionPeer winnerPeer)
    {
        if (PlayerDuelMatch.FirstPlayerPeer == winnerPeer || PlayerDuelMatch.SecondPlayerPeer == winnerPeer)
        {
            PlayerDuelMatch.OnPeerScored(winnerPeer);
            KillNotifications.Add(new MPDuelKillNotificationItemVM(PlayerDuelMatch.FirstPlayerPeer, PlayerDuelMatch.SecondPlayerPeer, PlayerDuelMatch.FirstPlayerScore, PlayerDuelMatch.SecondPlayerScore, (TroopType)PlayerDuelMatch.ArenaType, RemoveKillNotification));
            return;
        }

        DuelMatchVM duelMatchVM = OngoingDuels.FirstOrDefault((DuelMatchVM d) => d.FirstPlayerPeer == winnerPeer || d.SecondPlayerPeer == winnerPeer);
        if (duelMatchVM != null)
        {
            duelMatchVM.OnPeerScored(winnerPeer);
            KillNotifications.Add(new MPDuelKillNotificationItemVM(duelMatchVM.FirstPlayerPeer, duelMatchVM.SecondPlayerPeer, duelMatchVM.FirstPlayerScore, duelMatchVM.SecondPlayerScore, (TroopType)duelMatchVM.ArenaType, RemoveKillNotification));
        }
    }

    private void UpdatePlayerScore()
    {
        GameTexts.SetVariable("SCORE", _client.MyRepresentative.Score);
        PlayerScoreText = _scoreWithSeparatorText.ToString();
    }

    private void RemoveKillNotification(MPDuelKillNotificationItemVM item)
    {
        KillNotifications.Remove(item);
    }

    public void OnScreenResolutionChanged()
    {
        Markers.UpdateScreenCenter();
    }

    public void OnMainAgentRemoved()
    {
        if (!PlayerDuelMatch.IsEnabled)
        {
            Markers.IsEnabled = false;
            AreOngoingDuelsActive = false;
        }
    }

    public void OnMainAgentBuild()
    {
        if (!PlayerDuelMatch.IsEnabled)
        {
            Markers.IsEnabled = true;
            AreOngoingDuelsActive = true;
        }

        string stringId = MultiplayerClassDivisions.GetMPHeroClassForPeer(_client.MyRepresentative.MissionPeer).StringId;
        if (_isAgentBuiltForTheFirstTime || (stringId != _cachedPlayerClassID && !_hasPlayerChangedArenaPreferrence))
        {
            PlayerPrefferedArenaType = (int)GetAgentDefaultPreferredArenaType(Agent.Main);
            Markers.OnAgentBuiltForTheFirstTime();
            _isAgentBuiltForTheFirstTime = false;
            _cachedPlayerClassID = stringId;
        }
    }

    private string GetArenaTypeName(TroopType duelArenaType)
    {
        switch (duelArenaType)
        {
            case TroopType.Infantry:
                return "infantry";
            case TroopType.Ranged:
                return "archery";
            case TroopType.Cavalry:
                return "cavalry";
            default:
                TaleWorlds.Library.Debug.FailedAssert("Invalid duel arena type!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection\\MultiplayerDuelVM.cs", "GetArenaTypeName", 363);
                return "";
        }
    }

    private TextObject GetArenaTypeLocalizedName(TroopType duelArenaType)
    {
        switch (duelArenaType)
        {
            case TroopType.Infantry:
                return new TextObject("{=1Bm1Wk1v}Infantry");
            case TroopType.Ranged:
                return new TextObject("{=OJbpmlXu}Ranged");
            case TroopType.Cavalry:
                return new TextObject("{=YVGtcLHF}Cavalry");
            default:
                TaleWorlds.Library.Debug.FailedAssert("Invalid duel arena type!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection\\MultiplayerDuelVM.cs", "GetArenaTypeLocalizedName", 379);
                return TextObject.Empty;
        }
    }

    private DuelArenaProperties GetArenaPropertiesOfFlagEntity(GameEntity flagEntity)
    {
        DuelArenaProperties result = default(DuelArenaProperties);
        result.FlagEntity = flagEntity;
        string text = flagEntity.Tags.FirstOrDefault((string t) => t.StartsWith("area_flag"));
        if (!text.IsEmpty())
        {
            result.Index = int.Parse(text.Substring(text.LastIndexOf('_') + 1)) - 1;
        }
        else
        {
            result.Index = 0;
            TaleWorlds.Library.Debug.FailedAssert("Flag has duel_area Tag Missing!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection\\MultiplayerDuelVM.cs", "GetArenaPropertiesOfFlagEntity", 397);
        }

        result.ArenaTroopType = TroopType.Infantry;
        for (TroopType troopType = TroopType.Infantry; troopType < TroopType.NumberOfTroopTypes; troopType++)
        {
            if (flagEntity.HasTag("flag_" + GetArenaTypeName(troopType)))
            {
                result.ArenaTroopType = troopType;
            }
        }

        return result;
    }

    public static TroopType GetAgentDefaultPreferredArenaType(Agent agent)
    {
        return agent.Character.DefaultFormationClass.GetTroopTypeForRegularFormation();
    }
}
