using System.Collections.Generic;
using ScreenLogic;
using ScreenLogic.Messages;
using ScreenLogic.Requests;

namespace Gameplay.Player
{
    public class AirConsoleMoveProvider : MoveProvider, IPlayerToGameStateBridge
    {
        private const int InvalidDeviceId = 0; // Screen Id
        private GameStateController _gameStateController;
        private int _myDeviceId;

        private bool _isPlayerReady;
        private List<Move> _moveList;

        private bool IsPlayerControlling
        {
            get { return _myDeviceId != InvalidDeviceId; }
        }

        private void Awake()
        {
            _gameStateController = GameStateController.FindInScene();
            _myDeviceId = _gameStateController.GrabDeviceId(this);
        }


        public override void StartCollectingMoves()
        {
            _isPlayerReady = false;
            _moveList = new List<Move>(numberOfMovesPerRound);
            // TODO (slumley): Ask player to clean their movements (turn start)

            if (IsPlayerControlling)
            {
                var startRoundMessage = new StartRoundMessage
                {
                    DonorCount = 0,
                    FundsRaised = 0, // TODO (slumey): Get this amount from a ScoreTracker component
                    Round = 0,
                    TurnCount = numberOfMovesPerRound
                };
                AirConsoleBridge.Instance.SendStartRound(_myDeviceId, startRoundMessage);
            }
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

        public void SetDeviceId(int deviceId)
        {
            _myDeviceId = deviceId;
        }
    }
}