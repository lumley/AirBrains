using System;
using Newtonsoft.Json;

namespace ScreenLogic.Messages
{
    [Serializable]
    public class GameFinishedMessage
    {
        public const string MessageType = "GameFinished";
        public const string MessageTypeInvariant = "gamefinished";

        [JsonProperty("type")] public string Type = MessageType;
        [JsonProperty("fundsRaised")] public int FundsRaised;
        [JsonProperty("winnerDeviceId")] public int WinnerDeviceId;
        [JsonProperty("placement")] public int Placement;
    }
}