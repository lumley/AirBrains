using System;
using System.Collections.Generic;
using ScreenLogic;
using ScreenLogic.Messages;
using ScreenLogic.Requests;
using UnityEngine;

namespace Gameplay.Player
{
    [RequireComponent(typeof(ScoreTracker))]
    public class AirConsoleMoveProvider : MoveProvider, IPlayerToGameStateBridge
    {
        private const int InvalidDeviceId = 0; // Screen Id
        private GameStateController _gameStateController;
        private int _myDeviceId;

        [SerializeField] private bool _isPlayerReady;
        [SerializeField] private List<Move> _moveList;
        private ScoreTracker _scoreTracker;

        private bool IsPlayerControlling
        {
            get { return _myDeviceId != InvalidDeviceId; }
        }

        private void Awake()
        {
            _gameStateController = GameStateController.FindInScene();
            _myDeviceId = _gameStateController.GrabDeviceId(this);
            _scoreTracker = GetComponent<ScoreTracker>();
        }

        public override void StartCollectingMoves()
        {
            _isPlayerReady = false;
            _moveList = new List<Move>(numberOfMovesPerRound);

            SendStartRound();
        }

        public override bool FinishedPlanningMoves()
        {
            return _isPlayerReady || !IsPlayerControlling;
        }

        public override List<Move> GetPlannedMoves()
        {
            if (!IsPlayerControlling)
            {
                _moveList.Clear();
                for (var i = 0; i < numberOfMovesPerRound; i++)
                {
                    _moveList.Add(Move.STAY);
                }
            }
            else
            {
                while (_moveList.Count < numberOfMovesPerRound)
                {
                    _moveList.Add(Move.STAY);
                }

                var excess = _moveList.Count - numberOfMovesPerRound;
                if (excess > 0)
                {
                    _moveList.RemoveRange(numberOfMovesPerRound - 1, excess);
                }
            }
            return _moveList;
        }

        public void OnSetReadyMessage(SetReadyMessage setReadyMessage)
        {
            _isPlayerReady = setReadyMessage.IsReady;
        }

        public int DeviceId
        {
            get { return _myDeviceId; }
            set { _myDeviceId = value; }
        }

        public int CurrentRound { private get; set; }

        public void SetChosenActions(SendChosenActionsMessage.GameAction[] actionsSelected)
        {
            _moveList = new List<Move>(actionsSelected.Length);

            for (var i = 0; i < actionsSelected.Length; i++)
            {
                var gameAction = actionsSelected[i];
                _moveList.Add(ParseAction(gameAction));
            }
        }

        public void SendStartRound()
        {
            if (!IsPlayerControlling)
            {
                return;
            }

            var startRoundMessage = new StartRoundMessage
            {
                DonorCount = _scoreTracker.DonorCountInCurrentRound,
                FundsRaised = _scoreTracker.Score,
                Round = CurrentRound,
                TurnCount = numberOfMovesPerRound
            };
            AirConsoleBridge.Instance.SendStartRound(_myDeviceId, startRoundMessage);
        }

        public void SendFinishMessage(List<ScoreTracker> sortedScoreTrackers)
        {
            var winnerDeviceId = 0;
            if (sortedScoreTrackers.Count > 0)
            {
                var sortedScoreTracker = sortedScoreTrackers[0];
                var airConsoleMoveProvider = sortedScoreTracker.GetComponent<AirConsoleMoveProvider>();
                if (airConsoleMoveProvider)
                {
                    winnerDeviceId = airConsoleMoveProvider.DeviceId;
                }
            }

            var myPosition = sortedScoreTrackers.IndexOf(_scoreTracker);
            AirConsoleBridge.Instance.SendGameFinished(_myDeviceId, new GameFinishedMessage
            {
                FundsRaised = _scoreTracker.Score,
                Placement = myPosition,
                WinnerDeviceId = winnerDeviceId
            });
        }

        private Move ParseAction(SendChosenActionsMessage.GameAction gameAction)
        {
            switch (gameAction)
            {
                case SendChosenActionsMessage.GameAction.Wait:
                    return Move.STAY;
                case SendChosenActionsMessage.GameAction.Up:
                    return Move.UP;
                case SendChosenActionsMessage.GameAction.Down:
                    return Move.DOWN;
                case SendChosenActionsMessage.GameAction.Left:
                    return Move.LEFT;
                case SendChosenActionsMessage.GameAction.Right:
                    return Move.RIGHT;
                default:
                    throw new ArgumentOutOfRangeException("gameAction", gameAction, null);
            }
        }
    }
}