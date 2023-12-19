using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Multiplayer.NetworkComponents;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Intermission;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace Crpg.Module.GUI.Intermission;

public class CrpgIntermissionVM : ViewModel
{
    private bool _hasBaseNetworkComponentSet;

    private BaseNetworkComponent _baseNetworkComponent = default!;

    private MultiplayerIntermissionState _currentIntermissionState = default!;

    private readonly TextObject _voteLabelText = new TextObject("{=KOVHgkVq}Voting Ends In:");

    private readonly TextObject _nextGameLabelText = new TextObject("{=lX9Qx7Wo}Next Game Starts In:");

    private readonly TextObject _serverIdleLabelText = new TextObject("{=Rhcberxf}Awaiting Server");

    private readonly TextObject _matchFinishedText = new TextObject("{=RbazQjFt}Match is Finished");

    private readonly TextObject _returningToLobbyText = new TextObject("{=1UaxKbn6}Returning to the Lobby...");

    private MPIntermissionMapItemVM? _votedMapItem;

    private MPIntermissionCultureItemVM? _votedCultureItem;

    private string _connectedPlayersCountValueText = default!;

    private string _maxNumPlayersValueText = default!;

    private bool _isFactionAValid;

    private bool _isFactionBValid;

    private bool _isMissionTimerEnabled;

    private bool _isEndGameTimerEnabled;

    private bool _isNextMapInfoEnabled;

    private bool _isMapVoteEnabled;

    private bool _isCultureVoteEnabled;

    private bool _isPlayerCountEnabled;

    private string _nextMapId = default!;

    private string _nextFactionACultureId = default!;

    private string _nextFactionBCultureId = default!;

    private string _nextGameStateTimerLabel = default!;

    private string _nextGameStateTimerValue = default!;

    private string _playersLabel = default!;

    private string _mapVoteText = default!;

    private string _cultureVoteText = default!;

    private string _serverName = default!;

    private string _welcomeMessage = default!;

    private string _nextGameType = default!;

    private string _nextMapName = default!;

    private Color _nextFactionACultureColor1 = default!;

    private Color _nextFactionACultureColor2 = default!;

    private Color _nextFactionBCultureColor1 = default!;

    private Color _nextFactionBCultureColor2 = default!;

    private string _quitText = default!;

    private MBBindingList<MPIntermissionMapItemVM> _availableMaps = default!;

    private MBBindingList<MPIntermissionCultureItemVM> _availableCultures = default!;

    [DataSourceProperty]
    public string ConnectedPlayersCountValueText
    {
        get
        {
            return _connectedPlayersCountValueText;
        }
        set
        {
            if (value != _connectedPlayersCountValueText)
            {
                _connectedPlayersCountValueText = value;
                OnPropertyChangedWithValue(value, "ConnectedPlayersCountValueText");
            }
        }
    }

    [DataSourceProperty]
    public string MaxNumPlayersValueText
    {
        get
        {
            return _maxNumPlayersValueText;
        }
        set
        {
            if (value != _maxNumPlayersValueText)
            {
                _maxNumPlayersValueText = value;
                OnPropertyChangedWithValue(value, "MaxNumPlayersValueText");
            }
        }
    }

    [DataSourceProperty]
    public bool IsFactionAValid
    {
        get
        {
            return _isFactionAValid;
        }
        set
        {
            if (value != _isFactionAValid)
            {
                _isFactionAValid = value;
                OnPropertyChangedWithValue(value, "IsFactionAValid");
            }
        }
    }

    [DataSourceProperty]
    public bool IsFactionBValid
    {
        get
        {
            return _isFactionBValid;
        }
        set
        {
            if (value != _isFactionBValid)
            {
                _isFactionBValid = value;
                OnPropertyChangedWithValue(value, "IsFactionBValid");
            }
        }
    }

