using System;
using System.Collections.Generic;
using System.Linq;
using Crpg.Module.Scripts;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Objects.Siege;
using static Crpg.Module.Scripts.Transition;

namespace Crpg.Module.Scripts;

public class CrpgPortcullis : UsableMachine
{
    private const string OpenTag = "open";
    private const string CloseTag = "close";
    private const float DistanceThreshold = 0.01f;

    private ActionIndexCache _actStart = default!;
    private ActionIndexCache _actEnd = default!;

    public GateState State => GetGateState();
    public bool GateMoving => IsGateMoving();

    public enum GateState
    {
        Open,
        Closed,
        Opening,
        Closing,
    }

    private static readonly Dictionary<TimingFunctionType, CubicBezier> TimingFunctions = new()
    {
        [TimingFunctionType.Linear] = CubicBezier.CreateEase(0, 0, 1.0, 1.0),
        [TimingFunctionType.Ease] = CubicBezier.CreateEase(0.25, 0.1, 0.25, 1.0),
        [TimingFunctionType.EaseIn] = CubicBezier.CreateEase(0.42, 0.0, 1.0, 1.0),
        [TimingFunctionType.EaseOut] = CubicBezier.CreateEase(0.0, 0.0, 0.58, 1.0),
        [TimingFunctionType.EaseInOut] = CubicBezier.CreateEase(0.42, 0.0, 0.58, 1.0),
    };

    private SynchedMissionObject? _synchedObject;
    private DestructableComponent? _destructableComponent;
    private SoundEvent? _movementSound;
    private int _batteringRamHitSoundIdCache;
    private bool _isMoveSoundPlaying;
    private bool _hasMovedSinceLastStop = false;
    private int _movementSoundIndex;
    private int _closeSoundIndex;
    private float _currentDuration;
    private float _duration;
    private Vec3 _startPosition;
    private Vec3 _closedPosition;
    private Vec3 _openPosition;
    private Vec3 _targetPosition;
    private GateState _previousState;
    private TimingFunctionType _currentTimingFunction;

#pragma warning disable SA1401 // Bannerlord editor expects fields
#pragma warning disable SA1202
    public string MovementSoundEffect = "event:/mission/siege/siegetower/move";
    public string CloseSoundEffect = "event:/mission/siege/generic/stone_destroy";
    public string StartAnimation = "act_pickup_middle_begin";
    public string EndAnimation = "act_pickup_middle_end";
    public string BatteringRamHitSound = "event:/mission/siege/door/hit";
    public float OpenDuration = 5f;
    public float CloseDuration = 1f;
    public Vec3 Translation = Vec3.Zero;

    /// <summary>As defined in https://developer.mozilla.org/en-US/docs/Web/CSS/easing-function.</summary>
    public TimingFunctionType OpenTimingFunction = TimingFunctionType.EaseInOut;
    public TimingFunctionType CloseTimingFunction = TimingFunctionType.EaseIn;

#pragma warning restore SA1202
#pragma warning restore SA1401

    public override TickRequirement GetTickRequirement()
    {
        return base.GetTickRequirement() | TickRequirement.Tick | TickRequirement.TickParallel;
    }

    public GateState GetGateState()
    {
        if (_synchedObject != null)
        {
            Vec3 origin = _synchedObject.GameEntity.GetFrame().origin;
            float distanceSquared = (origin - _targetPosition).LengthSquared;
            if (distanceSquared > DistanceThreshold) // avoid floating point imprecision
            {
                if (_targetPosition == _closedPosition)
                {
                    return GateState.Closing;
                }

                return GateState.Opening;
            }
            else if ((origin - _closedPosition).LengthSquared < DistanceThreshold)
            {
                return GateState.Closed;
            }
        }

        return GateState.Open;
    }

    public bool IsGateMoving()
    {
        if (_synchedObject != null)
        {
            Vec3 origin = _synchedObject.GameEntity.GetFrame().origin;
            if ((origin - _closedPosition).LengthSquared < DistanceThreshold || (origin - _openPosition).LengthSquared < DistanceThreshold)
            {
                return false;
            }
        }

        return true;
    }

    public void OpenDoor()
    {
        if (!IsDisabled)
        {
            _currentDuration = 0;
            _duration = OpenDuration;
            _startPosition = _closedPosition;
            _targetPosition = _openPosition;
            _currentTimingFunction = OpenTimingFunction;
        }
    }

