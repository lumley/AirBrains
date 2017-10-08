using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ScreenLogic.Messages
{
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

        public GameAction[] ActionsSelected;

        public SendChosenActionsMessage(JToken data)
        {
            var receivedActions = new List<string>();
            foreach (var value in data["actions"].Values<string>())
            {
                receivedActions.Add(value);
            }

            ActionsSelected = new GameAction[receivedActions.Count];
            for (var i = 0; i < receivedActions.Count; i++)
            {
                var receivedAction = receivedActions[i];

                var actionEnum = ParseActionToEnum(receivedAction);
                ActionsSelected[i] = actionEnum;
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