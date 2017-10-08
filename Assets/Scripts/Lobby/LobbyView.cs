using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyView : MonoBehaviour 
{
	[SerializeField]
	private CharacterVisualConfig Config;

	[SerializeField]
	private GameObject _playerPortraitPrefab;

	[SerializeField]
	private Transform _portraitGrid;

	private List<LobbyPlayerView> _players = new List<LobbyPlayerView>();
	
	public LobbyModel Model;

	public void ApplyModel(LobbyModel model)
	{
		if (Model != null) 
		{
			Model.OnChanged -= OnModelChanged;
			Model = null;
		}

		Model = model;

		if (Model != null) 
		{
			Model.OnChanged += OnModelChanged;
			OnModelChanged ();
		}
	}

	private void OnModelChanged()
	{
		//refresh player portraits and state if smth changed
		for (int i = 0; i < Model.Players.Count; i++) 
		{
			var playerModel = Model.Players [i];
			var player = GetPlayer (playerModel);
			player.ApplyModel(playerModel, Config.GetConfiguration(playerModel.Character));
		}

		//remove unused player portraits if disconnected
		if (Model.Players.Count < _players.Count) 
		{	
			for (int i = Model.Players.Count; i < _players.Count; i++) 
			{
				Destroy (_players [i].gameObject);
			}

			_players.RemoveRange (Model.Players.Count, _players.Count - Model.Players.Count);
		}
	}

	private LobbyPlayerView GetPlayer(LobbyPlayerData playerData)
	{
		LobbyPlayerView player = _players.Find (temp => temp.PlayerId == playerData.Id);
		if (player == null) 
		{
			var playerObject = Instantiate (_playerPortraitPrefab);
			playerObject.transform.SetParent(_portraitGrid);
			player = playerObject.GetComponent<LobbyPlayerView> ();

			_players.Add (player);
		}

		return player;
	}
}