    public void CloseDoor()
    {
        if (!IsDisabled)
        {
            _currentDuration = 0;
            _duration = CloseDuration;
            _startPosition = _openPosition;
            _targetPosition = _closedPosition;
            _currentTimingFunction = CloseTimingFunction;
        }
    }

    protected override void OnInit()
    {
        base.OnInit();
        Debug.Print($"Initialize {nameof(Transition)} script on entity {GameEntity.GetGuid()}");
        SetScriptComponentToTick(GetTickRequirement());

        CollectGameEntities();
        if (_synchedObject == null)
        {
            Debug.Print($"Entity {GameEntity.GetGuid()} has a {nameof(CrpgPortcullis)} script but no {nameof(SynchedMissionObject)} one");
            return;
        }

        if (_destructableComponent != null && !GameNetwork.IsClientOrReplay)
        {
            _batteringRamHitSoundIdCache = SoundEvent.GetEventIdFromString(BatteringRamHitSound);
            _destructableComponent.OnDestroyed += new DestructableComponent.OnHitTakenAndDestroyedDelegate(OnDestroyed);
            _destructableComponent.OnHitTaken += new DestructableComponent.OnHitTakenAndDestroyedDelegate(OnHitTaken);
            _destructableComponent.BattleSide = BattleSideEnum.Defender;
        }

        _closedPosition = _synchedObject.GameEntity.GetFrame().origin;
        _targetPosition = _closedPosition;
        _startPosition = _closedPosition;
        _openPosition = _closedPosition + Translation;
        _previousState = State;
        _actStart = ActionIndexCache.Create(StartAnimation);
        _actEnd = ActionIndexCache.Create(EndAnimation);
        _movementSoundIndex = SoundEvent.GetEventIdFromString(MovementSoundEffect);
        _closeSoundIndex = SoundEvent.GetEventIdFromString(CloseSoundEffect);
    }

    protected override void OnTick(float dt)
    {
        base.OnTick(dt);

        if (!GameNetwork.IsClientOrReplay)
        {
            ServerTick(dt);
        }

        TickSound();
    }

    protected override void OnTickParallel(float dt)
    {
        if (!GameNetwork.IsServer || _synchedObject == null)
        {
            return;
        }

        var frame = _synchedObject.GameEntity.GetFrame();
        float distanceSquared = (frame.origin - _targetPosition).LengthSquared;

        if (distanceSquared < DistanceThreshold)
        {
            return;
        }

        if (_currentDuration >= _duration)
        {
            _currentDuration = _duration;
        }

        float durationProgress = _currentDuration / _duration;
        float transitionProgress = (float)TimingFunctions[_currentTimingFunction].Sample(durationProgress);
        frame.origin = _startPosition + (_targetPosition - _startPosition) * transitionProgress;
        _synchedObject.SetFrameSynched(ref frame);
        _currentDuration += dt;
    }

    protected override void OnRemoved(int removeReason)
    {
        base.OnRemoved(removeReason);
        if (_destructableComponent != null)
        {
            if (!GameNetwork.IsClientOrReplay)
            {
                _destructableComponent.OnDestroyed -= new DestructableComponent.OnHitTakenAndDestroyedDelegate(OnDestroyed);
                _destructableComponent.OnHitTaken -= new DestructableComponent.OnHitTakenAndDestroyedDelegate(OnHitTaken);
            }
        }
    }

    private void TickSound()
    {
        if (GateMoving)
        {
            PlayMovementSound();
            return;
        }

        StopMovementSound();
    }

    private void CheckGateClosed()
    {
        if (_synchedObject != null)
        {
            Vec3 currentPosition = _synchedObject.GameEntity.GetFrame().origin;
            if (State == GateState.Closed && _hasMovedSinceLastStop)
            {
                OnGateClosed();
                _hasMovedSinceLastStop = false;
            }
            else if (State != GateState.Closed && State != _previousState)
            {
                _hasMovedSinceLastStop = true;
                _previousState = State;
            }
        }
    }

    private void OnGateClosed()
    {
        Mission.Current.MakeSound(_closeSoundIndex, GameEntity.GlobalPosition, false, true, -1, -1);
    }

