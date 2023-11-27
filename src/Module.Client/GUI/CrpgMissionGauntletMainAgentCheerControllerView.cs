using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Crpg.Module.Helpers;
using NetworkMessages.FromClient;
using TaleWorlds.Core;
using TaleWorlds.Diamond;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI.Mission;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.ViewModelCollection;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD;

namespace Crpg.Module.GUI;

[OverrideView(typeof(MissionMainAgentCheerBarkControllerView))]
public class CrpgMissionGauntletMainAgentCheerControllerView : MissionView
{
    private readonly IMissionScreen _missionScreenAsInterface;
    private MissionMainAgentController? _missionMainAgentController;
    private readonly TextObject _cooldownInfoText = new("{=aogZyZlR}You need to wait {SECONDS} seconds until you can cheer/shout again.");
    private readonly TextObject _barkCooldownTimeInfoText = new("{=BorZeZPs}You need to wait {SECONDS} seconds until you can shout again.");
    private bool _holdHandled;
    private float _holdTime;
    private bool _prevCheerKeyDown;
    private GauntletLayer? _gauntletLayer;
    private MissionMainAgentCheerBarkControllerVM? _dataSource;
    private float _cooldownTimeRemaining;
    private int _barkUsesLeft;
    private float _barkUsesResetCooldownTime;
    private bool _isSelectingFromInput;
    private bool _isReturningToCategories;

    private bool IsDisplayingADialog
    {
        get
        {
            if (!(_missionScreenAsInterface?.GetDisplayDialog() ?? false) && !MissionScreen.IsRadialMenuActive)
            {
                return Mission.IsOrderMenuOpen;
            }

            return true;
        }
    }

    private bool HoldHandled
    {
        get
        {
            return _holdHandled;
        }
        set
        {
            _holdHandled = value;
            MissionScreen?.SetRadialMenuActiveState(value);
        }
    }

    public CrpgMissionGauntletMainAgentCheerControllerView()
    {
        _missionScreenAsInterface = MissionScreen;
        HoldHandled = false;
    }

