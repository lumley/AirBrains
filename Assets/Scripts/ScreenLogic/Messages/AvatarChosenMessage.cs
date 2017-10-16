using System;
using Newtonsoft.Json;

namespace ScreenLogic.Messages
{
    [Serializable]
    public class AvatarChosenMessage
    {
        public const string MessageType = "AvatarChosen";
        public const string MessageTypeInvariant = "avatarchosen";

        [JsonProperty("type")] public string Type = MessageType;
        [JsonProperty("avatarIndex")] public int AvatarIndex;
    }
}