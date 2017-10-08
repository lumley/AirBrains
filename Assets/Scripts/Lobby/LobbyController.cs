using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public sealed class LobbyController : MonoBehaviour 
{
	[SerializeField]
	public LobbyView View;

	public readonly LobbyModel Model = new LobbyModel();

	public event Action<List<CharacterType>> OnAvailableCharactersChanged; 

	private void Awake()
	{
		View.ApplyModel (Model);
		Model.OnAvailableCharactersChanged += SendAvailableCharacters;

		//for test
		SendAvailableCharacters (Model.AvailableCharacters);
	}

	private void OnDestroy()
	{
		View.ApplyModel (null);
		Model.OnAvailableCharactersChanged -= SendAvailableCharacters;
	}

	public void OnLobbyPlayerConnected(LobbyPlayerData playerData)
	{
		var existingPlayer = Model.GetPlayer (playerData.Character);

		if (existingPlayer != null) 
		{
			throw new CharacterAlreadyExistException (playerData.Character);
		}

		Model.AddPlayer (playerData);
	}


	public void OnLobbyPlayerDisconnected(int playerId)
	{
		var existingPlayer = Model.GetPlayer (playerId);
		
		if (existingPlayer == null) 
		{
			throw new PlayerDoesntExistException(playerId);
		}

		Model.RemovePlayer (playerId);
	}

	public void OnLobbyPlayerDataChanged(LobbyPlayerData player)
	{
		var existingPlayer = Model.GetPlayer (player.Id);

		if (existingPlayer == null) 
		{
			throw new PlayerDoesntExistException(player.Id);
		}

		Model.OnPlayerDataChanged (player);
	}

	private void SendAvailableCharacters(List<CharacterType> characters)
	{
		if (OnAvailableCharactersChanged != null) 
		{
			OnAvailableCharactersChanged (characters);
		}
	}
}