    private void PlayMovementSound()
    {
        if (!_isMoveSoundPlaying)
        {
            _movementSound = SoundEvent.CreateEvent(_movementSoundIndex, GameEntity.Scene);
            _movementSound.Play();
            _isMoveSoundPlaying = true;
        }

        _movementSound?.SetPosition(GameEntity.GlobalPosition);
    }

    private void StopMovementSound()
    {
        if (_isMoveSoundPlaying)
        {
            _movementSound?.Stop();
            _isMoveSoundPlaying = false;
        }
    }

    private void ServerTick(float dt)
    {
        if (!IsDeactivated)
        {
            CheckGateClosed();
            foreach (StandingPoint standingPoint in StandingPoints)
            {
                if (standingPoint.HasUser)
                {
                    Agent userAgent = standingPoint.UserAgent;
                    if (standingPoint.GameEntity.HasTag(OpenTag))
                    {
                        if (IsUserAgentAnimationEnding(userAgent))
                        {
                            OpenDoor();
                        }
                    }
                    else if (standingPoint.GameEntity.HasTag(CloseTag))
                    {
                        if (IsUserAgentAnimationEnding(userAgent))
                        {
                            CloseDoor();
                        }
                    }
                }
            }

            if (_destructableComponent == null || (_destructableComponent != null && !_destructableComponent.IsDestroyed))
            {
                foreach (StandingPoint standingPoint2 in StandingPoints)
                {
                    bool isDeactivatedSynched;

                    switch (State)
                    {
                        case GateState.Open:
                            isDeactivatedSynched = standingPoint2.GameEntity.HasTag(OpenTag);
                            break;
                        case GateState.Closed:
                            isDeactivatedSynched = standingPoint2.GameEntity.HasTag(CloseTag);
                            break;
                        default:
                            isDeactivatedSynched = true;
                            break;
                    }

                    standingPoint2.SetIsDeactivatedSynched(isDeactivatedSynched);
                }
            }
        }
    }

    private void OnHitTaken(DestructableComponent hitComponent, Agent hitterAgent, in MissionWeapon weapon, ScriptComponentBehavior attackerScriptComponentBehavior, int inflictedDamage)
    {
        if (!GameNetwork.IsClientOrReplay && inflictedDamage >= 200 && State == GateState.Closed && attackerScriptComponentBehavior is BatteringRam)
        {
            Mission.Current.MakeSound(_batteringRamHitSoundIdCache, GameEntity.GlobalPosition, false, true, -1, -1);
        }
    }

    private void OnDestroyed(DestructableComponent destroyedComponent, Agent destroyerAgent, in MissionWeapon weapon, ScriptComponentBehavior attackerScriptComponentBehavior, int inflictedDamage)
    {
        if (!GameNetwork.IsClientOrReplay)
        {
            foreach (StandingPoint standingPoint in StandingPoints)
            {
                standingPoint.SetIsDeactivatedSynched(true);
            }
        }
    }

    public enum TimingFunctionType
    {
        Linear,
        Ease,
        EaseIn,
        EaseOut,
        EaseInOut,
    }

    public override string GetDescriptionText(GameEntity? gameEntity = null)
    {
        return new TextObject("{=feRWCkrF}Portcullis", null).ToString();
    }

    public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
    {
        TextObject textObject = new(usableGameObject.GameEntity.HasTag(OpenTag) ? "{=5oozsaIb}{KEY} Open" : "{=TJj71hPO}{KEY} Close", null);
        textObject.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13)));
        return textObject;
    }

    protected void CollectGameEntities()
    {
        List<GameEntity> list = GameEntity.CollectChildrenEntitiesWithTag("portcullis");
        if (list.Count > 0)
        {
            _synchedObject = list.FirstOrDefault().GetFirstScriptOfType<SynchedMissionObject>();
            _destructableComponent = list.FirstOrDefault().GetFirstScriptOfType<DestructableComponent>();
        }
    }

    private bool IsUserAgentAnimationEnding(Agent agent)
    {
        ActionIndexCache currentAction = agent.GetCurrentAction(0);
        ActionIndexCache currentAction2 = agent.GetCurrentAction(1);
        if (currentAction != _actStart && currentAction != _actEnd)
        {
            agent.SetActionChannel(0, _actStart, false, 0UL, 0f, 0.8f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
        }

        return currentAction == _actEnd;
    }
}
