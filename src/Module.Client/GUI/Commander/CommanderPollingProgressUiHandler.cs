﻿using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.Multiplayer.View.MissionViews;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using Crpg.Module.Common.Commander;
using TaleWorlds.MountAndBlade;

namespace Crpg.Module.GUI.Commander;
public class CommanderPollingProgressUiHandler : MissionView
{
    private CrpgCommanderPollComponent? _commanderPollComponent;
    private CommanderPollProgressVm? _dataSource;
    private GauntletLayer? _gauntletLayer;
    private bool _isActive;
    private bool _isVoteOpenForMyPeer;
    private MissionPeer? _targetPeer;
    private InputContext _input
    {
        get
        {
            return MissionScreen.SceneLayer.Input;
        }
    }

    public CommanderPollingProgressUiHandler()
    {
        ViewOrderPriority = 24;
    }

    public override void OnMissionScreenInitialize()
    {
        base.OnMissionScreenInitialize();
        _dataSource = new();
        _commanderPollComponent = Mission.GetMissionBehavior<CrpgCommanderPollComponent>();
        _gauntletLayer = new GauntletLayer(ViewOrderPriority, "GauntletLayer", false);
        _gauntletLayer.LoadMovie("MultiplayerPollingProgress", _dataSource);
        _input.RegisterHotKeyCategory(HotKeyManager.GetCategory("PollHotkeyCategory"));
        _dataSource.AddKey(HotKeyManager.GetCategory("PollHotkeyCategory").GetGameKey(106));
        _dataSource.AddKey(HotKeyManager.GetCategory("PollHotkeyCategory").GetGameKey(107));
        MissionScreen.AddLayer(_gauntletLayer);
    }

    public override void AfterStart()
    {
        if (_commanderPollComponent != null)
        {
            _commanderPollComponent.OnCommanderPollOpened += OnCommanderPollOpened;
            _commanderPollComponent.OnPollUpdated += OnPollUpdated;
            _commanderPollComponent.OnPollCancelled += OnPollClosed;
            _commanderPollComponent.OnPollClosed += OnPollClosed;
        }
    }

    public override void OnMissionScreenFinalize()
    {
        MissionScreen.RemoveLayer(_gauntletLayer);
        _dataSource!.OnFinalize();
        MissionScreen.SetDisplayDialog(false);
        base.OnMissionScreenFinalize();
        if (_commanderPollComponent != null)
        {
            _commanderPollComponent.OnCommanderPollOpened -= OnCommanderPollOpened;
            _commanderPollComponent.OnPollUpdated -= OnPollUpdated;
            _commanderPollComponent.OnPollCancelled -= OnPollClosed;
            _commanderPollComponent.OnPollClosed -= OnPollClosed;
        }
    }

    private void OnCommanderPollOpened(MissionPeer initiatorPeer, MissionPeer targetPeer, bool isDemoteRequested)
    {
        _isActive = true;
        _targetPeer = targetPeer;
        _isVoteOpenForMyPeer = NetworkMain.GameClient.PlayerID == targetPeer.Peer.Id;
        _dataSource!.OnCommanderPollOpened(initiatorPeer, targetPeer, isDemoteRequested);
    }

    private void OnPollUpdated(int votesAccepted, int votesRejected)
    {
        _dataSource!.OnPollUpdated(votesAccepted, votesRejected);
    }

    private void OnPollClosed(CrpgCommanderPollComponent.CommanderPoll poll)
    {
        _isActive = false;
        _targetPeer = null;
        _dataSource!.OnPollClosed();
    }

    public override void OnMissionScreenTick(float dt)
    {
        base.OnMissionScreenTick(dt);
        if (_isActive && !_isVoteOpenForMyPeer)
        {
            if (_input.IsGameKeyPressed(106))
            {
                _isActive = false;
                _commanderPollComponent!.Vote(_commanderPollComponent.GetCommanderPollBySide(_targetPeer!.Team.Side), true);
                _dataSource!.OnPollOptionPicked();
                return;
            }
            if (_input.IsGameKeyPressed(107))
            {
                _isActive = false;
                _commanderPollComponent!.Vote(_commanderPollComponent.GetCommanderPollBySide(_targetPeer!.Team.Side), false);
                _dataSource!.OnPollOptionPicked();
            }
        }
    }

}