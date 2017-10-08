using Newtonsoft.Json.Linq;

namespace ScreenLogic.Messages
{
    public struct SetReadyMessage
    {
        public bool IsReady;

        public SetReadyMessage(JToken data)
        {
            IsReady = (bool) data["isReady"];
        }
    }
}