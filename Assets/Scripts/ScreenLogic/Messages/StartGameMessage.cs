using System;

namespace ScreenLogic.Messages
{
    [Serializable]
    public class StartGameMessage
    {
        public const string MessageType = "StartGame";
        public const string MessageTypeInvariant = "startgame";
    }
}