using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using ScreenLogic.Messages;
using UnityEngine;

namespace ScreenLogic
{
    public class AirConsoleBridge : MonoBehaviour
    {
#if !DISABLE_AIRCONSOLE

        private void Awake()
        {
            AirConsole.instance.onMessage += OnMessage;
            AirConsole.instance.onConnect += OnConnect;
            AirConsole.instance.onDisconnect += OnDeviceDisconnected;
        }

        /// <summary>
        /// We start the game if 2 players are connected and the game is not already running (activePlayers == null).
        /// 
        /// NOTE: We store the controller device_ids of the active players. We do not hardcode player device_ids 1 and 2,
        ///       because the two controllers that are connected can have other device_ids e.g. 3 and 7.
        ///       For more information read: http://developers.airconsole.com/#/guides/device_ids_and_states
        /// 
        /// </summary>
        /// <param name="deviceId">The device_id that connected</param>
        private void OnConnect(int deviceId)
        {
            if (AirConsole.instance.GetActivePlayerDeviceIds.Count == 0)
            {
                if (AirConsole.instance.GetControllerDeviceIds().Count > 0)
                {
                    StartGame();
                }
            }
        }

        private void OnDeviceDisconnected(int deviceId)
        {
            var activePlayer = AirConsole.instance.ConvertDeviceIdToPlayerNumber(deviceId);

            // No active player disconnected, ignore!
            if (activePlayer == -1)
            {
                return;
            }

            // TODO (slumley): Pass down what happened
        }

        /// <summary>
        /// We check which one of the active players has moved the paddle.
        /// </summary>
        /// <param name="deviceId">From.</param>
        /// <param name="data">Data.</param>
        private void OnMessage(int deviceId, JToken data)
        {
            var activePlayer = AirConsole.instance.ConvertDeviceIdToPlayerNumber(deviceId);

            // Ignore non-active player messages
            if (activePlayer == -1)
            {
                return;
            }

            // TODO (slumley): Send actual data to the device controller
            var actionType = (string) data["type"];
            switch (actionType.ToLowerInvariant())
            {
                case "setready":
                    new SetReadyMessage(data);
                    break;
                case "startgame":
                    new StartGameMessage();
                    break;
                case "sendchosenactions":
                    new SendChosenActionsMessage(data);
                    break;
            }
            
        }

        private void StartGame()
        {
            AirConsole.instance.SetActivePlayers(1);
            Debug.Log("Game started");
        }

        private void OnDestroy()
        {
            // unregister airconsole events on scene change
            if (AirConsole.instance != null)
            {
                AirConsole.instance.onMessage -= OnMessage;
            }
        }
#endif
    }
}