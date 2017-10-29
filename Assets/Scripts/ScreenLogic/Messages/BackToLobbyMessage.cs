using System;
using Newtonsoft.Json;

namespace ScreenLogic.Messages
{
    [Serializable]
    public class BackToLobbyMessage
    {
        public const string MessageType = "BackToLobby";
        public const string MessageTypeInvariant = "backtolobby";

        [JsonProperty("type")] public string Type = MessageType;
    }
}