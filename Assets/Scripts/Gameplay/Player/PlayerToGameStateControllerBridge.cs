using ScreenLogic;
using UnityEngine;

namespace Gameplay.Player
{
    /// <summary>
    /// Link between the player and the <see cref="GlobalPlayer"/>
    /// </summary>
    public class PlayerToGameStateControllerBridge : MonoBehaviour
    {
        private const int InvalidDeviceId = 0; // Screen Id
        private GameStateController _gameStateController;
        private int _myDeviceId;

        private void Awake()
        {
            _gameStateController = GameStateController.FindInScene();
            _myDeviceId = _gameStateController.GrabDeviceId(this);
        }
        
        // TODO (slumley): Add callbacks to device changes
    }
}