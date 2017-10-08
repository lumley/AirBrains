using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class LobbyMobileTest : MonoBehaviour 
{
	public LobbyController LobbyController;
	public CharacterVisualConfig Config;
	public GameObject PlayerPrefab;
	public Transform ListTransform;

	private List<LobbyPlayerViewTest> _players;

	private void Awake()
	{
		CreateItems ();
		LobbyController.OnAvailableCharactersChanged += OnAvailableCharactersChanged;
	}

	private void OnAvailableCharactersChanged (List<CharacterType> characters)
	{
		for (int i = 0; i < _players.Count; i++)
		{
			var playerView = _players [i];
			playerView.IsAvailable = characters.Contains (playerView.Character);
		}
	}

	private void CreateItems()
	{
		_players = new List<LobbyPlayerViewTest> ();
		for (int i = 0; i < Config.Characters.Count; i++) 
		{
			var playerObject = Instantiate (PlayerPrefab);
			playerObject.transform.SetParent (ListTransform);

			var playerView = playerObject.GetComponent<LobbyPlayerViewTest> ();
			playerView.ApplyData (Config.Characters[i], i);
			playerView.IsAvailable = true;

			playerView.OnConnected = OnPlayerTryToConnect;
			playerView.OnDisconnected = OnPlayerTryToDisconnect;
			playerView.OnReadyStateChanged = OnPlayerReadyStateChanged;

			_players.Add (playerView);
		}
	}

	private LobbyPlayerData GetPlayerData(LobbyPlayerViewTest playerView)
	{
		return new LobbyPlayerData 
		{ 
			Id = playerView.PlayerId, 
			Character = playerView.Character,
			IsReady = playerView.IsReady
		};
	}

	private void OnPlayerTryToConnect(LobbyPlayerViewTest playerView)
	{
		Debug.LogFormat ("OnPlayerTryToConnect: {0}", playerView);
		LobbyController.OnLobbyPlayerConnected (GetPlayerData(playerView));
	}

	private void OnPlayerTryToDisconnect(LobbyPlayerViewTest playerView)
	{
		Debug.LogFormat ("OnPlayerTryToDisconnect: {0}", playerView);
		LobbyController.OnLobbyPlayerDisconnected (playerView.PlayerId);
	}

	private void OnPlayerReadyStateChanged(LobbyPlayerViewTest playerView)
	{
		Debug.LogFormat ("OnPlayerReadyStateChanged: {0}", playerView);
		LobbyController.OnLobbyPlayerDataChanged (GetPlayerData (playerView));
	}

}
