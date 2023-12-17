using System;
using System.ComponentModel;
using Crpg.Module.Api.Models.Clans;
using Crpg.Module.Common;
using Crpg.Module.Helpers;
using Crpg.Module.Modes.Conquest;
using Crpg.Module.Modes.Siege;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.HUDExtensions;
using TaleWorlds.ObjectSystem;

namespace Crpg.Module.GUI.HudExtension;

internal class CrpgHudExtensionVm : ViewModel
{
    private const float RemainingTimeWarningThreshold = 5f;
    private readonly Mission _mission;
    private readonly Dictionary<MissionPeer, MPPlayerVM> _teammateDictionary;
    private readonly Dictionary<MissionPeer, MPPlayerVM> _enemyDictionary;
    private readonly MissionScoreboardComponent _missionScoreboardComponent;
    private readonly MissionMultiplayerGameModeBaseClient _gameMode;
    private readonly bool _isTeamScoresEnabled;
    private bool _isAttackerTeamAlly;
    private bool _isTeammateAndEnemiesRelevant;
    private bool _isOrderActive;
    private CrpgCommanderInfoVm? _commanderInfo;
    private MissionMultiplayerSpectatorHUDVM? _spectatorControls;
    private bool _warnRemainingTime;
    private bool _isRoundCountdownAvailable;
    private bool _isRoundCountdownSuspended;
    private bool _showTeamScores;
    private string? _remainingRoundTime;
    private string? _allyTeamColor;
    private string? _allyTeamColor2;
    private string? _enemyTeamColor;
    private string? _enemyTeamColor2;
    private string? _warmupInfoText;
    private int _allyTeamScore = -1;
    private int _enemyTeamScore = -1;
    private MBBindingList<MPPlayerVM> _teammatesList = default!;
    private MBBindingList<MPPlayerVM> _enemiesList = default!;
    private bool _showHud;
    private bool _showCommanderInfo;
    private bool _showPowerLevels;
    private bool _isInWarmup;
    private int _generalWarningCountdown;
    private bool _isGeneralWarningCountdownActive;
    private ImageIdentifierVM? _allyBanner;
    private ImageIdentifierVM? _enemyBanner;

    public CrpgHudExtensionVm(Mission mission)
    {
        _mission = mission;
        _missionScoreboardComponent = mission.GetMissionBehavior<MissionScoreboardComponent>();
        _gameMode = _mission.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
        SpectatorControls = new MissionMultiplayerSpectatorHUDVM(_mission);
        if (_gameMode.RoundComponent != null)
        {
            _gameMode.RoundComponent.OnCurrentRoundStateChanged += OnCurrentGameModeStateChanged;
        }

        if (_gameMode.WarmupComponent != null)
        {
            _gameMode.WarmupComponent.OnWarmupEnded += OnCurrentGameModeStateChanged;
        }

        _missionScoreboardComponent.OnRoundPropertiesChanged += UpdateTeamScores;
        MissionPeer.OnTeamChanged += OnTeamChanged;
        NetworkCommunicator.OnPeerComponentAdded += OnPeerComponentAdded;
        _mission.OnMissionReset += OnMissionReset;
        MissionLobbyComponent missionBehavior = mission.GetMissionBehavior<MissionLobbyComponent>();
        bool isTeamsEnabled = missionBehavior.MissionType != MultiplayerGameType.FreeForAll
                              && missionBehavior.MissionType != MultiplayerGameType.Duel;
        IsRoundCountdownAvailable = _gameMode.IsGameModeUsingRoundCountdown;
        IsRoundCountdownSuspended = false;
        _isTeamScoresEnabled = isTeamsEnabled;
        UpdateShowTeamScores();
        Teammates = new MBBindingList<MPPlayerVM>();
        Enemies = new MBBindingList<MPPlayerVM>();
        _teammateDictionary = new Dictionary<MissionPeer, MPPlayerVM>();
        _enemyDictionary = new Dictionary<MissionPeer, MPPlayerVM>();
        ShowHud = true;
        RefreshValues();
    }