    [DataSourceProperty]
    public bool IsMissionTimerEnabled
    {
        get
        {
            return _isMissionTimerEnabled;
        }
        set
        {
            if (value != _isMissionTimerEnabled)
            {
                _isMissionTimerEnabled = value;
                OnPropertyChangedWithValue(value, "IsMissionTimerEnabled");
            }
        }
    }

    [DataSourceProperty]
    public bool IsEndGameTimerEnabled
    {
        get
        {
            return _isEndGameTimerEnabled;
        }
        set
        {
            if (value != _isEndGameTimerEnabled)
            {
                _isEndGameTimerEnabled = value;
                OnPropertyChangedWithValue(value, "IsEndGameTimerEnabled");
            }
        }
    }

    [DataSourceProperty]
    public bool IsNextMapInfoEnabled
    {
        get
        {
            return _isNextMapInfoEnabled;
        }
        set
        {
            if (value != _isNextMapInfoEnabled)
            {
                _isNextMapInfoEnabled = value;
                OnPropertyChangedWithValue(value, "IsNextMapInfoEnabled");
            }
        }
    }

    [DataSourceProperty]
    public bool IsMapVoteEnabled
    {
        get
        {
            return _isMapVoteEnabled;
        }
        set
        {
            if (value != _isMapVoteEnabled)
            {
                _isMapVoteEnabled = value;
                OnPropertyChangedWithValue(value, "IsMapVoteEnabled");
            }
        }
    }

    [DataSourceProperty]
    public bool IsCultureVoteEnabled
    {
        get
        {
            return _isCultureVoteEnabled;
        }
        set
        {
            if (value != _isCultureVoteEnabled)
            {
                _isCultureVoteEnabled = value;
                OnPropertyChangedWithValue(value, "IsCultureVoteEnabled");
            }
        }
    }

    [DataSourceProperty]
    public bool IsPlayerCountEnabled
    {
        get
        {
            return _isPlayerCountEnabled;
        }
        set
        {
            if (value != _isPlayerCountEnabled)
            {
                _isPlayerCountEnabled = value;
                OnPropertyChangedWithValue(value, "IsPlayerCountEnabled");
            }
        }
    }

    [DataSourceProperty]
    public string NextMapID
    {
        get
        {
            return _nextMapId;
        }
        set
        {
            if (value != _nextMapId)
            {
                _nextMapId = value;
                OnPropertyChangedWithValue(value, "NextMapID");
            }
        }
    }

    [DataSourceProperty]
    public string NextFactionACultureID
    {
        get
        {
            return _nextFactionACultureId;
        }
        set
        {
            if (value != _nextFactionACultureId)
            {
                _nextFactionACultureId = value;
                OnPropertyChangedWithValue(value, "NextFactionACultureID");
            }
        }
    }

    [DataSourceProperty]
    public Color NextFactionACultureColor1
    {
        get
        {
            return _nextFactionACultureColor1;
        }
        set
        {
            if (value != _nextFactionACultureColor1)
            {
                _nextFactionACultureColor1 = value;
                OnPropertyChangedWithValue(value, "NextFactionACultureColor1");
            }
        }
    }

    [DataSourceProperty]
    public Color NextFactionACultureColor2
    {
        get
        {
            return _nextFactionACultureColor2;
        }
        set
        {
            if (value != _nextFactionACultureColor2)
            {
                _nextFactionACultureColor2 = value;
                OnPropertyChangedWithValue(value, "NextFactionACultureColor2");
            }
        }
    }

    [DataSourceProperty]
    public string NextFactionBCultureID
    {
        get
        {
            return _nextFactionBCultureId;
        }
        set
        {
            if (value != _nextFactionBCultureId)
            {
                _nextFactionBCultureId = value;
                OnPropertyChangedWithValue(value, "NextFactionBCultureID");
            }
        }
    }

    [DataSourceProperty]
    public Color NextFactionBCultureColor1
    {
        get
        {
            return _nextFactionBCultureColor1;
        }
        set
        {
            if (value != _nextFactionBCultureColor1)
            {
                _nextFactionBCultureColor1 = value;
                OnPropertyChangedWithValue(value, "NextFactionBCultureColor1");
            }
        }
    }

