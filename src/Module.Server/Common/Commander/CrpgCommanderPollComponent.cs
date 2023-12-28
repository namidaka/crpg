using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Diamond;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace Crpg.Module.Common.Commander;
internal class CrpgCommanderPollComponent : MissionNetwork
{
    public const int MinimumParticipantCountRequired = 3;
    public Action<MissionPeer, MissionPeer> OnCommanderPollOpened = default!;
    public Action<MultiplayerPollRejectReason> OnPollRejected = default!;
    public Action<int, int> OnPollUpdated = default!;
    public Action OnPollClosed = default!;
    public Action<CommanderPoll> OnPollCancelled = default!;
    private List<CommanderPoll> _ongoingPolls = new();
    private MultiplayerGameNotificationsComponent _notificationsComponent = default!;
    private CrpgCommanderBehaviorServer _commanderBehaviorServer = default!;
    private CrpgCommanderBehaviorClient _commanderBehaviorClient = default!;
    private MultiplayerPollComponent _multiplayerPollComponent = default!;
    private bool _isKickPollOngoing = false;

    public CommanderPoll? GetCommanderPollBySide(BattleSideEnum? side)
    {
        return _ongoingPolls.FirstOrDefault(c => c.Side == side) ?? null;
    }

    public CommanderPoll? GetCommanderPollByTarget(NetworkCommunicator? target)
    {
        return _ongoingPolls.FirstOrDefault(c => c.Target == target) ?? null;
    }

    protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
    {
        if (GameNetwork.IsClient)
        {
            //registerer.Register<CommanderPollRequestRejected>(HandleServerEventPollRequestRejected);
            registerer.Register<CommanderPollProgress>(HandleServerEventUpdatePollProgress);
            registerer.Register<CommanderPollCancelled>(HandleServerEventPollCancelled);
            registerer.Register<CommanderPollOpened>(HandleServerEventCommanderPollOpened);
            registerer.Register<CommanderPollClosed>(HandleServerEventCommanderPollClosed);
            return;
        }

        if (GameNetwork.IsServer)
        {
            registerer.Register<CommanderPollResponse>(HandleClientEventPollResponse);
            registerer.Register<CommanderPollRequested>(HandleClientEventCommanderPollRequested);
        }
    }

    public override void OnBehaviorInitialize()
    {
        base.OnBehaviorInitialize();
        _multiplayerPollComponent = Mission.Current.GetMissionBehavior<MultiplayerPollComponent>();
        _multiplayerPollComponent.OnKickPollOpened += OnKickPollStarted;
        _multiplayerPollComponent.OnPollCancelled += OnKickPollStopped;
        _multiplayerPollComponent.OnPollClosed += OnKickPollStopped;
        if (GameNetwork.IsServer)
        {
            _commanderBehaviorServer = Mission.Current.GetMissionBehavior<CrpgCommanderBehaviorServer>();
        }
        else if (GameNetwork.IsClient)
        {
            _commanderBehaviorClient = Mission.Current.GetMissionBehavior<CrpgCommanderBehaviorClient>();
        }
    }

    public override void OnMissionTick(float dt)
    {
        base.OnMissionTick(dt);

        int count = _ongoingPolls.Count;
        for (int i = 0; i < count; i++)
        {
            _ongoingPolls[i].Tick();
        }
    }

    public void Vote(CommanderPoll poll, bool accepted)
    {
        if (GameNetwork.IsServer)
        {
            if (GameNetwork.MyPeer != null)
            {
                ApplyVote(GameNetwork.MyPeer, poll, accepted);
                return;
            }
        }
        else if (poll != null && poll.IsOpen)
        {
            GameNetwork.BeginModuleEventAsClient();
            GameNetwork.WriteMessage(new CommanderPollResponse { Accepted = accepted });
            GameNetwork.EndModuleEventAsClient();
        }
    }

    private void ApplyVote(NetworkCommunicator peer, CommanderPoll poll, bool accepted)
    {
        if (poll != null && poll.ApplyVote(peer, accepted))
        {
            List<NetworkCommunicator> pollProgressReceivers = poll.GetPollProgressReceivers();
            int count = pollProgressReceivers.Count;
            for (int i = 0; i < count; i++)
            {
                GameNetwork.BeginModuleEventAsServer(pollProgressReceivers[i]);
                GameNetwork.WriteMessage(new PollProgress(poll.AcceptedCount, poll.RejectedCount));
                GameNetwork.EndModuleEventAsServer();
            }

            UpdatePollProgress(poll.AcceptedCount, poll.RejectedCount);
        }
    }

