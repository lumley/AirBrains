using System;
using Newtonsoft.Json;

namespace ScreenLogic.Messages
{
    [Serializable]
    public class LoadingTimeMessage
    {
        public const string MessageType = "LoadingTime";
        public const string MessageTypeInvariant = "loadingtime";

        [JsonProperty("type")] public string Type = MessageType;
        [JsonProperty("message")] public string Message;
    }
}