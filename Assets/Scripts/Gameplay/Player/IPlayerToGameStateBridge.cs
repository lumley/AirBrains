using ScreenLogic.Messages;

namespace Gameplay.Player
{
    public interface IPlayerToGameStateBridge
    {
        void OnSetReadyMessage(SetReadyMessage setReadyMessage);

        int DeviceId { get; set; }

        int CurrentRound { set; }

        void SetChosenActions(SendChosenActionsMessage.GameAction[] actionsSelected);
        void SendStartRound();
    }
}