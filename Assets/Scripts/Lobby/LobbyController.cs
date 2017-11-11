using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class LobbyController : MonoBehaviour
{
    [SerializeField] public LobbyView View;

    public readonly LobbyModel Model = new LobbyModel(GameStateController.MaxAmountOfPlayersAllowed, 2);

    public event Action<List<CharacterType>> OnAvailableCharactersChanged;

    public static LobbyController FindInScene()
    {
        return FindObjectOfType<LobbyController>();
    }

    private void Awake()
    {
        View.ApplyModel(Model);
        Model.OnAvailableCharactersChanged += SendAvailableCharacters;
        SendAvailableCharacters(Model.AvailableCharacters);
    }

    private void Start()
    {
        var gameStateController = GameStateController.FindInScene();
        if (gameStateController)
        {
            gameStateController.SetToState(GameStateController.GameState.OnLobby);
            gameStateController.LinkExistingPlayers();
        }
    }

    private void OnDestroy()
    {
        Model.OnAvailableCharactersChanged -= SendAvailableCharacters;
    }

    public void OnLobbyPlayerConnected(LobbyPlayerData playerData)
    {
        var existingPlayer = Model.GetPlayerByCharacter(playerData.Character);

        if (existingPlayer != null)
        {
            throw new CharacterAlreadyUsedException(playerData.Character);
        }

        Debug.LogFormat("OnLobbyPlayerConnected: {0}", playerData);
        Model.AddPlayer(playerData);
    }


    public void OnLobbyPlayerDisconnected(int playerId)
    {
        var existingPlayer = Model.GetOrCreatePlayer(playerId);

        if (existingPlayer == null)
        {
            throw new PlayerDoesntExistException(playerId);
        }

        Debug.LogFormat("OnLobbyPlayerDisconnected: {0}", existingPlayer);
        Model.RemovePlayer(playerId);
    }

    public void OnLobbyPlayerDataChanged(LobbyPlayerData player)
    {
        var existingPlayer = Model.GetOrCreatePlayer(player.Id);

        if (existingPlayer == null)
        {
            throw new PlayerDoesntExistException(player.Id);
        }

        Debug.LogFormat("OnLobbyPlayerDataChanged: {0} -> {1}", existingPlayer, player);
        Model.OnPlayerDataChanged(player);
    }

    private void SendAvailableCharacters(List<CharacterType> characters)
    {
        if (OnAvailableCharactersChanged != null)
        {
            OnAvailableCharactersChanged(characters);
        }
    }
}