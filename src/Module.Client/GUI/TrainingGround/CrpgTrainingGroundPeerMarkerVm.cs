using Crpg.Module.Modes.TrainingGround;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.ClassLoadout;

namespace Crpg.Module.GUI.TrainingGround;

public class CrpgTrainingGroundPeerMarkerVm : ViewModel
{
    private float _currentDuelRequestTimeRemaining;
    private float _latestX;
    private float _latestY;
    private float _latestW;
    private float _wPosAfterPositionCalculation;
    private TextObject _acceptDuelRequestText = TextObject.Empty;
    private TextObject _sendDuelRequestText = TextObject.Empty;
    private TextObject _waitingForDuelResponseText = TextObject.Empty;
    private bool _isEnabled;
    private bool _isTracked;
    private bool _shouldShowInformation;
    private bool _isAgentInScreenBoundaries;
    private bool _isFocused;
    private bool _hasDuelRequestForPlayer;
    private bool _hasSentDuelRequest;
    private string _name = string.Empty;
    private string _actionDescriptionText = string.Empty;
    private int _bounty;
    private int _preferredArenaType;
    private int _wSign;
    private Vec2 _screenPosition;
    private MPTeammateCompassTargetVM _compassElement = default!;
    private MBBindingList<MPPerkVM> _selectedPerks = default!;
    public MissionPeer TargetPeer { get; private set; }

    public float Distance { get; private set; }