    [DataSourceProperty]
    public Color NextFactionBCultureColor2
    {
        get
        {
            return _nextFactionBCultureColor2;
        }
        set
        {
            if (value != _nextFactionBCultureColor2)
            {
                _nextFactionBCultureColor2 = value;
                OnPropertyChangedWithValue(value, "NextFactionBCultureColor2");
            }
        }
    }

    [DataSourceProperty]
    public string PlayersLabel
    {
        get
        {
            return _playersLabel;
        }
        set
        {
            if (value != _playersLabel)
            {
                _playersLabel = value;
                OnPropertyChangedWithValue(value, "PlayersLabel");
            }
        }
    }

    [DataSourceProperty]
    public string MapVoteText
    {
        get
        {
            return _mapVoteText;
        }
        set
        {
            if (value != _mapVoteText)
            {
                _mapVoteText = value;
                OnPropertyChangedWithValue(value, "MapVoteText");
            }
        }
    }

    [DataSourceProperty]
    public string CultureVoteText
    {
        get
        {
            return _cultureVoteText;
        }
        set
        {
            if (value != _cultureVoteText)
            {
                _cultureVoteText = value;
                OnPropertyChangedWithValue(value, "CultureVoteText");
            }
        }
    }

    [DataSourceProperty]
    public string NextGameStateTimerLabel
    {
        get
        {
            return _nextGameStateTimerLabel;
        }
        set
        {
            if (value != _nextGameStateTimerLabel)
            {
                _nextGameStateTimerLabel = value;
                OnPropertyChangedWithValue(value, "NextGameStateTimerLabel");
            }
        }
    }

    [DataSourceProperty]
    public string NextGameStateTimerValue
    {
        get
        {
            return _nextGameStateTimerValue;
        }
        set
        {
            if (value != _nextGameStateTimerValue)
            {
                _nextGameStateTimerValue = value;
                OnPropertyChangedWithValue(value, "NextGameStateTimerValue");
            }
        }
    }

    [DataSourceProperty]
    public string WelcomeMessage
    {
        get
        {
            return _welcomeMessage;
        }
        set
        {
            if (value != _welcomeMessage)
            {
                _welcomeMessage = value;
                OnPropertyChangedWithValue(value, "WelcomeMessage");
            }
        }
    }

    [DataSourceProperty]
    public string ServerName
    {
        get
        {
            return _serverName;
        }
        set
        {
            if (value != _serverName)
            {
                _serverName = value;
                OnPropertyChangedWithValue(value, "ServerName");
            }
        }
    }

    [DataSourceProperty]
    public string NextGameType
    {
        get
        {
            return _nextGameType;
        }
        set
        {
            if (value != _nextGameType)
            {
                _nextGameType = value;
                OnPropertyChangedWithValue(value, "NextGameType");
            }
        }
    }

    [DataSourceProperty]
    public string NextMapName
    {
        get
        {
            return _nextMapName;
        }
        set
        {
            if (value != _nextMapName)
            {
                _nextMapName = value;
                OnPropertyChangedWithValue(value, "NextMapName");
            }
        }
    }

    [DataSourceProperty]
    public MBBindingList<MPIntermissionMapItemVM> AvailableMaps
    {
        get
        {
            return _availableMaps;
        }
        set
        {
            if (value != _availableMaps)
            {
                _availableMaps = value;
                OnPropertyChangedWithValue(value, "AvailableMaps");
            }
        }
    }

    [DataSourceProperty]
    public MBBindingList<MPIntermissionCultureItemVM> AvailableCultures
    {
        get
        {
            return _availableCultures;
        }
        set
        {
            if (value != _availableCultures)
            {
                _availableCultures = value;
                OnPropertyChangedWithValue(value, "AvailableCultures");
            }
        }
    }