    private void RejectPollOnServer(NetworkCommunicator pollCreatorPeer, MultiplayerPollRejectReason rejectReason)
    {
        if (pollCreatorPeer.IsMine)
        {
            RejectPoll(rejectReason);
            return;
        }

        GameNetwork.BeginModuleEventAsServer(pollCreatorPeer);
        GameNetwork.WriteMessage(new CommanderPollRequestRejected { Reason = (int)rejectReason });
        GameNetwork.EndModuleEventAsServer();
    }

    private void RejectPoll(MultiplayerPollRejectReason rejectReason)
    {
        if (!GameNetwork.IsDedicatedServer)
        {
            _notificationsComponent.PollRejected(rejectReason);
        }

        Action<MultiplayerPollRejectReason> onPollRejected = OnPollRejected;
        onPollRejected?.Invoke(rejectReason);
    }

    private void UpdatePollProgress(int votesAccepted, int votesRejected)
    {
        Action<int, int> onPollUpdated = OnPollUpdated;
        onPollUpdated?.Invoke(votesAccepted, votesRejected);
    }

    private void CancelPoll(CommanderPoll poll)
    {
        if (poll != null)
        {
            poll.Cancel();
            _ongoingPolls.Remove(poll);
            Action<CommanderPoll> onPollCancelled = OnPollCancelled;
            onPollCancelled?.Invoke(poll);
        }
    }

    private void OnPollCancelledOnServer(CommanderPoll poll)
    {
        List<NetworkCommunicator> pollProgressReceivers = poll.GetPollProgressReceivers();
        int count = pollProgressReceivers.Count;
        for (int i = 0; i < count; i++)
        {
            GameNetwork.BeginModuleEventAsServer(pollProgressReceivers[i]);
            GameNetwork.WriteMessage(new PollCancelled());
            GameNetwork.EndModuleEventAsServer();
        }

        CancelPoll(poll);
    }

    public void RequestCommanderPoll(NetworkCommunicator peer)
    {
        if (GameNetwork.IsServer)
        {
            if (GameNetwork.MyPeer != null)
            {
                OpenCommanderPollOnServer(GameNetwork.MyPeer, peer);
                return;
            }
        }
        else
        {
            GameNetwork.BeginModuleEventAsClient();
            GameNetwork.WriteMessage(new CommanderPollRequested { PlayerPeer = peer });
            GameNetwork.EndModuleEventAsClient();
        }
    }

    private void OpenCommanderPollOnServer(NetworkCommunicator pollCreatorPeer, NetworkCommunicator targetPeer)
    {
        if (_isKickPollOngoing)
        {
            RejectPollOnServer(pollCreatorPeer, MultiplayerPollRejectReason.HasOngoingPoll);
        }

        foreach (CommanderPoll poll in _ongoingPolls)
        {
            if (poll.Side == pollCreatorPeer?.GetComponent<MissionPeer>().Team.Side)
            {
                RejectPollOnServer(pollCreatorPeer, MultiplayerPollRejectReason.HasOngoingPoll);
            }
        }

        if (pollCreatorPeer != null && pollCreatorPeer.IsConnectionActive && targetPeer != null && targetPeer.IsConnectionActive)
        {
            if (_commanderBehaviorServer.IsPlayerACommander(targetPeer))
            {
                RejectPollOnServer(pollCreatorPeer, MultiplayerPollRejectReason.HasOngoingPoll);
                return;
            }

            if (!targetPeer.IsSynchronized)
            {
                RejectPollOnServer(pollCreatorPeer, MultiplayerPollRejectReason.KickPollTargetNotSynced);
                return;
            }

            MissionPeer component = pollCreatorPeer.GetComponent<MissionPeer>();
            if (component != null)
            {
                List<NetworkCommunicator> list = new();
                foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
                {
                    if (networkCommunicator != null && networkCommunicator != targetPeer && networkCommunicator.IsSynchronized)
                    {
                        MissionPeer? component2 = networkCommunicator.GetComponent<MissionPeer>();
                        if (component2 != null && component2.Team == component.Team)
                        {
                            list.Add(networkCommunicator);
                        }
                    }
                }

                int count = list.Count;
                if (count + 1 >= 2)
                {
                    CommanderPoll poll = OpenCommanderPoll(targetPeer, pollCreatorPeer, list);
                    for (int i = 0; i < count; i++)
                    {
                        GameNetwork.BeginModuleEventAsServer(poll.ParticipantsToVote[i]);
                        GameNetwork.WriteMessage(new CommanderPollOpened { InitiatorPeer = pollCreatorPeer, PlayerPeer = targetPeer });
                        GameNetwork.EndModuleEventAsServer();
                    }

                    GameNetwork.BeginModuleEventAsServer(targetPeer);
                    GameNetwork.WriteMessage(new CommanderPollOpened { InitiatorPeer = pollCreatorPeer, PlayerPeer = targetPeer });
                    GameNetwork.EndModuleEventAsServer();
                    return;
                }

                RejectPollOnServer(pollCreatorPeer, MultiplayerPollRejectReason.NotEnoughPlayersToOpenPoll);
                return;
            }
        }
    }

