﻿using System;
using System.Collections.Generic;
using Gameplay.Player;
using ScreenLogic;
using ScreenLogic.Messages;
using ScreenLogic.Requests;
using UnityEngine;
using UnityEngine.SceneManagement;
using NDream.AirConsole;

public class GameStateController : MonoBehaviour
{
    public enum GameState
    {
        StartingUp,
        OnLobby,
        LoadingGame,
        OnGame,
        OnWrapUpScreen,
    }

    public static GameStateController FindInScene()
    {
        return FindObjectOfType<GameStateController>();
    }

    public const int MaxAmountOfPlayersAllowed = 10;

    [SerializeField] private GameState _currentGameState = GameState.StartingUp;
    [SerializeField] private int _lobbyScreenIndex;
    [SerializeField] private int _gameScreenIndex;

    private readonly List<GlobalPlayer> _globalPlayers = new List<GlobalPlayer>(MaxAmountOfPlayersAllowed);

    private readonly HashSet<IPlayerToGameStateBridge> _gameCharacterReferences =
        new HashSet<IPlayerToGameStateBridge>();

    private readonly Dictionary<int, IPlayerToGameStateBridge> _deviceIdToGameCharacterMap =
        new Dictionary<int, IPlayerToGameStateBridge>(MaxAmountOfPlayersAllowed);

    public void HeadToTheLobby()
    {
        SceneManager.LoadScene(_lobbyScreenIndex, LoadSceneMode.Single);
        AirConsoleBridge.Instance.BroadcastBackToLobby();
    }

    public void LinkExistingPlayers()
    {
        var lobbyController = LobbyController.FindInScene();
        if (lobbyController == null)
        {
            Debug.LogError("NO LOBBYCONTROLLER FOUND!");
            return;
        }

        foreach (var globalPlayer in _globalPlayers)
        {
            RegisterPlayerInLobby(globalPlayer);
        }
    }

    public void OnDeviceConnected(int deviceId)
    {
        if ((_currentGameState == GameState.OnLobby || _currentGameState == GameState.StartingUp)
            && _globalPlayers.Count < MaxAmountOfPlayersAllowed)
        {
            RegisterGlobalPlayer(deviceId);
        }
        else if (_currentGameState == GameState.OnGame)
        {
            LinkPlayerToCharacterDuringGameplay(deviceId);
        }
    }

    private void LinkPlayerToCharacterDuringGameplay(int deviceId)
    {
        var playerIndex = IndexOfPlayerWithDeviceId(deviceId);
        if (playerIndex >= 0)
        {
            var globalPlayer = _globalPlayers[playerIndex];
            AirConsoleBridge.Instance.SendOrUpdateAvatarForPlayer(globalPlayer);
            IPlayerToGameStateBridge gameCharacter;
            if (_deviceIdToGameCharacterMap.TryGetValue(deviceId, out gameCharacter))
            {
                gameCharacter.DeviceId = deviceId;
                gameCharacter.SendStartRound();
            }
        }
        else
        {
            // Search for any orphan player and take it over!
            foreach (var playerToGameStateBridge in _gameCharacterReferences)
            {
                var id = playerToGameStateBridge.DeviceId;
                if (id == 0)
                {
                    playerToGameStateBridge.DeviceId = deviceId;
                    var previousKeyToRemove = 0;
                    foreach (var deviceIdToGameCharacter in _deviceIdToGameCharacterMap)
                    {
                        if (deviceIdToGameCharacter.Value == playerToGameStateBridge)
                        {
                            previousKeyToRemove = deviceIdToGameCharacter.Key;
                            break;
                        }
                    }

                    if (previousKeyToRemove != 0)
                    {
                        var globalPlayer =
                            _globalPlayers.Find(player => player.LobbyPlayerData.Id == previousKeyToRemove);
                        if (globalPlayer != null)
                        {
                            globalPlayer.LobbyPlayerData.Id = deviceId;
                        }

                        _deviceIdToGameCharacterMap.Remove(previousKeyToRemove);
                    }

                    _deviceIdToGameCharacterMap[deviceId] = playerToGameStateBridge;
                    break;
                }
            }
        }
    }

    private void RegisterGlobalPlayer(int deviceId)
    {
        var playerIndex = IndexOfPlayerWithDeviceId(deviceId);
        if (playerIndex < 0)
        {
            var availableAvatar = FindAvailableAvatar();
            var globalPlayer = new GlobalPlayer(deviceId, availableAvatar);
            _globalPlayers.Add(globalPlayer);

            if (_currentGameState == GameState.OnLobby)
            {
                RegisterPlayerInLobby(globalPlayer);
            }
        }
    }