    [DataSourceProperty]
    public string QuitText
    {
        get
        {
            return _quitText;
        }
        set
        {
            if (value != _quitText)
            {
                _quitText = value;
                OnPropertyChangedWithValue(value, "QuitText");
            }
        }
    }

    public CrpgIntermissionVM()
    {
        AvailableMaps = new MBBindingList<MPIntermissionMapItemVM>();
        AvailableCultures = new MBBindingList<MPIntermissionCultureItemVM>();
        RefreshValues();
    }

    public override void RefreshValues()
    {
        QuitText = new TextObject("{=3sRdGQou}Leave").ToString();
        PlayersLabel = new TextObject("{=RfXJdNye}Players").ToString();
        MapVoteText = new TextObject("{=DraJ6bxq}Vote for the Next Map").ToString();
        CultureVoteText = new TextObject("{=oF27vprQ}Vote for the Next Culture").ToString();
    }

    public void Tick()
    {
        if (!_hasBaseNetworkComponentSet)
        {
            _baseNetworkComponent = GameNetwork.GetNetworkComponent<BaseNetworkComponent>();
            if (_baseNetworkComponent != null)
            {
                _hasBaseNetworkComponentSet = true;
                BaseNetworkComponent baseNetworkComponent = _baseNetworkComponent;
                baseNetworkComponent.OnIntermissionStateUpdated = (Action)Delegate.Combine(baseNetworkComponent.OnIntermissionStateUpdated, new Action(OnIntermissionStateUpdated));
            }
        }
        else if (_baseNetworkComponent.ClientIntermissionState == MultiplayerIntermissionState.Idle)
        {
            NextGameStateTimerLabel = _serverIdleLabelText.ToString();
            NextGameStateTimerValue = string.Empty;
            IsMissionTimerEnabled = false;
            IsEndGameTimerEnabled = false;
            IsNextMapInfoEnabled = false;
            IsMapVoteEnabled = false;
            IsCultureVoteEnabled = false;
            IsPlayerCountEnabled = false;
        }
    }

    public override void OnFinalize()
    {
        if (_baseNetworkComponent != null)
        {
            BaseNetworkComponent baseNetworkComponent = _baseNetworkComponent;
            baseNetworkComponent.OnIntermissionStateUpdated = (Action)Delegate.Remove(baseNetworkComponent.OnIntermissionStateUpdated, new Action(OnIntermissionStateUpdated));
        }

        MultiplayerIntermissionVotingManager.Instance.ClearItems();
    }

