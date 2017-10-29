using System;
using System.Collections.Generic;

public sealed class LobbyModel
{
	public readonly List<LobbyPlayerData> Players;
	private readonly HashSet<CharacterType> _availableCharacters = new HashSet<CharacterType>
	{
		CharacterType.Bulldog,
		CharacterType.Crocodile,
		CharacterType.Cat,
		CharacterType.Monkey,
		CharacterType.Parrot,
		CharacterType.Rino,
		CharacterType.Crow,
		CharacterType.Seal,
		CharacterType.Shark,
		CharacterType.Tiger
	};
	
	public List<CharacterType>  AvailableCharacters
	{
		get
		{
			var availableCharacters = new List<CharacterType>(_availableCharacters.Count);
			foreach (var availableCharacter in _availableCharacters)
			{
				availableCharacters.Add(availableCharacter);
			}
			return availableCharacters;
		}
	}

	public readonly int NeedReadyPlayers;

	public LobbyModel(int playerAmount, int waitingForReadyPlayers)
	{
		NeedReadyPlayers = waitingForReadyPlayers;
		Players = new List<LobbyPlayerData>(playerAmount);

		for (int i = 0; i < playerAmount; i++) 
		{
			Players.Add(LobbyPlayerData.CreateEmpty());
		}
	}

	public event Action OnChanged;
	public event Action<List<CharacterType>> OnAvailableCharactersChanged;

	public void AddPlayer(LobbyPlayerData playerData)
	{
		var existingPlayer = GetOrCreatePlayer (playerData.Id);
		existingPlayer.Character = playerData.Character;
		_availableCharacters.Remove (playerData.Character);

		OnModelChanged ();
		OnModelAvailableCharactersChanged ();
	}
		
	public void RemovePlayer(int playerId)
	{
		var existingPlayer = GetOrCreatePlayer (playerId);
		_availableCharacters.Add (existingPlayer.Character);
		existingPlayer.Character = CharacterType.None;
		existingPlayer.IsReady = false;
		existingPlayer.Id = 0;

		OnModelChanged ();
		OnModelAvailableCharactersChanged ();
	}

	public void OnPlayerDataChanged(LobbyPlayerData playerData)
	{
		var existingPlayer = GetOrCreatePlayer (playerData.Id);

		bool availableCharactersChanged = existingPlayer.Character != playerData.Character;
		if (availableCharactersChanged)
		{
			_availableCharacters.Add (existingPlayer.Character);
			_availableCharacters.Remove (playerData.Character);
		}

		existingPlayer.Character = playerData.Character;
		existingPlayer.IsReady = playerData.IsReady;

		int readyPlayers = 0;
		for (int i = 0; i < Players.Count; i++) {
			readyPlayers += Players [i].IsReady ? 1 : 0;
		}

		IsGameReadyToStart = readyPlayers >= NeedReadyPlayers;

		OnModelChanged ();

		if (availableCharactersChanged)
		{
			OnModelAvailableCharactersChanged ();
		}
	}
		
	public LobbyPlayerData GetOrCreatePlayer(int playerId)
	{
		var indexOfFirstAvailablePlayer = -1;
		for (var i = 0; i < Players.Count; i++)
		{
			var lobbyPlayerData = Players[i];
			if (lobbyPlayerData.Id == playerId)
			{
				return lobbyPlayerData;
			}

			if (lobbyPlayerData.Id == 0 && indexOfFirstAvailablePlayer < 0)
			{
				indexOfFirstAvailablePlayer = i;
			}
		}
		
		// If it was not found, try to replace the player with the Id we just got
		if (indexOfFirstAvailablePlayer >= 0)
		{
			var lobbyPlayerData = Players[indexOfFirstAvailablePlayer];
			lobbyPlayerData.Id = playerId;
			return lobbyPlayerData;
		}

		// We don't have any player available, just bail
		return null;
	}

	public LobbyPlayerData GetPlayerByCharacter(CharacterType character)
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

	public bool IsGameReadyToStart {
		private set;
		get;
	}
}
