using System;
using System.Collections.Generic;
using System.Linq;
using Crpg.Module.GUI.HudExtension;
using JetBrains.Annotations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.TeamSelection
{
    public class CrpgTeamSelectVM : ViewModel
    {
        private MissionRepresentativeBase? missionRep
        {
            get
            {
                NetworkCommunicator myPeer = GameNetwork.MyPeer;
                if (myPeer == null)
                {
                    return null;
                }

                VirtualPlayer virtualPlayer = myPeer.VirtualPlayer;
                if (virtualPlayer == null)
                {
                    return null;
                }

                return virtualPlayer.GetComponent<MissionRepresentativeBase>();
            }
        }

        public CrpgTeamSelectVM(Mission mission, Action<Team> onChangeTeamTo, Action onAutoAssign, Action onClose, IEnumerable<Team> teams, string gamemode)
        {
            _onClose = onClose;
            _onAutoAssign = onAutoAssign;
            _gamemodeStr = gamemode;
            CrpgHudExtensionVm.UpdateTeamBanners(out ImageIdentifierVM? team1Banner, out ImageIdentifierVM? team2Banner, out string team1Name, out string team2Name, byTeamIndex: true);
            Debug.Print("MultiplayerTeamSelectVM 1", 0, Debug.DebugColor.White, 17179869184UL);
            _gameMode = mission.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
            MissionScoreboardComponent missionBehavior = mission.GetMissionBehavior<MissionScoreboardComponent>();
            Debug.Print("MultiplayerTeamSelectVM 2", 0, Debug.DebugColor.White, 17179869184UL);
            IsRoundCountdownAvailable = _gameMode.IsGameModeUsingRoundCountdown;
            Debug.Print("MultiplayerTeamSelectVM 3", 0, Debug.DebugColor.White, 17179869184UL);
            Team team = teams.FirstOrDefault((Team t) => t.Side == BattleSideEnum.None);
            TeamSpectators = new CrpgTeamSelectTeamInstanceVM(missionBehavior, team, null, null, onChangeTeamTo, false, new TextObject("{=pSheKLB4}Spectator", null).ToString());
            Debug.Print("MultiplayerTeamSelectVM 4", 0, Debug.DebugColor.White, 17179869184UL);
            Team team2 = teams.FirstOrDefault((Team t) => t.TeamIndex == 1);
            BasicCultureObject @object = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
            Team1 = new CrpgTeamSelectTeamInstanceVM(missionBehavior, team2, @object, team1Banner, onChangeTeamTo, false, team1Name);
            Debug.Print("MultiplayerTeamSelectVM 5", 0, Debug.DebugColor.White, 17179869184UL);
            Team team3 = teams.FirstOrDefault((Team t) => t.TeamIndex == 2);
            @object = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
            Team2 = new CrpgTeamSelectTeamInstanceVM(missionBehavior, team3, @object, team2Banner, onChangeTeamTo, true, team2Name);
            Debug.Print("MultiplayerTeamSelectVM 6", 0, Debug.DebugColor.White, 17179869184UL);
            if (GameNetwork.IsMyPeerReady)
            {
                _missionPeer = GameNetwork.MyPeer.GetComponent<MissionPeer>();
                IsCancelDisabled = _missionPeer.Team == null;
            }

            Debug.Print("MultiplayerTeamSelectVM 7", 0, Debug.DebugColor.White, 17179869184UL);
            RefreshValues();
            Debug.Print("MultiplayerTeamSelectVM 8", 0, Debug.DebugColor.White, 17179869184UL);
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
            AutoassignLbl = new TextObject("{=bON4Kn6B}Auto Assign", null).ToString();
            TeamSelectTitle = new TextObject("{=aVixswW5}Team Selection", null).ToString();
            GamemodeLbl = GameTexts.FindText("str_multiplayer_official_game_type_name", _gamemodeStr).ToString();
            Team1.RefreshValues();
            _team2.RefreshValues();
            _teamSpectators.RefreshValues();
        }

        public void Tick(float dt)
        {
            RemainingRoundTime = TimeSpan.FromSeconds((double)MathF.Ceiling(_gameMode.RemainingTime)).ToString("mm':'ss");
        }

        public void RefreshDisabledTeams(List<Team> disabledTeams)
        {
            if (disabledTeams == null)
            {
                CrpgTeamSelectTeamInstanceVM teamSpectators = TeamSpectators;
                if (teamSpectators != null)
                {
                    teamSpectators.SetIsDisabled(false, false);
                }

                CrpgTeamSelectTeamInstanceVM team = Team1;
                if (team != null)
                {
                    team.SetIsDisabled(false, false);
                }

                CrpgTeamSelectTeamInstanceVM team2 = Team2;
                if (team2 == null)
                {
                    return;
                }

                team2.SetIsDisabled(false, false);
                return;
            }
            else
            {
                CrpgTeamSelectTeamInstanceVM teamSpectators2 = TeamSpectators;
                if (teamSpectators2 != null)
                {
                    bool isCurrentTeam = false;
                    bool disabledForBalance;
                    if (disabledTeams == null)
                    {
                        disabledForBalance = false;
                    }
                    else
                    {
                        CrpgTeamSelectTeamInstanceVM teamSpectators3 = TeamSpectators;
                        disabledForBalance = teamSpectators3 == null ? false : disabledTeams.Contains(teamSpectators3.Team);
                    }

                    teamSpectators2.SetIsDisabled(isCurrentTeam, disabledForBalance);
                }

                CrpgTeamSelectTeamInstanceVM team3 = Team1;
                if (team3 != null)
                {
                    CrpgTeamSelectTeamInstanceVM team4 = Team1;
                    Team? team5 = (team4 != null) ? team4.Team : null;
                    MissionPeer missionPeer = _missionPeer;
                    bool isCurrentTeam2 = team5 == ((missionPeer != null) ? missionPeer.Team : null);
                    bool disabledForBalance2;
                    if (disabledTeams == null)
                    {
                        disabledForBalance2 = false;
                    }
                    else
                    {
                        CrpgTeamSelectTeamInstanceVM team6 = Team1;
                        disabledForBalance2 = team6 == null ? false : disabledTeams.Contains(team6.Team);
                    }

                    team3.SetIsDisabled(isCurrentTeam2, disabledForBalance2);
                }

                CrpgTeamSelectTeamInstanceVM team7 = Team2;
                if (team7 == null)
                {
                    return;
                }

                CrpgTeamSelectTeamInstanceVM team8 = Team2;
                Team? team9 = (team8 != null) ? team8.Team : null;
                MissionPeer missionPeer2 = _missionPeer;
                bool isCurrentTeam3 = team9 == ((missionPeer2 != null) ? missionPeer2.Team : null);
                bool disabledForBalance3;
                if (disabledTeams == null)
                {
                    disabledForBalance3 = false;
                }
                else
                {
                    CrpgTeamSelectTeamInstanceVM team10 = Team2;
                    disabledForBalance3 = team10 == null ? false : disabledTeams.Contains(team10.Team);
                }

                team7.SetIsDisabled(isCurrentTeam3, disabledForBalance3);
                return;
            }
        }

        public void RefreshPlayerAndBotCount(int playersCountOne, int playersCountTwo, int botsCountOne, int botsCountTwo)
        {
            MBTextManager.SetTextVariable("PLAYER_COUNT", playersCountOne.ToString(), false);
            Team1.DisplayedSecondary = new TextObject("{=Etjqamlh}{PLAYER_COUNT} Players", null).ToString();
            MBTextManager.SetTextVariable("BOT_COUNT", botsCountOne.ToString(), false);
            Team1.DisplayedSecondarySub = new TextObject("{=eCOJSSUH}({BOT_COUNT} Bots)", null).ToString();
            MBTextManager.SetTextVariable("PLAYER_COUNT", playersCountTwo.ToString(), false);
            Team2.DisplayedSecondary = new TextObject("{=Etjqamlh}{PLAYER_COUNT} Players", null).ToString();
            MBTextManager.SetTextVariable("BOT_COUNT", botsCountTwo.ToString(), false);
            Team2.DisplayedSecondarySub = new TextObject("{=eCOJSSUH}({BOT_COUNT} Bots)", null).ToString();
        }

        public void RefreshFriendsPerTeam(IEnumerable<MissionPeer> friendsTeamOne, IEnumerable<MissionPeer> friendsTeamTwo)
        {
            Team1.RefreshFriends(friendsTeamOne);
            Team2.RefreshFriends(friendsTeamTwo);
        }

        [UsedImplicitly]
        public void ExecuteCancel()
        {
            _onClose();
        }

        [UsedImplicitly]
        public void ExecuteAutoAssign()
        {
            _onAutoAssign();
        }

        [DataSourceProperty]
        public CrpgTeamSelectTeamInstanceVM Team1
        {
            get
            {
                return _team1;
            }
            set
            {
                if (value != _team1)
                {
                    _team1 = value;
                    OnPropertyChangedWithValue(value, "Team1");
                }
            }
        }

        [DataSourceProperty]
        public CrpgTeamSelectTeamInstanceVM Team2
        {
            get
            {
                return _team2;
            }
            set
            {
                if (value != _team2)
                {
                    _team2 = value;
                    OnPropertyChangedWithValue(value, "Team2");
                }
            }
        }

        [DataSourceProperty]
        public CrpgTeamSelectTeamInstanceVM TeamSpectators
        {
            get
            {
                return _teamSpectators;
            }
            set
            {
                if (value != _teamSpectators)
                {
                    _teamSpectators = value;
                    OnPropertyChangedWithValue(value, "TeamSpectators");
                }
            }
        }

        [DataSourceProperty]
        public string TeamSelectTitle
        {
            get
            {
                return _teamSelectTitle;
            }
            set
            {
                _teamSelectTitle = value;
                OnPropertyChangedWithValue(value, "TeamSelectTitle");
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
                if (value != _isRoundCountdownAvailable)
                {
                    _isRoundCountdownAvailable = value;
                    OnPropertyChangedWithValue(value, "IsRoundCountdownAvailable");
                }
            }
        }

        [DataSourceProperty]
        public string RemainingRoundTime
        {
            get
            {
                return _remainingRoundTime;
            }
            set
            {
                if (value != _remainingRoundTime)
                {
                    _remainingRoundTime = value;
                    OnPropertyChangedWithValue(value, "RemainingRoundTime");
                }
            }
        }

        [DataSourceProperty]
        public string GamemodeLbl
        {
            get
            {
                return _gamemodeLbl;
            }
            set
            {
                _gamemodeLbl = value;
                OnPropertyChangedWithValue(value, "GamemodeLbl");
            }
        }

        [DataSourceProperty]
        public string AutoassignLbl
        {
            get
            {
                return _autoassignLbl;
            }
            set
            {
                _autoassignLbl = value;
                OnPropertyChangedWithValue(value, "AutoassignLbl");
            }
        }

        [DataSourceProperty]
        public bool IsCancelDisabled
        {
            get
            {
                return _isCancelDisabled;
            }
            set
            {
                _isCancelDisabled = value;
                OnPropertyChangedWithValue(value, "IsCancelDisabled");
            }
        }

        private readonly Action _onClose;

        private readonly Action _onAutoAssign;

        private readonly MissionMultiplayerGameModeBaseClient _gameMode = default!;

        private readonly MissionPeer _missionPeer = default!;

        private readonly string _gamemodeStr;

        private string _teamSelectTitle = default!;

        private bool _isRoundCountdownAvailable;

        private string _remainingRoundTime = default!;

        private string _gamemodeLbl = default!;

        private string _autoassignLbl = default!;

        private bool _isCancelDisabled;

        private CrpgTeamSelectTeamInstanceVM _team1 = default!;

        private CrpgTeamSelectTeamInstanceVM _team2 = default!;

        private CrpgTeamSelectTeamInstanceVM _teamSpectators = default!;
        private string _toggleMuteText = default!;
    }
}
