using System;
using System.Collections.Generic;
using Gameplay.Player;
using ScreenLogic;
using ScreenLogic.Messages;
using ScreenLogic.Requests;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private const int MaxAmountOfPlayersAllowed = 10;

    [SerializeField] private GameState _currentGameState = GameState.StartingUp;
    [SerializeField] private int _lobbyScreenIndex;
    [SerializeField] private int _gameScreenIndex;

    private readonly List<GlobalPlayer> _globalPlayers = new List<GlobalPlayer>(MaxAmountOfPlayersAllowed);

    private readonly HashSet<IPlayerToGameStateBridge> _gameCharacterReferences =
        new HashSet<IPlayerToGameStateBridge>();

    private readonly Dictionary<int, IPlayerToGameStateBridge> _deviceIdToGameCharacterMap =
        new Dictionary<int, IPlayerToGameStateBridge>(MaxAmountOfPlayersAllowed);

    private void Start()
    {
		HeadToTheLobby ();
    }

	public void HeadToTheLobby() {
		SceneManager.LoadScene(_lobbyScreenIndex, LoadSceneMode.Single);
		_currentGameState = GameState.OnLobby;
	}

	public void LinkExistingPlayers() {
		var lobbyController = LobbyController.FindInScene();
		if (lobbyController == null) {
			Debug.LogError ("NO LOBBYCONTROLLER FOUND!");
		}
		foreach(GlobalPlayer globalPlayer in _globalPlayers) {
			lobbyController.OnLobbyPlayerConnected(globalPlayer.LobbyPlayerData);
			AirConsoleBridge.Instance.SendOrUpdateAvatarForPlayer(globalPlayer);
		}
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
    }

    private void StartGameWithCurrentPlayers()
    {
        _gameCharacterReferences.Clear();
        _deviceIdToGameCharacterMap.Clear();

        _currentGameState = GameState.LoadingGame;
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