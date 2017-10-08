using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public sealed class LobbyController : MonoBehaviour 
{
	[SerializeField]
	public LobbyView View;

	public LobbyModel Model = new LobbyModel();

	private void Awake()
	{
		View.ApplyModel (Model);
	}

	public void OnPlayerConnected(LobbyPlayerData playerData)
	{
		var existingPlayer = Model.GetPlayer (playerData.Character);

		if (existingPlayer != null) 
		{
			throw new CharacterAlreadyExistException (playerData.Character);
		}

		Model.AddPlayer (playerData);
	}


	public void OnPlayerDisconnected(int playerId)
	{
		var existingPlayer = Model.GetPlayer (playerId);
		
		if (existingPlayer == null) 
		{
			throw new PlayerDoesntExistException(playerId);
		}

		Model.RemovePlayer (playerId);
	}

	public void OnPlayerReadyStateChanged(LobbyPlayerData player)
	{
		var existingPlayer = Model.GetPlayer (player.Id);

		if (existingPlayer == null) 
		{
			throw new PlayerDoesntExistException(player.Id);
		}

		Model.OnPlayerDataChanged (player);
	}
		
}
