using System;
using Newtonsoft.Json;

namespace ScreenLogic.Messages
{
    [Serializable]
    public class SetReadyMessage
    {
        public const string MessageType = "SetReady";
        public const string MessageTypeInvariant = "setready";
        
        [JsonProperty("type")]
        public string Type;
        
        [JsonProperty("isReady")]
        public bool IsReady;
    }
}