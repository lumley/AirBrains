using ScreenLogic.Messages;

namespace Gameplay.Player
{
    public interface IPlayerToGameStateBridge
    {
        void OnSetReadyMessage(SetReadyMessage setReadyMessage);

        void SetDeviceId(int deviceId);
    }
}