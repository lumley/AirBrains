using System;
using System.Collections.Generic;
using ScreenLogic;
using ScreenLogic.Requests;
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
                AirConsoleBridge.Instance.SendOrUpdateAvatarForPlayer(globalPlayer);
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

    public void OnSetAvatarIndexMessage(int deviceId, SetAvatarIndexMessage setAvatarIndexMessage)
    {
        var playerIndex = IndexOfPlayerWithDeviceId(deviceId);
        if (playerIndex < 0)
        {
            return;
        }

        var globalPlayer = _globalPlayers[playerIndex];
        if (globalPlayer.AvatarIndex != setAvatarIndexMessage.AvatarIndex)
        {
            if (_currentGameState == GameState.OnLobby && IsAvatarAvailable(setAvatarIndexMessage.AvatarIndex))
            {
                globalPlayer.AvatarIndex = setAvatarIndexMessage.AvatarIndex;
                var lobbyController = LobbyController.FindInScene();

                lobbyController.OnLobbyPlayerDataChanged(globalPlayer.LobbyPlayerData);
            }
            AirConsoleBridge.Instance.SendOrUpdateAvatarForPlayer(globalPlayer);
        }
    }

    private bool IsAvatarAvailable(int avatarIndex)
    {
        var characterTypeValues = Enum.GetValues(typeof(CharacterType));
        var amountOfCharacterValues = characterTypeValues.Length;
        if (avatarIndex <= (int) CharacterType.None || avatarIndex >= amountOfCharacterValues)
        {
            return false;
        }

        var isAvatarTaken = false;
        for (var i = 0; i < _globalPlayers.Count; i++)
        {
            var globalPlayer = _globalPlayers[i];
            if (globalPlayer.AvatarIndex == avatarIndex)
            {
                isAvatarTaken = true;
                break;
            }
        }
        return !isAvatarTaken;
    }

    private CharacterType FindAvailableAvatar()
    {
        var characterTypeValues = Enum.GetValues(typeof(CharacterType));
        var amountOfCharacterValues = characterTypeValues.Length;
        for (var characterValueIndex = (int) CharacterType.None + 1;
            characterValueIndex < amountOfCharacterValues;
            characterValueIndex++)
        {
            var isAvatarAvailable = IsAvatarAvailable(characterValueIndex);
            if (isAvatarAvailable)
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