    private void RegisterPlayerInLobby(GlobalPlayer globalPlayer)
    {
        var lobbyController = LobbyController.FindInScene();
        lobbyController.OnLobbyPlayerConnected(globalPlayer.LobbyPlayerData);
        AirConsoleBridge.Instance.SendOrUpdateAvatarForPlayer(globalPlayer);
        AirConsoleBridge.Instance.BroadcastCharacterSetChanged(_globalPlayers);
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
        else if (_currentGameState == GameState.OnGame)
        {
            IPlayerToGameStateBridge gameCharacter;
            if (_deviceIdToGameCharacterMap.TryGetValue(deviceId, out gameCharacter))
            {
                gameCharacter.DeviceId = 0; // No device Id
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
            AirConsoleBridge.Instance.BroadcastCharacterSetChanged(_globalPlayers);
        }
    }

    public void OnSetReadyMessage(int deviceId, SetReadyMessage setReadyMessage)
    {
        var playerIndex = IndexOfPlayerWithDeviceId(deviceId);
        if (playerIndex < 0)
        {
            return;
        }

        if (_currentGameState == GameState.OnLobby)
        {
            var globalPlayer = _globalPlayers[playerIndex];
            globalPlayer.LobbyPlayerData.IsReady = setReadyMessage.IsReady;

            var lobbyController = LobbyController.FindInScene();
            lobbyController.OnLobbyPlayerDataChanged(globalPlayer.LobbyPlayerData);
            AirConsoleBridge.Instance.BroadcastCharacterSetChanged(_globalPlayers);
        }
        else if (_currentGameState == GameState.OnGame)
        {
            IPlayerToGameStateBridge playerOnGame;
            if (_deviceIdToGameCharacterMap.TryGetValue(deviceId, out playerOnGame))
            {
                playerOnGame.OnSetReadyMessage(setReadyMessage);
            }
        }
    }

    public void OnStartGameMessage(int deviceId, StartGameMessage startGameMessage)
    {
        var playerIndex = IndexOfPlayerWithDeviceId(deviceId);
        if (playerIndex < 0)
        {
            return;
        }

        if (_currentGameState == GameState.OnLobby)
        {
            for (var i = 0; i < _globalPlayers.Count; i++)
            {
                var globalPlayer = _globalPlayers[i];
                if (!globalPlayer.LobbyPlayerData.IsReady)
                {
                    return;
                }
            }

            StartGameWithCurrentPlayers();
        }
        else if (_currentGameState == GameState.OnWrapUpScreen)
        {
            AirConsoleBridge.Instance.RequestToShowAds(() =>
            {
                AirConsoleBridge.Instance.BroadcastLoadingScreen("LOADING");
                HeadToTheLobby();
            });
        }
    }

    private void StartGameWithCurrentPlayers()
    {
        _gameCharacterReferences.Clear();
        _deviceIdToGameCharacterMap.Clear();

        _currentGameState = GameState.LoadingGame;
        AirConsoleBridge.Instance.BroadcastLoadingScreen("LOADING");
        SceneManager.LoadScene(_gameScreenIndex, LoadSceneMode.Single);
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

    public int GrabDeviceId(IPlayerToGameStateBridge identifierGrabber)
    {
        int deviceIdToGrab;
        if (_gameCharacterReferences.Add(identifierGrabber))
        {
            deviceIdToGrab = 0;
            for (var i = 0; i < _globalPlayers.Count; i++)
            {
                var globalPlayer = _globalPlayers[i];
                var deviceId = globalPlayer.LobbyPlayerData.Id;
                if (!_deviceIdToGameCharacterMap.ContainsKey(deviceId))
                {
                    _deviceIdToGameCharacterMap[deviceId] = identifierGrabber;
                    deviceIdToGrab = deviceId;
                    break;
                }
            }
        }
        else
        {
            // Was already present
            deviceIdToGrab = 0;
            foreach (var playerToGameStateBridge in _deviceIdToGameCharacterMap)
            {
                var reference = playerToGameStateBridge.Value;
                if (reference == identifierGrabber)
                {
                    var deviceId = playerToGameStateBridge.Key;
                    deviceIdToGrab = deviceId;
                    break;
                }
            }
        }

        return deviceIdToGrab;
    }

    public void SetToState(GameState gameStateToSet)
    {
        if (_currentGameState == GameState.LoadingGame && gameStateToSet == GameState.OnGame)
        {
            _currentGameState = gameStateToSet;
            var gameSpawner = GameSpawner.FindInScene();
            gameSpawner.StartGame(_globalPlayers);
        }
        else if (_currentGameState == GameState.OnGame && gameStateToSet == GameState.OnWrapUpScreen)
        {
            _currentGameState = gameStateToSet;
        }
        else if (gameStateToSet == GameState.OnLobby)
        {
            _currentGameState = gameStateToSet;
        }
    }

    public void OnReceivedChosenActionsMessage(int deviceId, SendChosenActionsMessage sendChosenActionsMessage)
    {
        var playerIndex = IndexOfPlayerWithDeviceId(deviceId);
        if (_currentGameState == GameState.OnGame && playerIndex >= 0)
        {
            IPlayerToGameStateBridge playerToGameStateBridge;
            if (_deviceIdToGameCharacterMap.TryGetValue(deviceId, out playerToGameStateBridge))
            {
                playerToGameStateBridge.SetChosenActions(sendChosenActionsMessage.ActionsSelected);
            }
        }
    }
}