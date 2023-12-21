using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Crpg.Module.Modes.TrainingGround;

public class CrpgTrainingGroundMissionRepresentative : MissionRepresentativeBase
{
    public const int DuelPrepTime = 3;
    public Action<MissionPeer, TroopType> OnDuelRequestedEvent = default!;
    public Action<MissionPeer> OnDuelRequestSentEvent = default!;
    public Action<MissionPeer, int> OnDuelPrepStartedEvent = default!;
    public Action OnAgentSpawnedWithoutDuelEvent = default!;
    public Action<MissionPeer, MissionPeer, int> OnDuelPreparationStartedForTheFirstTimeEvent = default!;
    public Action<MissionPeer> OnDuelEndedEvent = default!;
    public Action<MissionPeer> OnDuelRoundEndedEvent = default!;
    public Action<TroopType> OnMyPreferredZoneChanged = default!;
    private List<Tuple<MissionPeer, MissionTime>> _requesters = default!;
    private IFocusable? _focusedObject;
#if CRPG_SERVER
    private CrpgTrainingGroundServer _mission = default!;
#endif
    public int Bounty { get; private set; }
    public int Score { get; private set; }
    public int NumberOfWins { get; private set; }
    private bool _isInDuel
    {
        get
        {
            if (MissionPeer != null && MissionPeer.Team != null)
            {
                return MissionPeer.Team.IsDefender;
            }

            return false;
        }
    }

    public override void Initialize()
    {
        _requesters = new List<Tuple<MissionPeer, MissionTime>>();
#if CRPG_SERVER
        if (GameNetwork.IsServerOrRecorder)
        {
            _mission = Mission.Current.GetMissionBehavior<CrpgTrainingGroundServer>();
        }
#endif
        Mission.Current.SetMissionMode(MissionMode.Duel, atStart: true);
    }

    public void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
    {
        if (GameNetwork.IsClient)
        {
            GameNetwork.NetworkMessageHandlerRegisterer networkMessageHandlerRegisterer = new(mode);
            networkMessageHandlerRegisterer.Register<NetworkMessages.FromServer.DuelRequest>(HandleServerEventDuelRequest);
            networkMessageHandlerRegisterer.Register<DuelSessionStarted>(HandleServerEventDuelSessionStarted);
            networkMessageHandlerRegisterer.Register<DuelPreparationStartedForTheFirstTime>(HandleServerEventDuelStarted);
            networkMessageHandlerRegisterer.Register<DuelEnded>(HandleServerEventDuelEnded);
            networkMessageHandlerRegisterer.Register<DuelRoundEnded>(HandleServerEventDuelRoundEnded);
            networkMessageHandlerRegisterer.Register<DuelPointsUpdateMessage>(HandleServerPointUpdate);
        }
    }