    [DataSourceProperty]
    public bool IsOrderActive
    {
        get
        {
            return _isOrderActive;
        }
        set
        {
            if (value == _isOrderActive)
            {
                return;
            }

            _isOrderActive = value;
            OnPropertyChangedWithValue(value);
        }
    }

    [DataSourceProperty]
    public CrpgCommanderInfoVm? CommanderInfo
    {
        get
        {
            return _commanderInfo;
        }
        set
        {
            if (value == _commanderInfo)
            {
                return;
            }

            _commanderInfo = value;
            OnPropertyChangedWithValue(value);
        }
    }

    [DataSourceProperty]
    public MissionMultiplayerSpectatorHUDVM? SpectatorControls
    {
        get
        {
            return _spectatorControls;
        }
        set
        {
            if (value == _spectatorControls)
            {
                return;
            }

            _spectatorControls = value;
            OnPropertyChangedWithValue(value);
        }
    }

    [DataSourceProperty]
    public MBBindingList<MPPlayerVM> Teammates
    {
        get
        {
            return _teammatesList;
        }
        set
        {
            if (value == _teammatesList)
            {
                return;
            }

            _teammatesList = value;
            OnPropertyChangedWithValue(value);
        }
    }

    [DataSourceProperty]
    public MBBindingList<MPPlayerVM> Enemies
    {
        get
        {
            return _enemiesList;
        }
        set
        {
            if (value == _enemiesList)
            {
                return;
            }

            _enemiesList = value;
            OnPropertyChangedWithValue(value);
        }
    }

    [DataSourceProperty]
    public ImageIdentifierVM? AllyBanner
    {
        get
        {
            return _allyBanner;
        }
        set
        {
            if (value == _allyBanner)
            {
                return;
            }

            _allyBanner = value;
            OnPropertyChangedWithValue(value);
        }
    }

    [DataSourceProperty]
    public ImageIdentifierVM? EnemyBanner
    {
        get
        {
            return _enemyBanner;
        }
        set
        {
            if (value == _enemyBanner)
            {
                return;
            }

            _enemyBanner = value;
            OnPropertyChangedWithValue(value);
        }
    }

    [DataSourceProperty]
    public bool IsRoundCountdownAvailable
    {
        get
        {
            return _isRoundCountdownAvailable;
        }
        set
        {
            if (value == _isRoundCountdownAvailable)
            {
                return;
            }

            _isRoundCountdownAvailable = value;
            OnPropertyChangedWithValue(value);
        }
    }

    [DataSourceProperty]
    public bool IsRoundCountdownSuspended
    {
        get
        {
            return _isRoundCountdownSuspended;
        }
        set
        {
            if (value == _isRoundCountdownSuspended)
            {
                return;
            }

            _isRoundCountdownSuspended = value;
            OnPropertyChangedWithValue(value);
        }
    }

    [DataSourceProperty]
    public bool ShowTeamScores
    {
        get
        {
            return _showTeamScores;
        }
        set
        {
            if (value == _showTeamScores)
            {
                return;
            }

            _showTeamScores = value;
            OnPropertyChangedWithValue(value);
        }
    }

    [DataSourceProperty]
    public string? RemainingRoundTime
    {
        get
        {
            return _remainingRoundTime;
        }
        set
        {
            if (value == _remainingRoundTime)
            {
                return;
            }

            _remainingRoundTime = value;
            OnPropertyChangedWithValue(value);
        }
    }

    [DataSourceProperty]
    public bool WarnRemainingTime
    {
        get
        {
            return _warnRemainingTime;
        }
        set
        {
            if (value == _warnRemainingTime)
            {
                return;
            }

            _warnRemainingTime = value;
            OnPropertyChangedWithValue(value);
        }
    }

