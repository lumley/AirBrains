using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public sealed class LobbyModel
{
	private List<LobbyPlayerData> _players = new List<LobbyPlayerData>();

	public Action OnChanged;

	public void AddPlayer(LobbyPlayerData playerData)
	{
		_players.Add (playerData);
	}
		
	public void RemovePlayer(int playerId)
	{
		var existingPlayer = GetPlayer (playerId);
		_players.Remove (existingPlayer);
	}

	public void OnPlayerDataChanged(LobbyPlayerData playerData)
	{
		var existingPlayer = GetPlayer (playerData.Id);
		existingPlayer.Character = playerData.Character;
		existingPlayer.IsReady = playerData.IsReady;
		OnModelChanged ();
	}
		
	public LobbyPlayerData GetPlayer(int playerId)
	{
		return _players.Find (temp => temp.Id == playerId);
	}

	public LobbyPlayerData GetPlayer(CharacterType character)
	{
		return _players.Find (temp => temp.Character == character);
	}

	private void OnModelChanged()
	{
		if (OnChanged != null) 
		{
			OnChanged ();
		}
	}
}