    public void OnInteraction()
    {
        if (_focusedObject == null)
        {
            return;
        }

        IFocusable focusedObject = _focusedObject;
        if (focusedObject is Agent focusedAgent)
        {
            if (!focusedAgent.IsActive())
            {
                return;
            }

            if (_requesters.Any((Tuple<MissionPeer, MissionTime> req) => req.Item1 == focusedAgent.MissionPeer))
            {
                for (int i = 0; i < _requesters.Count; i++)
                {
                    if (_requesters[i].Item1 == MissionPeer)
                    {
                        _requesters.Remove(_requesters[i]);
                        break;
                    }
                }

                switch (PlayerType)
                {
                    case PlayerTypes.Client:
                        GameNetwork.BeginModuleEventAsClient();
                        GameNetwork.WriteMessage(new DuelResponse(focusedAgent.MissionRepresentative.Peer.Communicator as NetworkCommunicator, accepted: true));
                        GameNetwork.EndModuleEventAsClient();
                        break;
#if CRPG_SERVER
                    case PlayerTypes.Server:
                        _mission.DuelRequestAccepted(focusedAgent, ControlledAgent);
                        break;
#endif
                }
            }
            else
            {
                switch (PlayerType)
                {
                    case PlayerTypes.Client:
                        OnDuelRequestSentEvent?.Invoke(focusedAgent.MissionPeer);
                        GameNetwork.BeginModuleEventAsClient();
                        GameNetwork.WriteMessage(new NetworkMessages.FromClient.DuelRequest(focusedAgent.Index));
                        GameNetwork.EndModuleEventAsClient();
                        break;
#if CRPG_SERVER
                    case PlayerTypes.Server:
                        _mission.DuelRequestReceived(MissionPeer, focusedAgent.MissionPeer);
                        break;
#endif
                }
            }
        }
        else if (_focusedObject is DuelZoneLandmark duelZoneLandmark)
        {
            if (_isInDuel)
            {
                InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=v5EqMSlD}Can't change arena preference while in duel.").ToString()));
                return;
            }

            GameNetwork.BeginModuleEventAsClient();
            GameNetwork.WriteMessage(new RequestChangePreferredTroopType(duelZoneLandmark.ZoneTroopType));
            GameNetwork.EndModuleEventAsClient();
            OnMyPreferredZoneChanged?.Invoke(duelZoneLandmark.ZoneTroopType);
        }
    }

    private void HandleServerEventDuelRequest(NetworkMessages.FromServer.DuelRequest message)
    {
        Agent agentFromIndex = Mission.MissionNetworkHelper.GetAgentFromIndex(message.RequesterAgentIndex);
        Mission.MissionNetworkHelper.GetAgentFromIndex(message.RequestedAgentIndex);
        DuelRequested(agentFromIndex, message.SelectedAreaTroopType);
    }

    private void HandleServerEventDuelSessionStarted(DuelSessionStarted message)
    {
        OnDuelPreparation(message.RequesterPeer.GetComponent<MissionPeer>(), message.RequestedPeer.GetComponent<MissionPeer>());
    }

    private void HandleServerEventDuelStarted(DuelPreparationStartedForTheFirstTime message)
    {
        MissionPeer component = message.RequesterPeer.GetComponent<MissionPeer>();
        MissionPeer component2 = message.RequesteePeer.GetComponent<MissionPeer>();
        OnDuelPreparationStartedForTheFirstTimeEvent?.Invoke(component, component2, message.AreaIndex);
    }

    private void HandleServerEventDuelEnded(DuelEnded message)
    {
        OnDuelEndedEvent?.Invoke(message.WinnerPeer.GetComponent<MissionPeer>());
    }

    private void HandleServerEventDuelRoundEnded(DuelRoundEnded message)
    {
        OnDuelRoundEndedEvent?.Invoke(message.WinnerPeer.GetComponent<MissionPeer>());
    }

    private void HandleServerPointUpdate(DuelPointsUpdateMessage message)
    {
        CrpgTrainingGroundMissionRepresentative component = message.NetworkCommunicator.GetComponent<CrpgTrainingGroundMissionRepresentative>();
        component.Bounty = message.Bounty;
        component.Score = message.Score;
        component.NumberOfWins = message.NumberOfWins;
    }

    public void DuelRequested(Agent requesterAgent, TroopType selectedAreaTroopType)
    {
        _requesters.Add(new Tuple<MissionPeer, MissionTime>(requesterAgent.MissionPeer, MissionTime.Now + MissionTime.Seconds(10f)));
        switch (PlayerType)
        {
#if CRPG_SERVER
            case PlayerTypes.Bot:
                _mission.DuelRequestAccepted(requesterAgent, ControlledAgent);
                break;
            case PlayerTypes.Server:
                OnDuelRequestedEvent?.Invoke(requesterAgent.MissionPeer, selectedAreaTroopType);
                break;
#endif
            case PlayerTypes.Client:
                if (IsMine)
                {
                    OnDuelRequestedEvent?.Invoke(requesterAgent.MissionPeer, selectedAreaTroopType);
                    break;
                }

                GameNetwork.BeginModuleEventAsServer(Peer);
                GameNetwork.WriteMessage(new NetworkMessages.FromServer.DuelRequest(requesterAgent.Index, ControlledAgent.Index, selectedAreaTroopType));
                GameNetwork.EndModuleEventAsServer();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public bool CheckHasRequestFromAndRemoveRequestIfNeeded(MissionPeer requestOwner)
    {
        if (requestOwner != null && requestOwner.Representative == this)
        {
            _requesters.Clear();
            return false;
        }

        Tuple<MissionPeer, MissionTime> tuple = _requesters.FirstOrDefault((Tuple<MissionPeer, MissionTime> req) => req.Item1 == requestOwner);
        if (tuple == null)
        {
            return false;
        }

        if (requestOwner?.ControlledAgent == null || !requestOwner.ControlledAgent.IsActive())
        {
            _requesters.Remove(tuple);
            return false;
        }

        if (!tuple.Item2.IsPast)
        {
            return true;
        }

        _requesters.Remove(tuple);
        return false;
    }

    public void OnDuelPreparation(MissionPeer requesterPeer, MissionPeer requesteePeer)
    {
        switch (PlayerType)
        {
            case PlayerTypes.Client:
                if (IsMine)
                {
                    OnDuelPrepStartedEvent?.Invoke((MissionPeer == requesterPeer) ? requesteePeer : requesterPeer, 3);
                    break;
                }

                GameNetwork.BeginModuleEventAsServer(Peer);
                GameNetwork.WriteMessage(new DuelSessionStarted(requesterPeer.GetNetworkPeer(), requesteePeer.GetNetworkPeer()));
                GameNetwork.EndModuleEventAsServer();
                break;
            case PlayerTypes.Server:
                OnDuelPrepStartedEvent?.Invoke((MissionPeer == requesterPeer) ? requesteePeer : requesterPeer, 3);
                break;
        }

        Tuple<MissionPeer, MissionTime> tuple = _requesters.FirstOrDefault((Tuple<MissionPeer, MissionTime> req) => req.Item1 == requesterPeer);
        if (tuple != null)
        {
            _requesters.Remove(tuple);
        }
    }

    public void OnObjectFocused(IFocusable focusedObject)
    {
        _focusedObject = focusedObject;
    }

    public void OnObjectFocusLost()
    {
        _focusedObject = null;
    }

    public override void OnAgentSpawned()
    {
        if (ControlledAgent.Team != null && ControlledAgent.Team.Side == BattleSideEnum.Attacker)
        {
            OnAgentSpawnedWithoutDuelEvent?.Invoke();
        }
    }

    public void ResetBountyAndNumberOfWins()
    {
        Bounty = 0;
        NumberOfWins = 0;
    }

    public void OnDuelWon(float gainedScore)
    {
        Bounty += (int)(gainedScore / 5f);
        Score += (int)gainedScore;
        NumberOfWins++;
    }
}
