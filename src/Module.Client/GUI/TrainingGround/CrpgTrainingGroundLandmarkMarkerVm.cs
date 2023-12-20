using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Crpg.Module.GUI.TrainingGround;

public class CrpgTrainingGroundLandmarkMarkerVm : ViewModel
{
    public readonly GameEntity Entity;
    public readonly IFocusable FocusableComponent;
    private float _latestX;
    private float _latestY;
    private float _latestW;
    private bool _isFocused;
    private int _troopType;
    private string _actionDescriptionText = string.Empty;
    public bool IsInScreenBoundaries { get; private set; }

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
    public int TroopType
    {
        get
        {
            return _troopType;
        }
        set
        {
            if (value != _troopType)
            {
                _troopType = value;
                OnPropertyChangedWithValue(value, "TroopType");
            }
        }
    }

    [DataSourceProperty]
    public string ActionDescriptionText
    {
        get
        {
            return _actionDescriptionText;
        }
        set
        {
            if (value != _actionDescriptionText)
            {
                _actionDescriptionText = value;
                OnPropertyChangedWithValue(value, "ActionDescriptionText");
            }
        }
    }

    public CrpgTrainingGroundLandmarkMarkerVm(GameEntity entity)
    {
        Entity = entity;
        FocusableComponent = Entity.GetFirstScriptOfType<DuelZoneLandmark>();
        TroopType = (int)Entity.GetFirstScriptOfType<DuelZoneLandmark>().ZoneTroopType;
        RefreshValues();
    }

    public override void RefreshValues()
    {
        base.RefreshValues();
        string keyHyperlinkText = HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13));
        GameTexts.SetVariable("KEY", keyHyperlinkText);
        GameTexts.SetVariable("ACTION", new TextObject("{=7jMnNlXG}Change Arena Preference"));
        ActionDescriptionText = GameTexts.FindText("str_key_action").ToString();
    }

    public void UpdateScreenPosition(Camera missionCamera)
    {
        Vec3 globalPosition = Entity.GlobalPosition;
        _latestX = 0f;
        _latestY = 0f;
        _latestW = 0f;
        MBWindowManager.WorldToScreen(missionCamera, globalPosition, ref _latestX, ref _latestY, ref _latestW);
        IsInScreenBoundaries = _latestW > 0f && !(_latestX > Screen.RealScreenResolutionWidth) && !(_latestY > Screen.RealScreenResolutionHeight) && !(_latestX + 200f < 0f) && !(_latestY + 100f < 0f);
    }
}
