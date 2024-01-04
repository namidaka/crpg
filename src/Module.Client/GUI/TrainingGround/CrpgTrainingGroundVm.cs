using System.Diagnostics;
using Crpg.Module.Modes.TrainingGround;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
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

    private readonly CrpgTrainingGroundMissionMultiplayerClient _client;
    private readonly MissionMultiplayerGameModeBaseClient _gameMode;
    private bool _isMyRepresentativeAssigned;
    private TextObject _scoreWithSeparatorText;
    private bool _isAgentBuiltForTheFirstTime = true;
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
    private CrpgDuelMatchVm _playerDuelMatch = default!;
    private MBBindingList<CrpgDuelMatchVm> _ongoingDuels = default!;
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
    public CrpgDuelMatchVm PlayerDuelMatch
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
    public MBBindingList<CrpgDuelMatchVm> OngoingDuels
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
        PlayerDuelMatch = new CrpgDuelMatchVm();
        OngoingDuels = new MBBindingList<CrpgDuelMatchVm>();
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
        myRepresentative3.OnDuelPreparationStartedForTheFirstTimeEvent = (Action<MissionPeer, MissionPeer>)Delegate.Combine(myRepresentative3.OnDuelPreparationStartedForTheFirstTimeEvent, new Action<MissionPeer, MissionPeer>(OnDuelStarted));
        CrpgTrainingGroundMissionRepresentative myRepresentative4 = _client.MyRepresentative;
        myRepresentative4.OnDuelEndedEvent = (Action<MissionPeer>)Delegate.Combine(myRepresentative4.OnDuelEndedEvent, new Action<MissionPeer>(OnDuelEnded));
        CrpgTrainingGroundMissionRepresentative myRepresentative5 = _client.MyRepresentative;
        myRepresentative5.OnDuelRoundEndedEvent = (Action<MissionPeer>)Delegate.Combine(myRepresentative5.OnDuelRoundEndedEvent, new Action<MissionPeer>(OnDuelRoundEnded));
        CrpgTrainingGroundMissionRepresentative myRepresentative6 = _client.MyRepresentative;
        ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Combine(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(OnManagedOptionChanged));
        Markers.RegisterEvents();
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
            myRepresentative3.OnDuelPreparationStartedForTheFirstTimeEvent = (Action<MissionPeer, MissionPeer>)Delegate.Remove(myRepresentative3.OnDuelPreparationStartedForTheFirstTimeEvent, new Action<MissionPeer, MissionPeer>(OnDuelStarted));
            CrpgTrainingGroundMissionRepresentative myRepresentative4 = _client.MyRepresentative;
            myRepresentative4.OnDuelEndedEvent = (Action<MissionPeer>)Delegate.Remove(myRepresentative4.OnDuelEndedEvent, new Action<MissionPeer>(OnDuelEnded));
            CrpgTrainingGroundMissionRepresentative myRepresentative5 = _client.MyRepresentative;
            myRepresentative5.OnDuelRoundEndedEvent = (Action<MissionPeer>)Delegate.Remove(myRepresentative5.OnDuelRoundEndedEvent, new Action<MissionPeer>(OnDuelRoundEnded));
            CrpgTrainingGroundMissionRepresentative myRepresentative6 = _client.MyRepresentative;
            ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Remove(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(OnManagedOptionChanged));
            Markers.UnregisterEvents();
        }
    }

    private void OnManagedOptionChanged(ManagedOptions.ManagedOptionsType changedManagedOptionsType)
    {
        if (changedManagedOptionsType == ManagedOptions.ManagedOptionsType.EnableGenericNames)
        {
            _ongoingDuels.ApplyActionOnAllItems(delegate (CrpgDuelMatchVm d)
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

    private void OnDuelStarted(MissionPeer firstPeer, MissionPeer secondPeer)
    {
        Markers.OnDuelStarted(firstPeer, secondPeer);
        if (firstPeer == _client.MyRepresentative.MissionPeer || secondPeer == _client.MyRepresentative.MissionPeer)
        {
            AreOngoingDuelsActive = false;
            IsPlayerInDuel = true;
            PlayerDuelMatch.OnDuelStarted(firstPeer, secondPeer);
        }
        else
        {
            CrpgDuelMatchVm duelMatchVM = new();
            duelMatchVM.OnDuelStarted(firstPeer, secondPeer);
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
            Markers.SetMarkerOfPeerEnabled(PlayerDuelMatch.FirstPlayerPeer!, isEnabled: true);
            Markers.SetMarkerOfPeerEnabled(PlayerDuelMatch.SecondPlayerPeer!, isEnabled: true);
            PlayerDuelMatch.OnDuelEnded();
        }

        CrpgDuelMatchVm duelMatchVM = OngoingDuels.FirstOrDefault((CrpgDuelMatchVm d) => d.FirstPlayerPeer == winnerPeer || d.SecondPlayerPeer == winnerPeer);
        if (duelMatchVM != null)
        {
            Markers.SetMarkerOfPeerEnabled(duelMatchVM.FirstPlayerPeer!, isEnabled: true);
            Markers.SetMarkerOfPeerEnabled(duelMatchVM.SecondPlayerPeer!, isEnabled: true);
            OngoingDuels.Remove(duelMatchVM);
        }
    }

    private void OnDuelRoundEnded(MissionPeer winnerPeer)
    {
        if (PlayerDuelMatch.FirstPlayerPeer == winnerPeer || PlayerDuelMatch.SecondPlayerPeer == winnerPeer)
        {
            PlayerDuelMatch.OnPeerScored(winnerPeer);
            KillNotifications.Add(new MPDuelKillNotificationItemVM(PlayerDuelMatch.FirstPlayerPeer, PlayerDuelMatch.SecondPlayerPeer, PlayerDuelMatch.FirstPlayerScore, PlayerDuelMatch.SecondPlayerScore, TroopType.Infantry, RemoveKillNotification));
            return;
        }

        CrpgDuelMatchVm duelMatchVM = OngoingDuels.FirstOrDefault((CrpgDuelMatchVm d) => d.FirstPlayerPeer == winnerPeer || d.SecondPlayerPeer == winnerPeer);
        if (duelMatchVM != null)
        {
            duelMatchVM.OnPeerScored(winnerPeer);
            KillNotifications.Add(new MPDuelKillNotificationItemVM(duelMatchVM.FirstPlayerPeer, duelMatchVM.SecondPlayerPeer, duelMatchVM.FirstPlayerScore, duelMatchVM.SecondPlayerScore, TroopType.Infantry, RemoveKillNotification));
        }
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
        if (_isAgentBuiltForTheFirstTime || (stringId != _cachedPlayerClassID))
        {
            _isAgentBuiltForTheFirstTime = false;
            _cachedPlayerClassID = stringId;
        }
    }

}
