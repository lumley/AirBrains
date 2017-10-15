using UnityEngine;

namespace ScreenLogic
{
    public class GameStateSetter : MonoBehaviour
    {
        [SerializeField] private GameStateController.GameState _gameStateToSet;

        private void Start()
        {
            var gameStateController = GameStateController.FindInScene();
            if (gameStateController)
            {
                gameStateController.SetToState(_gameStateToSet);
            }
        }
    }
}