    private CommanderPoll OpenCommanderPoll(NetworkCommunicator targetPeer, NetworkCommunicator pollCreatorPeer, List<NetworkCommunicator>? participantsToVote)
    {
        MissionPeer component = pollCreatorPeer.GetComponent<MissionPeer>();
        MissionPeer component2 = targetPeer.GetComponent<MissionPeer>();
        CommanderPoll poll = new(pollCreatorPeer, targetPeer, participantsToVote);
        _ongoingPolls.Add(poll);
        if (GameNetwork.IsServer)
        {
            poll.OnClosedOnServer += OnCommanderPollClosedOnServer;
            poll.OnCancelledOnServer += OnPollCancelledOnServer;
        }

        Action<MissionPeer, MissionPeer> onCommanderPollOpened = OnCommanderPollOpened;
        onCommanderPollOpened?.Invoke(component, component2);

        if (GameNetwork.MyPeer == pollCreatorPeer)
        {
            Vote(poll, true);
        }

        return poll;
    }

    private void OnCommanderPollClosedOnServer(CommanderPoll poll)
    {
        bool gotEnoughVotes = poll.GotEnoughAcceptVotesToEnd();
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage(new CommanderPollClosed { PlayerPeer = poll.Target, Accepted = gotEnoughVotes });
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
        CloseCommanderPoll(gotEnoughVotes, poll);
        if (gotEnoughVotes)
        {
            _commanderBehaviorServer.CreateCommand(poll.Target);
        }
    }

    private void CloseCommanderPoll(bool accepted, CommanderPoll poll)
    {
        poll.Close();
        _ongoingPolls.Remove(poll);

        Action onPollClosed = OnPollClosed;
        onPollClosed?.Invoke();

        if (!GameNetwork.IsDedicatedServer && accepted && poll.Target.IsMine)
        {
            InformationManager.DisplayMessage(new InformationMessage
            {
                Information = new TextObject("{=}You have been chosen to lead as Commander! Use '!o message' to order your troops!").ToString(),
                Color = new Color(0.48f, 0f, 1f),
            });
        }
    }

    private bool HandleClientEventCommanderPollRequested(NetworkCommunicator peer, GameNetworkMessage baseMessage)
    {
        CommanderPollRequested commanderPollRequested = (CommanderPollRequested)baseMessage;
        OpenCommanderPollOnServer(peer, commanderPollRequested.PlayerPeer);
        return true;
    }

    private bool HandleClientEventPollResponse(NetworkCommunicator peer, GameNetworkMessage baseMessage)
    {
        CommanderPollResponse pollResponse = (CommanderPollResponse)baseMessage;
        CommanderPoll? poll = GetCommanderPollBySide(peer.GetComponent<MissionPeer>().Team.Side);
        if (poll != null)
        {
            ApplyVote(peer, poll, pollResponse.Accepted);
        }

        return true;
    }

    private void HandleServerEventCommanderPollOpened(GameNetworkMessage baseMessage)
    {
        CommanderPollOpened commanderPollOpened = (CommanderPollOpened)baseMessage;
        OpenCommanderPoll(commanderPollOpened.PlayerPeer, commanderPollOpened.InitiatorPeer, null);
    }

    private void HandleServerEventUpdatePollProgress(GameNetworkMessage baseMessage)
    {
        PollProgress pollProgress = (PollProgress)baseMessage;
        UpdatePollProgress(pollProgress.VotesAccepted, pollProgress.VotesRejected);
    }

    private void HandleServerEventPollCancelled(GameNetworkMessage baseMessage)
    {
        CommanderPollCancelled commanderPollCancelled = (CommanderPollCancelled)baseMessage;
        Team team = Mission.MissionNetworkHelper.GetTeamFromTeamIndex(commanderPollCancelled.TeamIndex);
        CommanderPoll? poll = GetCommanderPollBySide(team.Side);
        if (poll != null)
        {
            CancelPoll(poll);
        }
    }

