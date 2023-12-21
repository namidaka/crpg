using System;
using System.Collections.Generic;
using System.Linq;
using Crpg.Module.Modes.TrainingGround;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Crpg.Module.GUI.TrainingGround;

public class CrpgTrainingGroundMarkersVm : ViewModel
{
    private class PeerMarkerDistanceComparer : IComparer<CrpgTrainingGroundPeerMarkerVm>
    {
        public int Compare(CrpgTrainingGroundPeerMarkerVm x, CrpgTrainingGroundPeerMarkerVm y)
        {
            return y.Distance.CompareTo(x.Distance);
        }
    }

    private const string ZoneLandmarkTag = "duel_zone_landmark";
    private const float FocusScreenDistanceThreshold = 350f;
    private const float LandmarkFocusDistanceThrehsold = 500f;
    private bool _hasEnteredLobby;
    private Camera _missionCamera;
    private CrpgTrainingGroundPeerMarkerVm? _previousFocusTarget;
    private CrpgTrainingGroundPeerMarkerVm? _currentFocusTarget;
    private CrpgTrainingGroundLandmarkMarkerVm? _previousLandmarkTarget;
    private CrpgTrainingGroundLandmarkMarkerVm? _currentLandmarkTarget;
    private PeerMarkerDistanceComparer _distanceComparer;
    private readonly Dictionary<MissionPeer, CrpgTrainingGroundPeerMarkerVm> _targetPeersToMarkersDictionary;
    private readonly CrpgTrainingGroundMissionMultiplayerClient _client;
    private Vec2 _screenCenter;
    private Dictionary<MissionPeer, bool> _targetPeersInDuelDictionary;
    private int _playerPreferredArenaType;
    private bool _isPlayerFocused;
    private bool _isEnabled;
    private MBBindingList<CrpgTrainingGroundPeerMarkerVm> _targets = default!;
    private MBBindingList<CrpgTrainingGroundLandmarkMarkerVm> _landmarks = default!;

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
                UpdateTargetsEnabled(value);
            }
        }
    }

    [DataSourceProperty]
    public MBBindingList<CrpgTrainingGroundPeerMarkerVm> Targets
    {
        get
        {
            return _targets;
        }
        set
        {
            if (value != _targets)
            {
                _targets = value;
                OnPropertyChangedWithValue(value, "Targets");
            }
        }
    }

    [DataSourceProperty]
    public MBBindingList<CrpgTrainingGroundLandmarkMarkerVm> Landmarks
    {
        get
        {
            return _landmarks;
        }
        set
        {
            if (value != _landmarks)
            {
                _landmarks = value;
                OnPropertyChangedWithValue(value, "Landmarks");
            }
        }
    }

    public CrpgTrainingGroundMarkersVm(Camera missionCamera, CrpgTrainingGroundMissionMultiplayerClient client)
    {
        _missionCamera = missionCamera;
        _client = client;
        List<GameEntity> list = new List<GameEntity>();
        list.AddRange(Mission.Current.Scene.FindEntitiesWithTag("duel_zone_landmark"));
        Landmarks = new MBBindingList<CrpgTrainingGroundLandmarkMarkerVm>();
        foreach (GameEntity item in list)
        {
            Landmarks.Add(new CrpgTrainingGroundLandmarkMarkerVm(item));
        }

        Targets = new MBBindingList<CrpgTrainingGroundPeerMarkerVm>();
        _targetPeersToMarkersDictionary = new Dictionary<MissionPeer, CrpgTrainingGroundPeerMarkerVm>();
        _targetPeersInDuelDictionary = new Dictionary<MissionPeer, bool>();
        _distanceComparer = new PeerMarkerDistanceComparer();
        UpdateScreenCenter();
        RefreshValues();
    }

    public override void RefreshValues()
    {
        base.RefreshValues();
        Targets.ApplyActionOnAllItems(delegate (CrpgTrainingGroundPeerMarkerVm t)
        {
            t.RefreshValues();
        });
        Landmarks.ApplyActionOnAllItems(delegate (CrpgTrainingGroundLandmarkMarkerVm l)
        {
            l.RefreshValues();
        });
    }

    public void UpdateScreenCenter()
    {
        _screenCenter = new Vec2(Screen.RealScreenResolutionWidth / 2f, Screen.RealScreenResolutionHeight / 2f);
    }

    public void Tick(float dt)
    {
        if (_hasEnteredLobby && GameNetwork.MyPeer != null)
        {
            OnRefreshPeerMarkers();
            UpdateTargets(dt);
        }
    }

    public void RegisterEvents()
    {
        CrpgTrainingGroundMissionRepresentative myRepresentative = _client.MyRepresentative;
        myRepresentative.OnDuelRequestSentEvent = (Action<MissionPeer>)Delegate.Combine(myRepresentative.OnDuelRequestSentEvent, new Action<MissionPeer>(OnDuelRequestSent));
        CrpgTrainingGroundMissionRepresentative myRepresentative2 = _client.MyRepresentative;
        myRepresentative2.OnDuelRequestedEvent = (Action<MissionPeer, TroopType>)Delegate.Combine(myRepresentative2.OnDuelRequestedEvent, new Action<MissionPeer, TroopType>(OnDuelRequested));
        ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Combine(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(OnManagedOptionsChanged));
    }

    public void UnregisterEvents()
    {
        CrpgTrainingGroundMissionRepresentative myRepresentative = _client.MyRepresentative;
        myRepresentative.OnDuelRequestSentEvent = (Action<MissionPeer>)Delegate.Remove(myRepresentative.OnDuelRequestSentEvent, new Action<MissionPeer>(OnDuelRequestSent));
        CrpgTrainingGroundMissionRepresentative myRepresentative2 = _client.MyRepresentative;
        myRepresentative2.OnDuelRequestedEvent = (Action<MissionPeer, TroopType>)Delegate.Remove(myRepresentative2.OnDuelRequestedEvent, new Action<MissionPeer, TroopType>(OnDuelRequested));
        ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Remove(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(OnManagedOptionsChanged));
    }

    private void OnManagedOptionsChanged(ManagedOptions.ManagedOptionsType changedManagedOptionsType)
    {
        if (changedManagedOptionsType == ManagedOptions.ManagedOptionsType.EnableGenericNames)
        {
            Targets.ApplyActionOnAllItems(delegate (CrpgTrainingGroundPeerMarkerVm t)
            {
                t.RefreshValues();
            });
        }
    }

    private void UpdateTargets(float dt)
    {
        if (_currentFocusTarget != null)
        {
            _previousFocusTarget = _currentFocusTarget;
            _currentFocusTarget = null;
            if (_isPlayerFocused)
            {
                _previousFocusTarget.IsFocused = false;
            }
        }

        if (_currentLandmarkTarget != null)
        {
            _previousLandmarkTarget = _currentLandmarkTarget;
            _currentLandmarkTarget = null;
            if (_isPlayerFocused)
            {
                _previousLandmarkTarget.IsFocused = false;
            }
        }

        if (_client.MyRepresentative?.MissionPeer.ControlledAgent == null)
        {
            return;
        }

        float num = float.MaxValue;
        foreach (CrpgTrainingGroundPeerMarkerVm target in Targets)
        {
            target.OnTick(dt);
            if (target.IsEnabled)
            {
                if (!target.HasSentDuelRequest && !target.HasDuelRequestForPlayer && target.TargetPeer.ControlledAgent != null)
                {
                    target.PreferredArenaType = _playerPreferredArenaType;
                }

                target.UpdateScreenPosition(_missionCamera);
                target.HasDuelRequestForPlayer = _client.MyRepresentative.CheckHasRequestFromAndRemoveRequestIfNeeded(target.TargetPeer);
                float num2 = target.ScreenPosition.Distance(_screenCenter);
                if (!_isPlayerFocused && target.WSign >= 0 && num2 < 350f && num2 < num)
                {
                    num = num2;
                    _currentFocusTarget = target;
                }
            }
        }

        Targets.Sort(_distanceComparer);
        if (_client.MyRepresentative == null)
        {
            return;
        }

        if (_currentFocusTarget != null && _currentFocusTarget.TargetPeer.ControlledAgent != null)
        {
            _client.MyRepresentative.OnObjectFocused(_currentFocusTarget!.TargetPeer.ControlledAgent);
            if (_previousFocusTarget != null && _currentFocusTarget.TargetPeer != _previousFocusTarget.TargetPeer)
            {
                _previousFocusTarget!.IsFocused = false;
            }

            _currentFocusTarget.IsFocused = true;
            if (_previousLandmarkTarget != null)
            {
                _previousLandmarkTarget.IsFocused = false;
            }

            return;
        }

        if (_previousFocusTarget != null)
        {
            _previousFocusTarget.IsFocused = false;
        }

        foreach (CrpgTrainingGroundLandmarkMarkerVm landmark in Landmarks)
        {
            if (Agent.Main == null)
            {
                continue;
            }

            landmark.UpdateScreenPosition(_missionCamera);
            if (_isPlayerFocused || !landmark.IsInScreenBoundaries || !(Agent.Main.GetWorldPosition().GetGroundVec3().DistanceSquared(landmark.Entity.GlobalPosition) < 500f))
            {
                continue;
            }

            landmark.IsFocused = true;
            _currentLandmarkTarget = landmark;
            if (_previousLandmarkTarget != landmark)
            {
                if (_previousLandmarkTarget != null)
                {
                    _previousLandmarkTarget.IsFocused = false;
                }

                _currentLandmarkTarget.IsFocused = true;
            }

            _client.MyRepresentative.OnObjectFocused(landmark.FocusableComponent);
            break;
        }

        if (_currentLandmarkTarget == null && _previousLandmarkTarget != null)
        {
            _previousLandmarkTarget.IsFocused = false;
        }

        if (_currentFocusTarget == null && _currentLandmarkTarget == null)
        {
            _client.MyRepresentative.OnObjectFocusLost();
        }
    }

    public void RefreshPeerEquipments()
    {
        foreach (MissionPeer item in VirtualPlayer.Peers<MissionPeer>())
        {
            OnPeerEquipmentRefreshed(item);
        }
    }

    private void OnRefreshPeerMarkers()
    {
        List<CrpgTrainingGroundPeerMarkerVm> list = Targets.ToList();
        foreach (MissionPeer item in VirtualPlayer.Peers<MissionPeer>())
        {
            if (item?.Team == null || !item.IsControlledAgentActive || item.IsMine)
            {
                continue;
            }

            if (!_targetPeersToMarkersDictionary.ContainsKey(item))
            {
                CrpgTrainingGroundPeerMarkerVm CrpgTrainingGroundPeerMarkerVm = new(item);
                Targets.Add(CrpgTrainingGroundPeerMarkerVm);
                _targetPeersToMarkersDictionary.Add(item, CrpgTrainingGroundPeerMarkerVm);
                OnPeerEquipmentRefreshed(item);
                if (_targetPeersInDuelDictionary.ContainsKey(item))
                {
                    CrpgTrainingGroundPeerMarkerVm.UpdateCurentDuelStatus(_targetPeersInDuelDictionary[item]);
                }
            }
            else
            {
                list.Remove(_targetPeersToMarkersDictionary[item]);
            }

            if (!_targetPeersInDuelDictionary.ContainsKey(item))
            {
                _targetPeersInDuelDictionary.Add(item, value: false);
            }
        }

        foreach (CrpgTrainingGroundPeerMarkerVm item2 in list)
        {
            Targets.Remove(item2);
            _targetPeersToMarkersDictionary.Remove(item2.TargetPeer);
        }
    }

    private void UpdateTargetsEnabled(bool isEnabled)
    {
        foreach (CrpgTrainingGroundPeerMarkerVm target in Targets)
        {
            target.IsEnabled = !target.IsInDuel && isEnabled;
        }
    }

    private void OnDuelRequestSent(MissionPeer targetPeer)
    {
        foreach (CrpgTrainingGroundPeerMarkerVm target in Targets)
        {
            if (target.TargetPeer == targetPeer)
            {
                target.HasSentDuelRequest = true;
            }
        }
    }

    private void OnDuelRequested(MissionPeer targetPeer, TroopType troopType)
    {
        CrpgTrainingGroundPeerMarkerVm CrpgTrainingGroundPeerMarkerVm = Targets.FirstOrDefault((CrpgTrainingGroundPeerMarkerVm t) => t.TargetPeer == targetPeer);
        if (CrpgTrainingGroundPeerMarkerVm != null)
        {
            CrpgTrainingGroundPeerMarkerVm.HasDuelRequestForPlayer = true;
            CrpgTrainingGroundPeerMarkerVm.PreferredArenaType = (int)troopType;
        }
    }

    public void OnAgentSpawnedWithoutDuel()
    {
        _hasEnteredLobby = true;
        IsEnabled = true;
    }

    public void OnAgentBuiltForTheFirstTime()
    {
        _playerPreferredArenaType = (int)CrpgTrainingGroundVm.GetAgentDefaultPreferredArenaType(Agent.Main);
    }

    public void OnDuelStarted(MissionPeer firstPeer, MissionPeer secondPeer)
    {
        if (_client.MyRepresentative.MissionPeer == firstPeer || _client.MyRepresentative.MissionPeer == secondPeer)
        {
            IsEnabled = false;
        }

        foreach (CrpgTrainingGroundPeerMarkerVm target in Targets)
        {
            if (target.TargetPeer == firstPeer || target.TargetPeer == secondPeer)
            {
                target.OnDuelStarted();
            }
        }

        _targetPeersInDuelDictionary[firstPeer] = true;
        _targetPeersInDuelDictionary[secondPeer] = true;
    }

    public void SetMarkerOfPeerEnabled(MissionPeer peer, bool isEnabled)
    {
        if (peer != null)
        {
            if (_targetPeersToMarkersDictionary.ContainsKey(peer))
            {
                _targetPeersToMarkersDictionary[peer].UpdateCurentDuelStatus(!isEnabled);
                _targetPeersToMarkersDictionary[peer].UpdateBounty();
            }

            if (_targetPeersInDuelDictionary.ContainsKey(peer))
            {
                _targetPeersInDuelDictionary[peer] = !isEnabled;
            }
        }
    }

    public void OnPlayerPreferredZoneChanged(int playerPrefferedArenaType)
    {
        _playerPreferredArenaType = playerPrefferedArenaType;
    }

    public void OnFocusGained()
    {
        _isPlayerFocused = true;
    }

    public void OnFocusLost()
    {
        _isPlayerFocused = false;
    }

    public void OnPeerEquipmentRefreshed(MissionPeer peer)
    {
        if (_targetPeersToMarkersDictionary.TryGetValue(peer, out var value))
        {
            value.RefreshPerkSelection();
        }
    }
}
