using System;
using Newtonsoft.Json;

namespace ScreenLogic.Requests
{
    [Serializable]
    public class StartRoundMessage
    {
        public const string MessageType = "StartRound";
        public const string MessageTypeInvariant = "startround";

        [JsonProperty("type")] public string Type = MessageType;
        [JsonProperty("round")] public int Round;
        [JsonProperty("turnCount")] public int TurnCount;
        [JsonProperty("donorCount")] public int DonorCount;
        [JsonProperty("fundsRaised")] public int FundsRaised;
    }
}