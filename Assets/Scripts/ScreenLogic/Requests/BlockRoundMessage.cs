using System;
using Newtonsoft.Json;

namespace ScreenLogic.Requests
{
    [Serializable]
    public class BlockRoundMessage
    {
        public const string MessageType = "BlockRound";
        public const string MessageTypeInvariant = "blockround";

        [JsonProperty("type")] public string Type = MessageType;
    }
}