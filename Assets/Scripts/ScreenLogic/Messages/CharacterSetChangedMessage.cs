using System;
using Newtonsoft.Json;

namespace ScreenLogic.Messages
{
    [Serializable]
    public class CharacterSetChangedMessage
    {
        public const string MessageType = "CharacterSetChanged";
        public const string MessageTypeInvariant = "charactersetchanged";

        [JsonProperty("type")] public string Type = MessageType;
        [JsonProperty("availableAvatarIndexes")] public int[] AvailableAvatarIndexes;
        [JsonProperty("notReadyDeviceIds")] public int[] NotReadyDeviceIds;
        
    }
}