using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.GauntletUI.Layout;
using TaleWorlds.TwoDimension;

using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.NameMarker;

namespace Crpg.Module.GUI.TrainingGround;
internal class CrpgTrainingGroundTargetMarkerListPanel : ListPanel
{
    private const string DefaultState = "Default";
    private const string FocusedState = "Focused";
    private const string TrackedState = "Tracked";
    private Vec2 _position;
    private bool _isAgentInScreenBoundaries;
    private bool _isAvailable;
    private bool _isTracked;
    private bool _isAgentFocused;
    private bool _hasTargetSentDuelRequest;
    private bool _hasPlayerSentDuelRequest;
    private int _wSign;
    private RichTextWidget _actionText = default!;
    private BrushWidget _background = default!;
    private BrushWidget _border = default!;
    private BrushWidget _troopClassBorder = default!;
    [Editor(false)]
    public Vec2 Position
    {
        get
        {
            return _position;
        }
        set
        {
            if (value != _position)
            {
                _position = value;
                OnPropertyChanged(value, "Position");
            }
        }
    }

    [Editor(false)]
    public bool IsAgentInScreenBoundaries
    {
        get
        {
            return _isAgentInScreenBoundaries;
        }
        set
        {
            if (value != _isAgentInScreenBoundaries)
            {
                _isAgentInScreenBoundaries = value;
                OnPropertyChanged(value, "IsAgentInScreenBoundaries");
            }
        }
    }

    [Editor(false)]
    public bool IsAvailable
    {
        get
        {
            return _isAvailable;
        }
        set
        {
            if (value != _isAvailable)
            {
                _isAvailable = value;
                OnPropertyChanged(value, "IsAvailable");
            }
        }
    }

    [Editor(false)]
    public bool IsTracked
    {
        get
        {
            return _isTracked;
        }
        set
        {
            if (value != _isTracked)
            {
                _isTracked = value;
                OnPropertyChanged(value, "IsTracked");
            }
        }
    }

    [Editor(false)]
    public bool IsAgentFocused
    {
        get
        {
            return _isAgentFocused;
        }
        set
        {
            if (value != _isAgentFocused)
            {
                _isAgentFocused = value;
                OnPropertyChanged(value, "IsAgentFocused");
                UpdateChildrenFocusStates();
            }
        }
    }

    [Editor(false)]
    public bool HasTargetSentDuelRequest
    {
        get
        {
            return _hasTargetSentDuelRequest;
        }
        set
        {
            if (value != _hasTargetSentDuelRequest)
            {
                _hasTargetSentDuelRequest = value;
                OnPropertyChanged(value, "HasTargetSentDuelRequest");
                UpdateChildrenFocusStates();
            }
        }
    }

    [Editor(false)]
    public bool HasPlayerSentDuelRequest
    {
        get
        {
            return _hasPlayerSentDuelRequest;
        }
        set
        {
            if (value != _hasPlayerSentDuelRequest)
            {
                _hasPlayerSentDuelRequest = value;
                OnPropertyChanged(value, "HasPlayerSentDuelRequest");
                UpdateChildrenFocusStates();
            }
        }
    }

    [Editor(false)]
    public int WSign
    {
        get
        {
            return _wSign;
        }
        set
        {
            if (_wSign != value)
            {
                _wSign = value;
                OnPropertyChanged(value, "WSign");
            }
        }
    }

    [Editor(false)]
    public RichTextWidget ActionText
    {
        get
        {
            return _actionText;
        }
        set
        {
            if (value != _actionText)
            {
                _actionText = value;
                OnPropertyChanged(value, "ActionText");
            }
        }
    }

    [Editor(false)]
    public BrushWidget Background
    {
        get
        {
            return _background;
        }
        set
        {
            if (value != _background)
            {
                _background = value;
                OnPropertyChanged(value, "Background");
            }
        }
    }

    [Editor(false)]
    public BrushWidget Border
    {
        get
        {
            return _border;
        }
        set
        {
            if (value != _border)
            {
                _border = value;
                OnPropertyChanged(value, "Border");
            }
        }
    }

    [Editor(false)]
    public BrushWidget TroopClassBorder
    {
        get
        {
            return _troopClassBorder;
        }
        set
        {
            if (value != _troopClassBorder)
            {
                _troopClassBorder = value;
                OnPropertyChanged(value, "TroopClassBorder");
            }
        }
    }

    public CrpgTrainingGroundTargetMarkerListPanel(UIContext context)
        : base(context)
    {
    }

    protected override void OnLateUpdate(float dt)
    {
        if (!IsAvailable)
        {
            base.IsVisible = false;
            return;
        }

        float x = Context.EventManager.PageSize.X;
        float y = Context.EventManager.PageSize.Y;
        Vec2 position = Position;
        if (WSign > 0 && position.x - base.Size.X / 2f > 0f && position.x + base.Size.X / 2f < base.Context.EventManager.PageSize.X && position.y > 0f && position.y + base.Size.Y < base.Context.EventManager.PageSize.Y)
        {
            base.ScaledPositionXOffset = position.x - base.Size.X / 2f;
            base.ScaledPositionYOffset = position.y - base.Size.Y - 20f;
            _actionText.ScaledPositionXOffset = base.ScaledPositionXOffset;
            _actionText.ScaledPositionYOffset = base.ScaledPositionYOffset + base.Size.Y;
            base.IsVisible = true;
        }
        else if (IsTracked)
        {
            Vec2 vec = new Vec2(base.Context.EventManager.PageSize.X / 2f, base.Context.EventManager.PageSize.Y / 2f);
            position -= vec;
            if (WSign < 0)
            {
                position *= -1f;
            }

            float radian = Mathf.Atan2(position.y, position.x) - (float)Math.PI / 2f;
            float num = Mathf.Cos(radian);
            float num2 = Mathf.Sin(radian);
            position = vec + new Vec2(num2 * 150f, num * 150f);
            float num3 = num / num2;
            Vec2 vec2 = vec * 1f;
            position = ((num > 0f) ? new Vec2((0f - vec2.y) / num3, vec.y) : new Vec2(vec2.y / num3, 0f - vec.y));
            if (position.x > vec2.x)
            {
                position = new Vec2(vec2.x, (0f - vec2.x) * num3);
            }
            else if (position.x < 0f - vec2.x)
            {
                position = new Vec2(0f - vec2.x, vec2.x * num3);
            }

            position += vec;
            base.ScaledPositionXOffset = Mathf.Clamp(position.x - base.Size.X / 2f, 0f, x - base.Size.X);
            base.ScaledPositionYOffset = Mathf.Clamp(position.y - base.Size.Y, 0f, y - base.Size.Y);
            base.IsVisible = true;
        }
        else
        {
            base.IsVisible = false;
        }
    }

    private void UpdateChildrenFocusStates()
    {
        string state = (HasTargetSentDuelRequest ? "Tracked" : ((HasPlayerSentDuelRequest || IsAgentFocused) ? "Focused" : "Default"));
        Background.SetState(state);
        Border.SetState(state);
        TroopClassBorder?.SetState(state);
    }
}