    public override void OnMissionScreenInitialize()
    {
        base.OnMissionScreenInitialize();
        _gauntletLayer = new GauntletLayer(2);
        _missionMainAgentController = Mission.GetMissionBehavior<MissionMainAgentController>();
        _dataSource = new MissionMainAgentCheerBarkControllerVM(OnCheerSelect, OnBarkSelect, Agent.TauntCheerActions, SkinVoiceManager.VoiceType.MpBarks);
        _gauntletLayer.LoadMovie("MainAgentCheerBarkController", _dataSource);
        _gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("CombatHotKeyCategory"));
        MissionScreen.AddLayer(_gauntletLayer);
        Mission.OnMainAgentChanged += OnMainAgentChanged;
        ResetBarkUses();
    }

    public override void OnMissionScreenFinalize()
    {
        base.OnMissionScreenFinalize();
        Mission.OnMainAgentChanged -= OnMainAgentChanged;
        MissionScreen.RemoveLayer(_gauntletLayer);
        _gauntletLayer = null;
        _dataSource?.OnFinalize();
        _dataSource = null;
        _missionMainAgentController = null;
    }

    public override void OnMissionScreenTick(float dt)
    {
        base.OnMissionScreenTick(dt);
        if (IsMainAgentAvailable() && Mission.Mode != MissionMode.Deployment && (!MissionScreen.IsRadialMenuActive || (_dataSource != null && _dataSource.IsActive)))
        {
            TickControls(dt);
        }
    }

    private void OnMainAgentChanged(object sender, PropertyChangedEventArgs e)
    {
        if (Mission.MainAgent == null)
        {
            if (HoldHandled)
            {
                HoldHandled = false;
            }

            _holdTime = 0f;
            _dataSource?.OnCancelHoldController();
        }
    }

    private void HandleNodeSelectionInput(CheerBarkNodeItemVM node, int nodeIndex, int parentNodeIndex = -1)
    {
        if (node.ShortcutKey == null)
        {
            return;
        }

        if (MissionScreen.SceneLayer.Input.IsHotKeyPressed(node.ShortcutKey.HotKey.Id))
        {
            if (parentNodeIndex != -1)
            {
                _dataSource?.SelectItem(parentNodeIndex, nodeIndex);
                _isReturningToCategories = nodeIndex == 0;
            }
            else
            {
                _dataSource?.SelectItem(nodeIndex);
                _isSelectingFromInput = node.HasSubNodes;
            }
        }
        else
        {
            if (!MissionScreen.SceneLayer.Input.IsHotKeyReleased(node.ShortcutKey.HotKey.Id))
            {
                return;
            }

            if (!_isSelectingFromInput)
            {
                if (_isReturningToCategories && nodeIndex >= 0 && nodeIndex < _dataSource?.Nodes.Count)
                {
                    _dataSource.Nodes[nodeIndex].SubNodes.ApplyActionOnAllItems(delegate(CheerBarkNodeItemVM x)
                    {
                        x.IsSelected = false;
                    });
                    _dataSource.Nodes.ApplyActionOnAllItems(delegate(CheerBarkNodeItemVM x)
                    {
                        x.IsSelected = false;
                    });
                }
                else
                {
                    HandleClosingHoldCheer();
                }
            }

            _isSelectingFromInput = false;
        }
    }

    private void TickControls(float dt)
    {
        if (GameNetwork.IsMultiplayer && _cooldownTimeRemaining > 0f)
        {
            _cooldownTimeRemaining -= dt;
            if (MissionScreen.SceneLayer.Input.IsGameKeyDown(31))
            {
                if (!_prevCheerKeyDown && (double)_cooldownTimeRemaining >= 0.1)
                {
                    _cooldownInfoText.SetTextVariable("SECONDS", _cooldownTimeRemaining.ToString("0.0"));
                    InformationManager.DisplayMessage(new InformationMessage(_cooldownInfoText.ToString()));
                }

                _prevCheerKeyDown = true;
            }
            else
            {
                _prevCheerKeyDown = false;
            }

            return;
        }

        if (GameNetwork.IsMultiplayer && _barkUsesResetCooldownTime > 0f)
        {
            _barkUsesResetCooldownTime -= dt;
        }
        else
        {
            ResetBarkUses();
        }

        if (HoldHandled)
        {
            int num = -1;
            for (int i = 0; i < _dataSource?.Nodes.Count; i++)
            {
                if (_dataSource.Nodes[i].IsSelected)
                {
                    num = i;
                    break;
                }
            }

            if (_dataSource != null && _dataSource.IsNodesCategories)
            {
                if (num != -1)
                {
                    for (int j = 0; j < _dataSource.Nodes[num].SubNodes.Count; j++)
                    {
                        HandleNodeSelectionInput(_dataSource.Nodes[num].SubNodes[j], j, num);
                    }
                }
                else if (MissionScreen.SceneLayer.Input.IsHotKeyReleased("CheerBarkSelectFirstCategory"))
                {
                    _dataSource.SelectItem(0);
                }
                else if (MissionScreen.SceneLayer.Input.IsHotKeyReleased("CheerBarkSelectSecondCategory"))
                {
                    if (_barkUsesLeft > 0)
                    {
                        _dataSource.SelectItem(1);
                    }
                    else
                    {
                        _barkCooldownTimeInfoText.SetTextVariable("SECONDS", _barkUsesResetCooldownTime.ToString("0.0"));
                        InformationManager.DisplayMessage(new InformationMessage(_barkCooldownTimeInfoText.ToString()));
                    }
                }
            }
            else
            {
                for (int k = 0; k < _dataSource?.Nodes.Count; k++)
                {
                    HandleNodeSelectionInput(_dataSource.Nodes[k], k);
                }
            }
        }

        if (MissionScreen.SceneLayer.Input.IsGameKeyDown(31) && !IsDisplayingADialog && IsMainAgentAvailable() && !MissionScreen.IsRadialMenuActive)
        {
            if (_holdTime > 0f && !HoldHandled)
            {
                HandleOpenHold();
                HoldHandled = true;
            }

            _holdTime += dt;
            _prevCheerKeyDown = true;
        }
        else if (_prevCheerKeyDown && !MissionScreen.SceneLayer.Input.IsGameKeyDown(31))
        {
            if (_holdTime < 0f)
            {
                HandleQuickReleaseCheer();
            }
            else
            {
                HandleClosingHoldCheer();
            }

            HoldHandled = false;
            _holdTime = 0f;
            _prevCheerKeyDown = false;
        }
    }

    private void HandleOpenHold()
    {
        _dataSource?.OnSelectControllerToggle(isActive: true);
        MissionScreen.SetRadialMenuActiveState(isActive: true);
    }

    private void HandleClosingHoldCheer()
    {
        _dataSource?.OnSelectControllerToggle(isActive: false);
        MissionScreen.SetRadialMenuActiveState(isActive: false);
    }

    private void HandleQuickReleaseCheer()
    {
        OnCheerSelect(0);
    }

    private void OnCheerSelect(int indexOfCheer)
    {
        if (GameNetwork.IsClient)
        {
            GameNetwork.BeginModuleEventAsClient();
            GameNetwork.WriteMessage(new CheerSelected(indexOfCheer));
            GameNetwork.EndModuleEventAsClient();
        }
        else
        {
            Agent.Main.HandleCheer(indexOfCheer);
        }

        _cooldownTimeRemaining = 4f;
    }

    private void OnBarkSelect(int indexOfBark)
    {
        if (GameNetwork.IsClient)
        {
            GameNetwork.BeginModuleEventAsClient();
            GameNetwork.WriteMessage(new BarkSelected(indexOfBark));
            GameNetwork.EndModuleEventAsClient();
        }
        else
        {
            Agent.Main.HandleBark(indexOfBark);
        }

        _cooldownTimeRemaining = 5f;
        _barkUsesLeft -= 1;
    }

    private void ResetBarkUses()
    {
        _barkUsesLeft = 3;
        _barkUsesResetCooldownTime = 60;
    }

    private bool IsMainAgentAvailable()
    {
        return Agent.Main?.IsActive() ?? false;
    }

    public override void OnPhotoModeActivated()
    {
        base.OnPhotoModeActivated();
        if (_gauntletLayer == null)
        {
            return;
        }

        _gauntletLayer._gauntletUIContext.ContextAlpha = 0f;
    }

    public override void OnPhotoModeDeactivated()
    {
        base.OnPhotoModeDeactivated();
        if (_gauntletLayer == null)
        {
            return;
        }

        _gauntletLayer._gauntletUIContext.ContextAlpha = 1f;
    }
}
