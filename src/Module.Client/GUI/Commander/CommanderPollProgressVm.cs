using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace Crpg.Module.GUI.Commander;

public class CommanderPollProgressVm : ViewModel
{
    private static readonly TextObject _promoteText = new("{=}promote");
    private static readonly TextObject _demoteText = new("{=}demote");
    private bool _hasOngoingPoll;
    private bool _areKeysEnabled;
    private int _votesAccepted;
    private int _votesRejected;
    private string _pollInitiatorName = string.Empty;
    private string _pollDescription = string.Empty;
    private MPPlayerVM _targetPlayer = default!;
    private MBBindingList<InputKeyItemVM> _keys = new();

    [DataSourceProperty]
    public bool HasOngoingPoll
    {
        get
        {
            return _hasOngoingPoll;
        }
        set
        {
            if (value != _hasOngoingPoll)
            {
                _hasOngoingPoll = value;
                OnPropertyChangedWithValue(value, "HasOngoingPoll");
            }
        }
    }

    [DataSourceProperty]
    public bool AreKeysEnabled
    {
        get
        {
            return _areKeysEnabled;
        }
        set
        {
            if (value != _areKeysEnabled)
            {
                _areKeysEnabled = value;
                OnPropertyChangedWithValue(value, "AreKeysEnabled");
            }
        }
    }

    [DataSourceProperty]
    public int VotesAccepted
    {
        get
        {
            return _votesAccepted;
        }
        set
        {
            if (_votesAccepted != value)
            {
                _votesAccepted = value;
                OnPropertyChangedWithValue(value, "VotesAccepted");
            }
        }
    }

    [DataSourceProperty]
    public int VotesRejected
    {
        get
        {
            return _votesRejected;
        }
        set
        {
            if (_votesRejected != value)
            {
                _votesRejected = value;
                OnPropertyChangedWithValue(value, "VotesRejected");
            }
        }
    }

    [DataSourceProperty]
    public string PollInitiatorName
    {
        get
        {
            return _pollInitiatorName;
        }
        set
        {
            if (_pollInitiatorName != value)
            {
                _pollInitiatorName = value;
                OnPropertyChangedWithValue(value, "PollInitiatorName");
            }
        }
    }

    [DataSourceProperty]
    public string PollDescription
    {
        get
        {
            return _pollDescription;
        }
        set
        {
            if (_pollDescription != value)
            {
                _pollDescription = value;
                OnPropertyChangedWithValue(value, "PollDescription");
            }
        }
    }

    [DataSourceProperty]
    public MPPlayerVM TargetPlayer
    {
        get
        {
            return _targetPlayer;
        }
        set
        {
            if (value != _targetPlayer)
            {
                _targetPlayer = value;
                OnPropertyChangedWithValue(value, "TargetPlayer");
            }
        }
    }

    [DataSourceProperty]
    public MBBindingList<InputKeyItemVM> Keys
    {
        get
        {
            return _keys;
        }
        set
        {
            if (_keys != value)
            {
                _keys = value;
                OnPropertyChangedWithValue(value, "Keys");
            }
        }
    }

    public void OnCommanderPollOpened(MissionPeer initiatorPeer, MissionPeer targetPeer, bool isDemoteRequested)
    {
        TargetPlayer = new MPPlayerVM(targetPeer);
        PollInitiatorName = initiatorPeer.DisplayedName;
        GameTexts.SetVariable("ACTION", isDemoteRequested ? _demoteText : _promoteText);
        PollDescription = new TextObject("{=qyuhC21P}wants to {ACTION}").ToString();
        VotesAccepted = 0;
        VotesRejected = 0;
        AreKeysEnabled = NetworkMain.GameClient.PlayerID != targetPeer.Peer.Id;
        HasOngoingPoll = true;
    }

    public void OnPollUpdated(int votesAccepted, int votesRejected)
    {
        VotesAccepted = votesAccepted;
        VotesRejected = votesRejected;
    }

    public void OnPollClosed()
    {
        HasOngoingPoll = false;
    }

    public void OnPollOptionPicked()
    {
        AreKeysEnabled = false;
    }

    public void AddKey(GameKey key)
    {
        Keys.Add(InputKeyItemVM.CreateFromGameKey(key, isConsoleOnly: false));
    }
}
