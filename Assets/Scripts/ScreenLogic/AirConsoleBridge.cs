using System;
using System.Collections.Generic;
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
                case SendChosenActionsMessage.MessageTypeInvariant:
                    var sendChosenActionsMessage = data.ToObject<SendChosenActionsMessage>();
                    _gameStateController.OnReceivedChosenActionsMessage(deviceId, sendChosenActionsMessage);
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
                AvatarIndex = globalPlayer.AvatarIndex
            };
            AirConsole.instance.Message(globalPlayer.LobbyPlayerData.Id,
                JsonConvert.SerializeObject(avatarChosenMessage));
#endif
        }

        public void SendStartRound(int deviceId, StartRoundMessage startRoundMessage)
        {
#if !DISABLE_AIRCONSOLE
            AirConsole.instance.Message(deviceId, JsonConvert.SerializeObject(startRoundMessage));
#endif
        }

        public void BroadcastBlockRound()
        {
#if !DISABLE_AIRCONSOLE
            var blockRoundMessage = new BlockRoundMessage();
            AirConsole.instance.Broadcast(JsonConvert.SerializeObject(blockRoundMessage));
#endif
        }

        public void BroadcastCharacterSetChanged(List<GlobalPlayer> globalPlayers)
        {
            var deviceIdNotReady = new List<int>(globalPlayers.Count);
            var characterIndexAvailable = new HashSet<int>();
            var characterTypeValues = Enum.GetValues(typeof(CharacterType));
            var amountOfCharacterValues = characterTypeValues.Length;
            for (var characterValueIndex = (int) CharacterType.None + 1;
                characterValueIndex < amountOfCharacterValues - 1;
                characterValueIndex++)
            {
                characterIndexAvailable.Add(characterValueIndex);
            }

            for (var i = 0; i < globalPlayers.Count; i++)
            {
                var globalPlayer = globalPlayers[i];
                if (!globalPlayer.LobbyPlayerData.IsReady)
                {
                    deviceIdNotReady.Add(globalPlayer.LobbyPlayerData.Id);
                }

                characterIndexAvailable.Remove((int) globalPlayer.LobbyPlayerData.Character);
            }

            var characterIndexArray = new int[characterIndexAvailable.Count];
            var arrayIndex = 0;
            foreach (var characterIndex in characterIndexAvailable)
            {
                characterIndexArray[arrayIndex++] = characterIndex;
            }
            var characterSetChangedMessage = new CharacterSetChangedMessage
            {
                AvailableAvatarIndexes = characterIndexArray,
                NotReadyDeviceIds = deviceIdNotReady.ToArray()
            };

#if !DISABLE_AIRCONSOLE
            AirConsole.instance.Broadcast(JsonConvert.SerializeObject(characterSetChangedMessage));
#endif
        }

        public void SendGameFinished(int deviceId, GameFinishedMessage message)
        {
#if !DISABLE_AIRCONSOLE
            AirConsole.instance.Message(deviceId, JsonConvert.SerializeObject(message));
#endif
        }
    }
}