using NDream.AirConsole;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScreenLogic.Messages;
using ScreenLogic.Requests;
using UnityEngine;

namespace ScreenLogic
{
    public class AirConsoleBridge : MonoBehaviour
    {
        [SerializeField] private GameStateController _gameStateController;
        private static AirConsoleBridge _bridgeInstance;

        public static AirConsoleBridge Instance
        {
            get { return _bridgeInstance; }
        }
#if !DISABLE_AIRCONSOLE

        private void Awake()
        {
            _bridgeInstance = this;
            AirConsole.instance.onMessage += OnMessage;
            AirConsole.instance.onConnect += OnConnect;
            AirConsole.instance.onDisconnect += OnDeviceDisconnected;
        }

        private void OnDestroy()
        {
            // unregister airconsole events on scene change
            if (AirConsole.instance != null)
            {
                AirConsole.instance.onMessage -= OnMessage;
            }
        }

        /// <summary>
        /// We check which one of the active players has moved the paddle.
        /// </summary>
        /// <param name="deviceId">From.</param>
        /// <param name="data">Data.</param>
        private void OnMessage(int deviceId, JToken data)
        {
            var actionType = (string) data["type"];
            if (actionType == null)
            {
                return;
            }
            
            switch (actionType.ToLowerInvariant())
            {
                case SetReadyMessage.MessageTypeInvariant:
                    var setReadyMessage = data.ToObject<SetReadyMessage>();
                    _gameStateController.OnSetReadyMessage(deviceId, setReadyMessage);
                    break;
                case StartGameMessage.MessageTypeInvariant:
                    var startGameMessage = data.ToObject<StartGameMessage>();
                    _gameStateController.OnStartGameMessage(deviceId, startGameMessage);
                    break;
                case "sendchosenactions":
                    new SendChosenActionsMessage(data);
                    break;
                case SetAvatarIndexMessage.MessageTypeInvariant:
                    var globalPlayerState = data.ToObject<SetAvatarIndexMessage>();
                    _gameStateController.OnSetAvatarIndexMessage(deviceId, globalPlayerState);
                    break;
            }
        }
#endif

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
            _gameStateController.OnDeviceConnected(deviceId);
        }

        private void OnDeviceDisconnected(int deviceId)
        {
            _gameStateController.OnDeviceDisconnected(deviceId);
        }

        public void SendOrUpdateAvatarForPlayer(GlobalPlayer globalPlayer)
        {
#if !DISABLE_AIRCONSOLE
            var avatarChosenMessage = new AvatarChosenMessage
            {
                Type = AvatarChosenMessage.MessageType,
                AvatarIndex = globalPlayer.AvatarIndex
            };
            AirConsole.instance.Message(globalPlayer.LobbyPlayerData.Id,
                JsonConvert.SerializeObject(avatarChosenMessage));
#endif
        }
    }
}