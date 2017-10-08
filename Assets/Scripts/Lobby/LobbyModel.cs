using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public sealed class LobbyModel
{
	public readonly List<LobbyPlayerData> Players;
	public readonly List<CharacterType> AvailableCharacters = new List<CharacterType>
	{
		CharacterType.Bulldog,
		CharacterType.Crocodile,
		CharacterType.Eagle,
		CharacterType.Monkey,
		CharacterType.Monster07,
		CharacterType.Monster08,
		CharacterType.Monster09,
		CharacterType.Seal,
		CharacterType.Shark,
		CharacterType.Tiger
	};

	public LobbyModel(int playerAmount)
	{
		Players = new List<LobbyPlayerData>(playerAmount);

		for (int i = 0; i < playerAmount; i++) 
		{
			Players.Add(new LobbyPlayerData ().Set (i, CharacterType.None));
		}
	}

	public event Action OnChanged;
	public event Action<List<CharacterType>> OnAvailableCharactersChanged;

	public void AddPlayer(LobbyPlayerData playerData)
	{
		var existingPlayer = GetPlayer (playerData.Id);
		existingPlayer.Character = playerData.Character;
		AvailableCharacters.Remove (playerData.Character);

		OnModelChanged ();
		OnModelAvailableCharactersChanged ();
	}
		
	public void RemovePlayer(int playerId)
	{
		var existingPlayer = GetPlayer (playerId);
		AvailableCharacters.Add (existingPlayer.Character);
		existingPlayer.Character = CharacterType.None;
		existingPlayer.IsReady = false;

		OnModelChanged ();
		OnModelAvailableCharactersChanged ();
	}

	public void OnPlayerDataChanged(LobbyPlayerData playerData)
	{
		var existingPlayer = GetPlayer (playerData.Id);

		bool availableCharactersChanged = existingPlayer.Character != playerData.Character;
		if (availableCharactersChanged)
		{
			AvailableCharacters.Add (existingPlayer.Character);
			AvailableCharacters.Remove (playerData.Character);
		}

		existingPlayer.Character = playerData.Character;
		existingPlayer.IsReady = playerData.IsReady;

		OnModelChanged ();

		if (availableCharactersChanged) {
			OnModelAvailableCharactersChanged ();
		}
	}
		
	public LobbyPlayerData GetPlayer(int playerId)
	{
		return Players.Find (temp =>  temp != null && temp.Id == playerId);
	}

	public LobbyPlayerData GetPlayer(CharacterType character)
	{
		return Players.Find (temp => temp != null && temp.Character == character);
	}

	private void OnModelChanged()
	{
		if (OnChanged != null) 
		{
			OnChanged ();
		}
	}

	private void OnModelAvailableCharactersChanged()
	{
		if (OnAvailableCharactersChanged != null) 
		{
			OnAvailableCharactersChanged (AvailableCharacters);
		}
	}
}