    [DataSourceProperty]
    public int AllyTeamScore
    {
        get
        {
            return _allyTeamScore;
        }
        set
        {
            if (value == _allyTeamScore)
            {
                return;
            }

            _allyTeamScore = value;
            OnPropertyChangedWithValue(value);
        }
    }

    [DataSourceProperty]
    public int EnemyTeamScore
    {
        get
        {
            return _enemyTeamScore;
        }
        set
        {
            if (value == _enemyTeamScore)
            {
                return;
            }

            _enemyTeamScore = value;
            OnPropertyChangedWithValue(value);
        }
    }

    [DataSourceProperty]
    public string? AllyTeamColor
    {
        get
        {
            return _allyTeamColor;
        }
        set
        {
            if (value == _allyTeamColor)
            {
                return;
            }

            _allyTeamColor = value;
            OnPropertyChangedWithValue(value);
        }
    }

    [DataSourceProperty]
    public string? AllyTeamColor2
    {
        get
        {
            return _allyTeamColor2;
        }
        set
        {
            if (value == _allyTeamColor2)
            {
                return;
            }

            _allyTeamColor2 = value;
            OnPropertyChangedWithValue(value);
        }
    }

    [DataSourceProperty]
    public string? EnemyTeamColor
    {
        get
        {
            return _enemyTeamColor;
        }
        set
        {
            if (value == _enemyTeamColor)
            {
                return;
            }

            _enemyTeamColor = value;
            OnPropertyChangedWithValue(value);
        }
    }

    [DataSourceProperty]
    public string? EnemyTeamColor2
    {
        get
        {
            return _enemyTeamColor2;
        }
        set
        {
            if (value == _enemyTeamColor2)
            {
                return;
            }

            _enemyTeamColor2 = value;
            OnPropertyChangedWithValue(value);
        }
    }

    [DataSourceProperty]
    public bool ShowHud
    {
        get
        {
            return _showHud;
        }
        set
        {
            if (value == _showHud)
            {
                return;
            }

            _showHud = value;
            OnPropertyChangedWithValue(value);
        }
    }

    [DataSourceProperty]
    public bool ShowCommanderInfo
    {
        get
        {
            return _showCommanderInfo;
        }
        set
        {
            if (value == _showCommanderInfo)
            {
                return;
            }

            _showCommanderInfo = value;
            OnPropertyChangedWithValue(value);
            UpdateShowTeamScores();
        }
    }

    [DataSourceProperty]
    public bool ShowPowerLevels
    {
        get
        {
            return _showPowerLevels;
        }
        set
        {
            if (value == _showPowerLevels)
            {
                return;
            }

            _showPowerLevels = value;
            OnPropertyChangedWithValue(value);
        }
    }

    [DataSourceProperty]
    public bool IsInWarmup
    {
        get
        {
            return _isInWarmup;
        }
        set
        {
            if (value == _isInWarmup)
            {
                return;
            }

            _isInWarmup = value;
            OnPropertyChangedWithValue(value);
            UpdateShowTeamScores();
            CommanderInfo?.UpdateWarmupDependentFlags(_isInWarmup);
        }
    }

    [DataSourceProperty]
    public string? WarmupInfoText
    {
        get
        {
            return _warmupInfoText;
        }
        set
        {
            if (value == _warmupInfoText)
            {
                return;
            }

            _warmupInfoText = value;
            OnPropertyChangedWithValue(value);
        }
    }

    [DataSourceProperty]
    public int GeneralWarningCountdown
    {
        get
        {
            return _generalWarningCountdown;
        }
        set
        {
            if (value == _generalWarningCountdown)
            {
                return;
            }

            _generalWarningCountdown = value;
            OnPropertyChangedWithValue(value);
        }
    }

    [DataSourceProperty]
    public bool IsGeneralWarningCountdownActive
    {
        get
        {
            return _isGeneralWarningCountdownActive;
        }
        set
        {
            if (value == _isGeneralWarningCountdownActive)
            {
                return;
            }

            _isGeneralWarningCountdownActive = value;
            OnPropertyChangedWithValue(value);
        }
    }