    public bool IsInDuel { get; private set; }

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
                UpdateTracked();
            }
        }
    }

    [DataSourceProperty]
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
                OnPropertyChangedWithValue(value, "IsTracked");
            }
        }
    }

    [DataSourceProperty]
    public bool ShouldShowInformation
    {
        get
        {
            return _shouldShowInformation;
        }
        set
        {
            if (value != _shouldShowInformation)
            {
                _shouldShowInformation = value;
                OnPropertyChangedWithValue(value, "ShouldShowInformation");
            }
        }
    }

    [DataSourceProperty]
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
                OnPropertyChangedWithValue(value, "IsAgentInScreenBoundaries");
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
                SetFocused(value);
                UpdateTracked();
            }
        }
    }

    [DataSourceProperty]
    public bool HasDuelRequestForPlayer
    {
        get
        {
            return _hasDuelRequestForPlayer;
        }
        set
        {
            if (value != _hasDuelRequestForPlayer)
            {
                _hasDuelRequestForPlayer = value;
                OnPropertyChangedWithValue(value, "HasDuelRequestForPlayer");
                OnInteractionChanged();
                UpdateTracked();
            }
        }
    }

    [DataSourceProperty]
    public bool HasSentDuelRequest
    {
        get
        {
            return _hasSentDuelRequest;
        }
        set
        {
            if (value != _hasSentDuelRequest)
            {
                _hasSentDuelRequest = value;
                OnPropertyChangedWithValue(value, "HasSentDuelRequest");
                OnInteractionChanged();
                UpdateTracked();
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

    [DataSourceProperty]
    public int Bounty
    {
        get
        {
            return _bounty;
        }
        set
        {
            if (value != _bounty)
            {
                _bounty = value;
                OnPropertyChangedWithValue(value, "Bounty");
            }
        }
    }

    [DataSourceProperty]
    public int PreferredArenaType
    {
        get
        {
            return _preferredArenaType;
        }
        set
        {
            if (value != _preferredArenaType)
            {
                _preferredArenaType = value;
                OnPropertyChangedWithValue(value, "PreferredArenaType");
            }
        }
    }

    [DataSourceProperty]
    public int WSign
    {
        get
        {
            return _wSign;
        }
        set
        {
            if (value != _wSign)
            {
                _wSign = value;
                OnPropertyChangedWithValue(value, "WSign");
            }
        }
    }

    [DataSourceProperty]
    public Vec2 ScreenPosition
    {
        get
        {
            return _screenPosition;
        }
        set
        {
            if (value.x != _screenPosition.x || value.y != _screenPosition.y)
            {
                _screenPosition = value;
                OnPropertyChangedWithValue(value, "ScreenPosition");
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
    public MBBindingList<MPPerkVM> SelectedPerks
    {
        get
        {
            return _selectedPerks;
        }
        set
        {
            if (value != _selectedPerks)
            {
                _selectedPerks = value;
                OnPropertyChangedWithValue(value, "SelectedPerks");
            }
        }
    }

    public CrpgTrainingGroundPeerMarkerVm(MissionPeer peer)
    {
        TargetPeer = peer;
        Bounty = ((CrpgTrainingGroundMissionRepresentative)peer.Representative).Bounty;
        IsEnabled = true;
        TargetIconType iconType = MultiplayerClassDivisions.GetMPHeroClassForPeer(TargetPeer).IconType;
        CompassElement = new MPTeammateCompassTargetVM(iconType, Color.White.ToUnsignedInteger(), Color.White.ToUnsignedInteger(), BannerCode.CreateFrom(new Banner()), isAlly: true);
        SelectedPerks = new MBBindingList<MPPerkVM>();
        RefreshPerkSelection();
        RefreshValues();
    }

    public override void RefreshValues()
    {
        base.RefreshValues();
        Name = TargetPeer.DisplayedName;
        _acceptDuelRequestText = new TextObject("{=tidE1V1k}Accept duel");
        _sendDuelRequestText = new TextObject("{=YLPJWgqF}Challenge");
        _waitingForDuelResponseText = new TextObject("{=MPgnsZoo}Waiting for response");
    }

    public void OnTick(float dt)
    {
        if (Agent.Main != null && TargetPeer.ControlledAgent != null)
        {
            Distance = _latestW;
        }

        if (HasSentDuelRequest)
        {
            _currentDuelRequestTimeRemaining -= dt;
            GameTexts.SetVariable("SECONDS", (int)_currentDuelRequestTimeRemaining);
            GameTexts.SetVariable("ACTION", _waitingForDuelResponseText);
            ActionDescriptionText = new TextObject("{=HXWpxvgT}{ACTION} ({SECONDS})").ToString();
            if (_currentDuelRequestTimeRemaining <= 0f)
            {
                HasSentDuelRequest = false;
            }
        }
    }

    public void UpdateScreenPosition(Camera missionCamera)
    {
        if (TargetPeer.ControlledAgent != null)
        {
            Vec3 groundVec = TargetPeer.ControlledAgent.GetWorldPosition().GetGroundVec3();
            groundVec += new Vec3(0f, 0f, TargetPeer.ControlledAgent.GetEyeGlobalHeight());
            _latestX = 0f;
            _latestY = 0f;
            _latestW = 0f;
            MBWindowManager.WorldToScreen(missionCamera, groundVec, ref _latestX, ref _latestY, ref _latestW);
            ScreenPosition = new Vec2(_latestX, _latestY);
            IsAgentInScreenBoundaries = !(_latestX > Screen.RealScreenResolutionWidth) && !(_latestY > Screen.RealScreenResolutionHeight) && !(_latestX + 200f < 0f) && !(_latestY + 100f < 0f);
            _wPosAfterPositionCalculation = ((_latestW < 0f) ? (-1f) : 1.1f);
            WSign = (int)_wPosAfterPositionCalculation;
        }
    }

    private void OnInteractionChanged()
    {
        ActionDescriptionText = "";
        if (HasDuelRequestForPlayer)
        {
            string keyHyperlinkText = HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13));
            GameTexts.SetVariable("KEY", keyHyperlinkText);
            GameTexts.SetVariable("ACTION", _acceptDuelRequestText);
            ActionDescriptionText = GameTexts.FindText("str_key_action").ToString();
        }
        else if (HasSentDuelRequest)
        {
            _currentDuelRequestTimeRemaining = 10f;
        }
    }

    private void SetFocused(bool isFocused)
    {
        if (!HasDuelRequestForPlayer && !HasSentDuelRequest)
        {
            if (isFocused)
            {
                string keyHyperlinkText = HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13));
                GameTexts.SetVariable("KEY", keyHyperlinkText);
                GameTexts.SetVariable("ACTION", _sendDuelRequestText);
                ActionDescriptionText = GameTexts.FindText("str_key_action").ToString();
            }
            else
            {
                ActionDescriptionText = string.Empty;
            }
        }
    }

    public void UpdateBounty()
    {
        Bounty = ((CrpgTrainingGroundMissionRepresentative)TargetPeer.Representative).Bounty;
    }

    private void UpdateTracked()
    {
        if (!IsEnabled)
        {
            IsTracked = false;
        }
        else if (HasDuelRequestForPlayer || HasSentDuelRequest || IsFocused)
        {
            IsTracked = true;
        }
        else
        {
            IsTracked = false;
        }

        ShouldShowInformation = IsTracked || IsFocused;
    }

    public void OnDuelStarted()
    {
        IsEnabled = false;
        IsInDuel = true;
    }

    public void OnDuelEnded()
    {
        IsEnabled = true;
        IsInDuel = false;
    }

    public void UpdateCurentDuelStatus(bool isInDuel)
    {
        IsInDuel = isInDuel;
        IsEnabled = !IsInDuel;
    }

    public void RefreshPerkSelection()
    {
        SelectedPerks.Clear();
        TargetPeer.RefreshSelectedPerks();
        foreach (MPPerkObject selectedPerk in TargetPeer.SelectedPerks)
        {
            SelectedPerks.Add(new MPPerkVM(null, selectedPerk, isSelectable: true, 0));
        }
    }
}
