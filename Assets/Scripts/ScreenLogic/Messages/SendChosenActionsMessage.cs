using System;
using Newtonsoft.Json;

namespace ScreenLogic.Messages
{
    [Serializable]
    public class SendChosenActionsMessage
    {
        public enum GameAction
        {
            Wait,
            Up,
            Down,
            Left,
            Right
        }

        public const string MessageType = "SendChosenActions";
        public const string MessageTypeInvariant = "sendchosenactions";
        
        [JsonProperty("actions")] public string[] Actions;

        public GameAction[] ActionsSelected
        {
            get
            {
                var actionsSelected = new GameAction[Actions.Length];
                for (var i = 0; i < Actions.Length; i++)
                {
                    var receivedAction = Actions[i];

                    var actionEnum = ParseActionToEnum(receivedAction);
                    actionsSelected[i] = actionEnum;
                }
                return actionsSelected;
            }
        }

        private GameAction ParseActionToEnum(string receivedAction)
        {
            switch (receivedAction.ToLowerInvariant())
            {
                case "left":
                    return GameAction.Left;
                case "up":
                    return GameAction.Up;
                case "right":
                    return GameAction.Right;
                case "down":
                    return GameAction.Down;
                default:
                    return GameAction.Wait;
            }
        }
    }
}