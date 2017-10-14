using System;
using Newtonsoft.Json;

namespace ScreenLogic.Requests
{
    [Serializable]
    public class SetAvatarIndexMessage
    {
        public const string MessageType = "SetAvatarIndex";
        public const string MessageTypeInvariant = "setavatarindex";
        
        [JsonProperty(GlobalPlayer.AvatarIndexMessage)]
        public int AvatarIndex;

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}