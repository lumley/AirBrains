using UnityEngine;

namespace ScreenLogic
{
    public class GlobalPlayer
    {
        public readonly LobbyPlayerData LobbyPlayerData;

        public const string AvatarIndexMessage = "avatarIndex";
        public int AvatarIndex
        {
            get { return (int) LobbyPlayerData.Character; }

            set { LobbyPlayerData.Character = (CharacterType) value; }
        }

        public GlobalPlayer(int deviceId, CharacterType characterTypeAssigned)
        {
            LobbyPlayerData = new LobbyPlayerData
            {
                Id = deviceId,
                Character = characterTypeAssigned,
                IsReady = false
            };
        }
    }
}