    public override void RefreshValues()
    {
        base.RefreshValues();
        string strValue = MultiplayerOptions.OptionType.GameType.GetStrValue();
        TextObject textObject = new("{=XJTX8w8M}Warmup Phase - {GAME_MODE}\nWaiting for players to join");
        textObject.SetTextVariable("GAME_MODE", GameTexts.FindText("str_multiplayer_official_game_type_name", strValue));
        WarmupInfoText = textObject.ToString();
        UpdateTeamBanners(out ImageIdentifierVM? allyBanner, out ImageIdentifierVM? enemyBanner, out _, out _);
        AllyBanner = allyBanner;
        EnemyBanner = enemyBanner;
        SpectatorControls!.RefreshValues();
    }

    public override void OnFinalize()
    {
        MissionPeer.OnTeamChanged -= OnTeamChanged;
        if (_gameMode.RoundComponent != null)
        {
            _gameMode.RoundComponent.OnCurrentRoundStateChanged -= OnCurrentGameModeStateChanged;
        }

        if (_gameMode.WarmupComponent != null)
        {
            _gameMode.WarmupComponent.OnWarmupEnded -= OnCurrentGameModeStateChanged;
        }

        _missionScoreboardComponent.OnRoundPropertiesChanged -= UpdateTeamScores;
        NetworkCommunicator.OnPeerComponentAdded -= OnPeerComponentAdded;
        CommanderInfo?.OnFinalize();
        CommanderInfo = null;
        SpectatorControls?.OnFinalize();
        SpectatorControls = null;
        base.OnFinalize();
        UpdateTeamBanners(out ImageIdentifierVM? allyBanner, out ImageIdentifierVM? enemyBanner, out _, out _);
        AllyBanner = allyBanner;
        EnemyBanner = enemyBanner;
    }

    public void Tick(float dt)
    {
        IsInWarmup = _gameMode.IsInWarmup;
        CheckTimers();
        if (_isTeammateAndEnemiesRelevant)
        {
            OnRefreshTeamMembers();
            OnRefreshEnemyMembers();
        }

        _commanderInfo?.Tick(dt);
        _spectatorControls?.Tick(dt);
    }

    public void OnSpectatedAgentFocusIn(Agent followedAgent)
    {
        if (_spectatorControls != null)
        {
            ReflectionHelper.InvokeMethod(_spectatorControls, "OnSpectatedAgentFocusIn", new object[] { followedAgent });
        }
    }

    public void OnSpectatedAgentFocusOut(Agent followedPeer)
    {
        if (_spectatorControls != null)
        {
            ReflectionHelper.InvokeMethod(_spectatorControls, "OnSpectatedAgentFocusOut", new object[] { followedPeer });
        }
    }

    public static async Task<TeamsBannerAndNameModel> UpdateTeamBanners(bool byTeamSide = false)
    {
        Task.WhenAll()
        var allyOrAttackerTeamBanner = ResolveTeamBannerKey(allyTeamOrAttackerTeam: true, byTeamSide);
        var enemyOrDefenderTeamBanner = ResolveTeamBannerKey(allyTeamOrAttackerTeam: false, byTeamSide);
        var allyOrAttackerTeamBannerCode = BannerCode.CreateFrom(allyOrAttackerTeamBanner.Result.Item1);
        var enemyOrDefenderTeamBannerCode = BannerCode.CreateFrom(enemyOrDefenderTeamBanner.Result.Item1);
        ImageIdentifierVM allyOrDefenderImageId = new(allyOrAttackerTeamBannerCode, true);
        ImageIdentifierVM enemyOrAttackerImageId = new(enemyOrDefenderTeamBannerCode, true);

        allyBannerOrDefenderTeamBannerVM = allyOrDefenderImageId;
        enemyBannerOrAttackerTeamBannerVM = enemyOrAttackerImageId;
    }