    private void HandleServerEventCommanderPollClosed(GameNetworkMessage baseMessage)
    {
        CommanderPollClosed commanderPollClosed = (CommanderPollClosed)baseMessage;
        CommanderPoll? poll = GetCommanderPollByTarget(commanderPollClosed.PlayerPeer);
        if (poll != null)
        {
            CloseCommanderPoll(commanderPollClosed.Accepted, poll);
        }
    }

    private void HandleServerEventPollRequestRejected(GameNetworkMessage baseMessage)
    {
        CommanderPollRequestRejected pollRequestRejected = (CommanderPollRequestRejected)baseMessage;
        RejectPoll((MultiplayerPollRejectReason)pollRequestRejected.Reason);// ctd
    }

    private void OnKickPollStarted(MissionPeer peer1, MissionPeer peer2, bool isBan)
    {
        _isKickPollOngoing = true;
    }

    private void OnKickPollStopped()
    {
        _isKickPollOngoing = false;
    }

    public class CommanderPoll
    {
        public NetworkCommunicator Requester;
        public NetworkCommunicator Target;
        public Action<CommanderPoll> OnClosedOnServer = default!;
        public Action<CommanderPoll> OnCancelledOnServer = default!;
        public int AcceptedCount;
        public int RejectedCount;
        private const int TimeoutInSeconds = 30;

        public BattleSideEnum Side { get; private set; }
        public List<NetworkCommunicator> ParticipantsToVote { get; } = new List<NetworkCommunicator>();
        public bool IsOpen { get; private set; }
        private int OpenTime { get; }
        private int CloseTime { get; set; }

        public CommanderPoll(NetworkCommunicator requester, NetworkCommunicator target, List<NetworkCommunicator>? participantsToVote)
        {
            if (participantsToVote != null)
            {
                ParticipantsToVote = participantsToVote;
            }

            Requester = requester;
            Target = target;
            Side = target.GetComponent<MissionPeer>().Team.Side;
            OpenTime = Environment.TickCount;
            CloseTime = 0;
            AcceptedCount = 0;
            RejectedCount = 0;
            IsOpen = true;
        }

        public virtual bool IsCancelled()
        {
            return false;
        }

        public virtual List<NetworkCommunicator> GetPollProgressReceivers()
        {
            return GameNetwork.NetworkPeers.ToList();
        }

        public void Tick()
        {
            if (GameNetwork.IsServer)
            {
                for (int i = ParticipantsToVote.Count - 1; i >= 0; i--)
                {
                    if (!ParticipantsToVote[i].IsConnectionActive)
                    {
                        ParticipantsToVote.RemoveAt(i);
                    }
                }

                if (IsCancelled())
                {
                    Action<CommanderPoll> onCancelledOnServer = OnCancelledOnServer;
                    onCancelledOnServer?.Invoke(this);
                    return;
                }
                else if (OpenTime < Environment.TickCount - 30000 || ResultsFinalized())
                {
                    Action<CommanderPoll> onClosedOnServer = OnClosedOnServer;
                    onClosedOnServer?.Invoke(this);
                }
            }
        }

        public void Close()
        {
            CloseTime = Environment.TickCount;
            IsOpen = false;
        }

        public void Cancel()
        {
            Close();
        }

        public bool ApplyVote(NetworkCommunicator peer, bool accepted)
        {
            bool result = false;
            if (ParticipantsToVote.Contains(peer))
            {
                if (accepted)
                {
                    AcceptedCount++;
                }
                else
                {
                    RejectedCount++;
                }

                ParticipantsToVote.Remove(peer);
                result = true;
            }

            return result;
        }

        public bool GotEnoughAcceptVotesToEnd()
        {
            return AcceptedByMajority();
        }

        private bool GotEnoughRejectVotesToEnd()
        {
            return RejectedByMajority();
        }

        private bool AcceptedByAllParticipants()
        {
            return AcceptedCount == GetPollParticipantCount();
        }

        private bool AcceptedByMajority()
        {
            return (float)AcceptedCount / GetPollParticipantCount() >= 0.5f;
        }

        private bool RejectedByAtLeastOneParticipant()
        {
            return RejectedCount > 0;
        }

        private bool RejectedByMajority()
        {
            return (float)RejectedCount / GetPollParticipantCount() > 0.50001f;
        }

        private int GetPollParticipantCount()
        {
            return ParticipantsToVote.Count + AcceptedCount + RejectedCount;
        }

        private bool ResultsFinalized()
        {
            return GotEnoughAcceptVotesToEnd() || GotEnoughRejectVotesToEnd() || ParticipantsToVote.Count == 0;
        }

    }
}
