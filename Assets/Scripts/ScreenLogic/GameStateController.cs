using System;
using System.Collections.Generic;
using ScreenLogic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateController : MonoBehaviour
{
    public enum GameState
    {
        StartingUp,
        OnLobby,
        OnGame,
        OnWrapUpScreen,
    }

    public static GameStateController FindInScene()
    {
        return FindObjectOfType<GameStateController>();
    }

    private const int MaxAmountOfPlayersAllowed = 10;

    [SerializeField] private GameState _currentGameState = GameState.StartingUp;
    [SerializeField] private int _lobbyScreenIndex;
    [SerializeField] private int _gameScreenIndex;

    private readonly List<GlobalPlayer> _globalPlayers = new List<GlobalPlayer>(MaxAmountOfPlayersAllowed);

    private void Start()
    {
        SceneManager.LoadScene(_lobbyScreenIndex, LoadSceneMode.Single);
        _currentGameState = GameState.OnLobby;
    }

    public void OnDeviceConnected(int deviceId)
    {
        if (_currentGameState == GameState.OnLobby && _globalPlayers.Count < MaxAmountOfPlayersAllowed)
        {
            var playerIndex = IndexOfPlayerWithDeviceId(deviceId);
            if (playerIndex < 0)
            {
                var availableAvatar = FindAvailableAvatar();
                var globalPlayer = new GlobalPlayer(deviceId, availableAvatar);
                _globalPlayers.Add(globalPlayer);

                var lobbyController = LobbyController.FindInScene();
                lobbyController.OnLobbyPlayerConnected(globalPlayer.LobbyPlayerData);
            }
        }
    }

    public void OnDeviceDisconnected(int deviceId)
    {
        if (_currentGameState == GameState.OnLobby)
        {
            var indexOfPlayerWithDeviceId = IndexOfPlayerWithDeviceId(deviceId);
            if (indexOfPlayerWithDeviceId >= 0)
            {
                var globalPlayer = _globalPlayers[indexOfPlayerWithDeviceId];
                var lobbyController = LobbyController.FindInScene();

                lobbyController.OnLobbyPlayerDisconnected(globalPlayer.LobbyPlayerData.Id);

                // Tell the Lobby that this guy has disconnected
                _globalPlayers.RemoveAt(indexOfPlayerWithDeviceId);
            }
        }
    }

    private CharacterType FindAvailableAvatar()
    {
        var characterTypeValues = Enum.GetValues(typeof(CharacterType));
        var amountOfCharacterValues = characterTypeValues.Length;
        for (var characterValueIndex = (int) CharacterType.None + 1;
            characterValueIndex < amountOfCharacterValues;
            characterValueIndex++)
        {
            var isAvatarTaken = false;
            for (var i = 0; i < _globalPlayers.Count; i++)
            {
                var globalPlayer = _globalPlayers[i];
                if (globalPlayer.AvatarIndex == characterValueIndex)
                {
                    isAvatarTaken = true;
                    break;
                }
            }

            if (!isAvatarTaken)
            {
                return (CharacterType) characterValueIndex;
            }
        }
        return CharacterType.None;
    }

    private int IndexOfPlayerWithDeviceId(int deviceId)
    {
        for (var i = 0; i < _globalPlayers.Count; i++)
        {
            var globalPlayer = _globalPlayers[i];
            if (globalPlayer.LobbyPlayerData.Id == deviceId)
            {
                return i;
            }
        }
        return -1;
    }
}