    public async static Task<(Banner?, string)> ResolveTeamBannerKey(bool allyTeamOrAttackerTeam,  bool byTeamSide = false)
    {
        if (Mission.Current.Teams.Count == 0)
        {
            return (null, string.Empty);
        }

        Dictionary<int, (int count, CrpgClan clan)> clanNumber = new();
        var myMissionPeer = GameNetwork.MyPeer?.GetComponent<MissionPeer>();
        Team myTeam = (myMissionPeer?.Team?.TeamIndex ?? 0) == 0
            ? Mission.Current.Teams.Attacker
            : GameNetwork.MyPeer.GetComponent<MissionPeer>().Team;
        Team enemyTeam = myTeam.TeamIndex == 0
            ? Mission.Current.Teams.Defender
            : Mission.Current.Teams.First(t => t.TeamIndex != myTeam.TeamIndex && t.TeamIndex != 0);

        foreach (var networkPeer in GameNetwork.NetworkPeers)
        {
            var crpgPeer = networkPeer.GetComponent<CrpgPeer>();
            var missionPeer = networkPeer.GetComponent<MissionPeer>();

            if (missionPeer == null || crpgPeer?.User == null || crpgPeer?.Clan == null || (missionPeer?.Team?.TeamIndex ?? 0) == 0)
            {
                continue;
            }

            bool isAlliedOrIsTeam1 = byTeamSide
                ? missionPeer!.Team == Mission.Current.Teams.Attacker
                : missionPeer!.Team == myTeam;
            bool isEnemyOrTeam2 = byTeamSide
                ? missionPeer.Team == Mission.Current.Teams.Defender
                : missionPeer.Team != myTeam;
            bool isSelected = allyTeamOrAttackerTeam
                ? isAlliedOrIsTeam1
                : isEnemyOrTeam2;

            if (!isSelected)
            {
                continue;
            }

            int peerClanId = crpgPeer!.Clan!.Id;

            if (clanNumber.ContainsKey(peerClanId))
            {
                var clan = clanNumber[peerClanId];
                clan.count++;
                clanNumber[peerClanId] = clan;
            }
            else
            {
                clanNumber.Add(peerClanId, (1, crpgPeer.Clan));
            }
        }

        // TODO: ordering the dictionary is unnecessary, we just want the Tuple with the max count. Eventually look for a better way.
        var maxClan = clanNumber.OrderByDescending(c => c.Value.count).FirstOrDefault();

        if (maxClan.Value.clan == null)
        {
            string team1Name = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)).Name.ToString();
            string team2Name = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)).Name.ToString();
            if (byTeamSide)
            {
                if (allyTeamOrAttackerTeam)
                {
                    return (Mission.Current.Teams.Attacker.Banner, team1Name);
                }
                else
                {
                    return (Mission.Current.Teams.Defender.Banner, team2Name);
                }
            }
            else
            {
                if (allyTeamOrAttackerTeam)
                {
                    if (myTeam.IsAttacker)
                    {
                        return (myTeam.Banner, team1Name);
                    }
                    else
                    {
                        return (myTeam.Banner, team2Name);
                    }

                }
                else
                {
                    if (myTeam.IsAttacker)
                    {

                        return (enemyTeam.Banner, team2Name);
                    }
                    else
                    {
                        return (enemyTeam.Banner, team1Name);
                    }
                }
            }
        }

        return (new Banner(maxClan.Value.clan.BannerKey, maxClan.Value.clan.PrimaryColor, maxClan.Value.clan.SecondaryColor), maxClan.Value.clan.Name);
    }

    private void OnMissionReset(object sender, PropertyChangedEventArgs e)
    {
        IsGeneralWarningCountdownActive = false;
        UpdateTeamBanners(out ImageIdentifierVM? allyBanner, out ImageIdentifierVM? enemyBanner, out _, out _);
        AllyBanner = allyBanner;
        EnemyBanner = enemyBanner;
    }

    private void OnPeerComponentAdded(PeerComponent component)
    {
        if (!component.IsMine || component is not MissionRepresentativeBase)
        {
            return;
        }

        AllyTeamScore = _missionScoreboardComponent.GetRoundScore(BattleSideEnum.Attacker);
        EnemyTeamScore = _missionScoreboardComponent.GetRoundScore(BattleSideEnum.Defender);
        _isTeammateAndEnemiesRelevant =
            _mission.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>().IsGameModeTactical
            && !_mission.HasMissionBehavior<CrpgSiegeClient>()
            && _gameMode.GameType != MultiplayerGameType.Battle;
        CommanderInfo = new CrpgCommanderInfoVm();
        ShowCommanderInfo = !_mission.HasMissionBehavior<CrpgConquestClient>();
        if (_isTeammateAndEnemiesRelevant)
        {
            OnRefreshTeamMembers();
            OnRefreshEnemyMembers();
        }

        ShowPowerLevels = _gameMode.GameType == MultiplayerGameType.Battle;
        UpdateTeamBanners(out ImageIdentifierVM? allyBanner, out ImageIdentifierVM? enemyBanner, out _, out _);
        AllyBanner = allyBanner;
        EnemyBanner = enemyBanner;
    }

    private void CheckTimers(bool forceUpdate = false)
    {
        if (!_gameMode.CheckTimer(out int remainingTime, out int remainingWarningTime, forceUpdate))
        {
            return;
        }

        RemainingRoundTime = TimeSpan.FromSeconds(remainingTime).ToString("mm':'ss");
        WarnRemainingTime = remainingTime <= RemainingTimeWarningThreshold;
        if (GeneralWarningCountdown == remainingWarningTime)
        {
            return;
        }

        IsGeneralWarningCountdownActive = remainingWarningTime > 0;
        GeneralWarningCountdown = remainingWarningTime;
    }

    private void OnCurrentGameModeStateChanged()
    {
        CheckTimers(true);
        UpdateTeamBanners(out ImageIdentifierVM? allyBanner, out ImageIdentifierVM? enemyBanner, out _ , out _);
        AllyBanner = allyBanner;
        EnemyBanner = enemyBanner;
    }

    private void UpdateTeamScores()
    {
        if (!_isTeamScoresEnabled)
        {
            return;
        }

        int attackScore = _missionScoreboardComponent.GetRoundScore(BattleSideEnum.Attacker);
        int defenderScore = _missionScoreboardComponent.GetRoundScore(BattleSideEnum.Defender);
        AllyTeamScore = _isAttackerTeamAlly ? attackScore : defenderScore;
        EnemyTeamScore = _isAttackerTeamAlly ? defenderScore : attackScore;
    }

    private void OnTeamChanged(NetworkCommunicator peer, Team previousTeam, Team newTeam)
    {
        if (peer.IsMine)
        {
            if (_isTeamScoresEnabled || _gameMode.GameType == MultiplayerGameType.Battle)
            {
                _isAttackerTeamAlly = newTeam.Side == BattleSideEnum.Attacker || newTeam.Side == BattleSideEnum.None;
                UpdateTeamScores();
            }

            CommanderInfo?.OnTeamChanged();
        }

        UpdateTeamBanners(out ImageIdentifierVM? allyBanner, out ImageIdentifierVM? enemyBanner, out _, out _);
        AllyBanner = allyBanner;
        EnemyBanner = enemyBanner;

        if (CommanderInfo == null)
        {
            return;
        }

        Teammates.FirstOrDefault(x => x.Peer.GetNetworkPeer() == peer)?.RefreshTeam();
        GetTeamColors(_mission.AttackerTeam, out string attackerColor1, out string attackerColor2);
        if (_isTeamScoresEnabled || _gameMode.GameType == MultiplayerGameType.Battle)
        {
            GetTeamColors(_mission.DefenderTeam, out string defenderColor1, out string defenderColor2);
            if (_isAttackerTeamAlly)
            {
                AllyTeamColor = attackerColor1;
                AllyTeamColor2 = attackerColor2;
                EnemyTeamColor = defenderColor1;
                EnemyTeamColor2 = defenderColor2;
            }
            else
            {
                AllyTeamColor = defenderColor1;
                AllyTeamColor2 = defenderColor2;
                EnemyTeamColor = attackerColor1;
                EnemyTeamColor2 = attackerColor2;
            }

            CommanderInfo.RefreshColors(AllyTeamColor, AllyTeamColor2, EnemyTeamColor, EnemyTeamColor2);
        }
        else
        {
            AllyTeamColor = attackerColor1;
            AllyTeamColor2 = attackerColor2;
            CommanderInfo.RefreshColors(AllyTeamColor, AllyTeamColor2, EnemyTeamColor!, EnemyTeamColor2!);
        }
    }

    private void GetTeamColors(Team team, out string color1, out string color2)
    {
        color1 = team.Color.ToString("X");
        color1 = color1.Remove(0, 2);
        color1 = "#" + color1 + "FF";
        color2 = team.Color2.ToString("X");
        color2 = color2.Remove(0, 2);
        color2 = "#" + color2 + "FF";
    }

    private void OnRefreshTeamMembers()
    {
        var teammates = Teammates.ToList();
        foreach (MissionPeer peer in VirtualPlayer.Peers<MissionPeer>())
        {
            if (peer.GetNetworkPeer().GetComponent<MissionPeer>() == null
                || PlayerTeam == null
                || peer.Team == null
                || peer.Team != PlayerTeam)
            {
                continue;
            }

            if (_teammateDictionary.TryGetValue(peer, out var teammate))
            {
                teammates.Remove(teammate);
            }
            else
            {
                MPPlayerVM playerVm = new(peer);
                Teammates.Add(playerVm);
                _teammateDictionary.Add(peer, playerVm);
            }
        }

        foreach (MPPlayerVM teammate in teammates)
        {
            Teammates.Remove(teammate);
            _teammateDictionary.Remove(teammate.Peer);
        }

        foreach (MPPlayerVM teammate in Teammates)
        {
            teammate.RefreshDivision();
            teammate.RefreshGold();
            teammate.RefreshProperties();
            teammate.UpdateDisabled();
        }
    }

    private void OnRefreshEnemyMembers()
    {
        List<MPPlayerVM> enemies = Enemies.ToList();
        foreach (MissionPeer peer in VirtualPlayer.Peers<MissionPeer>())
        {
            if (peer.GetNetworkPeer().GetComponent<MissionPeer>() == null
                || PlayerTeam == null
                || peer.Team == null
                || peer.Team == PlayerTeam
                || peer.Team == _mission.SpectatorTeam)
            {
                continue;
            }

            if (_enemyDictionary.TryGetValue(peer, out var enemy))
            {
                enemies.Remove(enemy);
            }
            else
            {
                MPPlayerVM playerVm = new(peer);
                Enemies.Add(playerVm);
                _enemyDictionary.Add(peer, playerVm);
            }
        }

        foreach (MPPlayerVM enemy in enemies)
        {
            Enemies.Remove(enemy);
            _enemyDictionary.Remove(enemy.Peer);
        }

        foreach (MPPlayerVM enemy in Enemies)
        {
            enemy.RefreshDivision();
            enemy.UpdateDisabled();
        }
    }

    private void UpdateShowTeamScores()
    {
        ShowTeamScores = !_gameMode.IsInWarmup
                         && ShowCommanderInfo
                         && _gameMode.GameType != MultiplayerGameType.Siege;
    }

    private Team? PlayerTeam
    {
        get
        {
            if (!GameNetwork.IsMyPeerReady)
            {
                return null;
            }

            MissionPeer component = GameNetwork.MyPeer.GetComponent<MissionPeer>();
            return component?.Team == null || component.Team.Side == BattleSideEnum.None ? null : component.Team;
        }
    }
}