    private void OnIntermissionStateUpdated()
    {
        _currentIntermissionState = _baseNetworkComponent.ClientIntermissionState;

        bool flag = true;
        if (_currentIntermissionState == MultiplayerIntermissionState.CountingForMapVote)
        {
            int num = (int)_baseNetworkComponent.CurrentIntermissionTimer;
            NextGameStateTimerLabel = _voteLabelText.ToString();
            NextGameStateTimerValue = num.ToString();
            IsMissionTimerEnabled = true;
            IsEndGameTimerEnabled = false;
            IsNextMapInfoEnabled = false;
            IsCultureVoteEnabled = false;
            IsPlayerCountEnabled = true;
            flag = false;
            List<IntermissionVoteItem> mapVoteItems = MultiplayerIntermissionVotingManager.Instance.MapVoteItems;
            if (mapVoteItems.Count > 0)
            {
                IsMapVoteEnabled = true;
                foreach (IntermissionVoteItem mapItem in mapVoteItems)
                {
                    if (AvailableMaps.FirstOrDefault((MPIntermissionMapItemVM m) => m.MapID == mapItem.Id) == null)
                    {
                        AvailableMaps.Add(new MPIntermissionMapItemVM(mapItem.Id, OnPlayerVotedForMap));
                    }

                    int voteCount = mapItem.VoteCount;
                    AvailableMaps.First((MPIntermissionMapItemVM m) => m.MapID == mapItem.Id).Votes = voteCount;
                }
            }
        }

        if (_baseNetworkComponent.ClientIntermissionState == MultiplayerIntermissionState.CountingForCultureVote)
        {
            int num2 = (int)_baseNetworkComponent.CurrentIntermissionTimer;
            NextGameStateTimerLabel = _voteLabelText.ToString();
            NextGameStateTimerValue = num2.ToString();
            IsMissionTimerEnabled = true;
            IsEndGameTimerEnabled = false;
            IsNextMapInfoEnabled = false;
            IsMapVoteEnabled = false;
            IsPlayerCountEnabled = true;
            flag = false;
            List<IntermissionVoteItem> cultureVoteItems = MultiplayerIntermissionVotingManager.Instance.CultureVoteItems;
            if (cultureVoteItems.Count > 0)
            {
                IsCultureVoteEnabled = true;
                foreach (IntermissionVoteItem cultureItem in cultureVoteItems)
                {
                    if (AvailableCultures.FirstOrDefault((MPIntermissionCultureItemVM c) => c.CultureCode == cultureItem.Id) == null)
                    {
                        AvailableCultures.Add(new MPIntermissionCultureItemVM(cultureItem.Id, OnPlayerVotedForCulture));
                    }

                    int voteCount2 = cultureItem.VoteCount;
                    AvailableCultures.FirstOrDefault((MPIntermissionCultureItemVM c) => c.CultureCode == cultureItem.Id).Votes = voteCount2;
                }
            }

            MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.CultureTeam1).GetValue(out string value);
            MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.CultureTeam2).GetValue(out string value2);
            NextFactionACultureID = value;
            BasicCultureObject @object = MBObjectManager.Instance.GetObject<BasicCultureObject>(NextFactionACultureID);
            NextFactionACultureColor1 = Color.FromUint(@object?.Color ?? 0);
            NextFactionACultureColor2 = Color.FromUint(@object?.Color2 ?? 0);
            NextFactionBCultureID = value2;
            BasicCultureObject object2 = MBObjectManager.Instance.GetObject<BasicCultureObject>(NextFactionBCultureID);
            NextFactionBCultureColor1 = Color.FromUint(object2?.Color2 ?? 0);
            NextFactionBCultureColor2 = Color.FromUint(object2?.Color ?? 0);
        }

        if (_currentIntermissionState == MultiplayerIntermissionState.CountingForMission)
        {
            int num3 = (int)_baseNetworkComponent.CurrentIntermissionTimer;
            NextGameStateTimerLabel = _nextGameLabelText.ToString();
            NextGameStateTimerValue = num3.ToString();
            IsMissionTimerEnabled = true;
            IsEndGameTimerEnabled = false;
            IsNextMapInfoEnabled = true;
            IsMapVoteEnabled = false;
            IsCultureVoteEnabled = false;
            IsPlayerCountEnabled = true;
            flag = true;
            AvailableMaps.Clear();
            AvailableCultures.Clear();
            MultiplayerIntermissionVotingManager.Instance.ClearVotes();
            _votedMapItem = null;
            _votedCultureItem = null;
        }

        if (_currentIntermissionState == MultiplayerIntermissionState.CountingForEnd)
        {
            TextObject textObject = GameTexts.FindText("str_string_newline_string");
            textObject.SetTextVariable("STR1", _matchFinishedText.ToString());
            textObject.SetTextVariable("STR2", _returningToLobbyText.ToString());
            NextGameStateTimerLabel = textObject.ToString();
            NextGameStateTimerValue = string.Empty;
            IsMissionTimerEnabled = false;
            IsEndGameTimerEnabled = false;
            IsNextMapInfoEnabled = false;
            IsMapVoteEnabled = false;
            IsCultureVoteEnabled = false;
            IsPlayerCountEnabled = false;
            flag = false;
        }

        MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.Map).GetValue(out string value3);
        NextMapID = (IsEndGameTimerEnabled ? string.Empty : value3);
        TextObject textObject2;
        string text = ((!GameTexts.TryGetText("str_multiplayer_scene_name", out textObject2, value3)) ? value3 : textObject2.ToString());
        NextMapName = (IsEndGameTimerEnabled ? string.Empty : text);
        if (flag)
        {
            MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.CultureTeam1).GetValue(out string value4);
            IsFactionAValid = !IsEndGameTimerEnabled && !string.IsNullOrEmpty(value4) && _currentIntermissionState != MultiplayerIntermissionState.CountingForMapVote;
            NextFactionACultureID = (IsEndGameTimerEnabled ? string.Empty : value4);
            if (!string.IsNullOrEmpty(NextFactionACultureID))
            {
                BasicCultureObject object3 = MBObjectManager.Instance.GetObject<BasicCultureObject>(NextFactionACultureID);
                NextFactionACultureColor1 = Color.FromUint(object3?.Color ?? 0);
                NextFactionACultureColor2 = Color.FromUint(object3?.Color2 ?? 0);
            }

            MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.CultureTeam2).GetValue(out string value5);
            IsFactionBValid = !IsEndGameTimerEnabled && !string.IsNullOrEmpty(value5) && _currentIntermissionState != MultiplayerIntermissionState.CountingForMapVote;
            NextFactionBCultureID = (IsEndGameTimerEnabled ? string.Empty : value5);
            if (!string.IsNullOrEmpty(NextFactionBCultureID))
            {
                BasicCultureObject object4 = MBObjectManager.Instance.GetObject<BasicCultureObject>(NextFactionBCultureID);
                NextFactionBCultureColor1 = Color.FromUint(object4?.Color2 ?? 0);
                NextFactionBCultureColor2 = Color.FromUint(object4?.Color ?? 0);
            }
        }
        else
        {
            IsFactionAValid = false;
            IsFactionBValid = false;
        }

        MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.ServerName).GetValue(out string value6);
        ServerName = value6;
        MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.GameType).GetValue(out string value7);
        NextGameType = (IsEndGameTimerEnabled ? string.Empty : GameTexts.FindText("str_multiplayer_game_type", value7).ToString());
        MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.WelcomeMessage).GetValue(out string value8);
        WelcomeMessage = (IsEndGameTimerEnabled ? string.Empty : value8);
        MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.MaxNumberOfPlayers).GetValue(out int value9);
        MaxNumPlayersValueText = value9.ToString();
        ConnectedPlayersCountValueText = GameNetwork.NetworkPeers.Count.ToString();
    }

    public void ExecuteQuitServer()
    {
        LobbyClient gameClient = NetworkMain.GameClient;
        if (gameClient.CurrentState == LobbyClient.State.InCustomGame)
        {
            gameClient.QuitFromCustomGame();
        }

        MultiplayerIntermissionVotingManager.Instance.ClearItems();
    }

    private void OnPlayerVotedForMap(MPIntermissionMapItemVM mapItem)
    {
        if (_votedMapItem != null)
        {
            _baseNetworkComponent.IntermissionCastVote(_votedMapItem.MapID, -1);
            _votedMapItem.IsSelected = false;
            _votedMapItem.Votes--;
        }

        _baseNetworkComponent.IntermissionCastVote(mapItem.MapID, 1);
        _votedMapItem = mapItem;
        _votedMapItem.IsSelected = true;
        _votedMapItem.Votes++;
    }

    private void OnPlayerVotedForCulture(MPIntermissionCultureItemVM cultureItem)
    {
        if (_votedCultureItem != null)
        {
            _baseNetworkComponent.IntermissionCastVote(_votedCultureItem.CultureCode, -1);
            _votedCultureItem.IsSelected = false;
            _votedCultureItem.Votes--;
        }

        _baseNetworkComponent.IntermissionCastVote(cultureItem.CultureCode, 1);
        _votedCultureItem = cultureItem;
        _votedCultureItem.IsSelected = true;
        _votedCultureItem.Votes++;
    }
}

