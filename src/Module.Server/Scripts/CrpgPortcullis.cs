﻿using System;
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

    private ActionIndexCache _actStart = default!;
    private ActionIndexCache _actEnd = default!;

    public GateState State => GetGateState();
    public bool GateMoving => IsGateMoving();

    public enum GateState
    {
        Open,
        Closed,
        Moving,
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
    private SoundEvent? _movementSound;
    private bool _isMoveSoundPlaying;
    private int _soundIndex;
    private float _duration;
    private Vec3 _startPosition;
    private Vec3 _closedPosition;
    private Vec3 _openPosition;
    private Vec3 _targetPosition;

#pragma warning disable SA1401 // Bannerlord editor expects fields
#pragma warning disable SA1202
    public string MovementSoundEffect = "event:/mission/siege/siegetower/move";
    public string StartAnimation = "act_pickup_middle_begin";
    public string EndAnimation = "act_pickup_middle_end";
    public float Duration = 5f;
    public Vec3 Translation = Vec3.Zero;

    /// <summary>As defined in https://developer.mozilla.org/en-US/docs/Web/CSS/easing-function.</summary>
    public TimingFunctionType TimingFunction = TimingFunctionType.Linear;

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
            if (origin != _targetPosition)
            {
                return GateState.Moving;
            }
            else if (origin == _closedPosition)
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
            if (origin.Distance(_closedPosition) < 0.05f || origin.Distance(_openPosition) < 0.05f)
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
            _duration = 0;
            _startPosition = _closedPosition;
            _targetPosition = _openPosition;
        }
    }

    public void CloseDoor()
    {
        if (!IsDisabled)
        {
            _duration = 0;
            _startPosition = _openPosition;
            _targetPosition = _closedPosition;
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

        _closedPosition = _synchedObject.GameEntity.GetFrame().origin;
        _targetPosition = _closedPosition;
        _startPosition = _closedPosition;
        _openPosition = _closedPosition + Translation;
        _actStart = ActionIndexCache.Create(StartAnimation);
        _actEnd = ActionIndexCache.Create(EndAnimation);
        _soundIndex = SoundEvent.GetEventIdFromString(MovementSoundEffect);
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

        if (frame.origin == _targetPosition)
        {
            return;
        }

        if (_duration >= Duration)
        {
            _duration = Duration;
        }

        float durationProgress = _duration / Duration;
        float transitionProgress = (float)TimingFunctions[TimingFunction].Sample(durationProgress);
        frame.origin = _startPosition + (_targetPosition - _startPosition) * transitionProgress;
        _synchedObject.SetFrameSynched(ref frame);
        _duration += dt;
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

    private void PlayMovementSound()
    {
        if (!_isMoveSoundPlaying)
        {
            _movementSound = SoundEvent.CreateEvent(_soundIndex, GameEntity.Scene);
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

            if (!IsDestroyed)
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

    public enum TimingFunctionType
    {
        Linear,
        Ease,
        EaseIn,
        EaseOut,
        EaseInOut,
    }

    protected void CollectGameEntities()
    {
        List<GameEntity> list = GameEntity.CollectChildrenEntitiesWithTag("portcullis");
        if (list.Count > 0)
        {
            _synchedObject = list.FirstOrDefault().GetFirstScriptOfType<SynchedMissionObject>();
        }
    }

    public override string GetDescriptionText(GameEntity? gameEntity = null)
    {
        return new TextObject("{=}Portcullis", null).ToString();
    }

    public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
    {
        TextObject textObject = new TextObject(usableGameObject.GameEntity.HasTag(OpenTag) ? "{=5oozsaIb}{KEY} Open" : "{=TJj71hPO}{KEY} Close", null);
        textObject.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13)));
        return textObject;
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
