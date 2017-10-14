using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScreenLogic;

public class GameSpawner : MonoBehaviour {

	public int humansToSpawnPerPlayer = 2; //TODO: Make this to a scene-specific variable?

	public void StartGame (List<GlobalPlayer> players) {
		List<PrefabReplacer> humanSpawns = new List<PrefabReplacer> ();
		List<PrefabReplacer> playerSpawns = new List<PrefabReplacer> ();
		foreach (PrefabReplacer replacer in GameObject.FindObjectsOfType<PrefabReplacer>()) {
			if (replacer.gameObject.name.Contains ("Human")) {
				humanSpawns.Add (replacer);
			} else if (replacer.gameObject.name.Contains ("Player")) {
				playerSpawns.Add (replacer);
			}
		}

		int playersToSpawn = players.Count;
		for (int playerId = 0; playerId < playersToSpawn; playerId++) {
			PrefabReplacer replacer = playerSpawns [Mathf.RoundToInt (Random.value * playerSpawns.Count)];
			playerSpawns.Remove (replacer);
			GameObject newPlayer = replacer.SpawnPrefab ();
			GlobalPlayer thisPlayer = players [playerId];
			//TODO: Update the player object with animations based on thisPlayer.AvatarIndex
		}
		foreach (PrefabReplacer replacer in playerSpawns) {
			Destroy (replacer.gameObject);
		}
		int humansToSpawn = playersToSpawn * humansToSpawnPerPlayer;
		for (int humanId = 0; humanId < humansToSpawn; humanId++) {
			PrefabReplacer replacer = humanSpawns [Mathf.RoundToInt (Random.value * humanSpawns.Count)];
			humanSpawns.Remove (replacer);
			replacer.SpawnPrefab ();
		}
		foreach (PrefabReplacer replacer in humanSpawns) {
			Destroy (replacer.gameObject);
		}

		GameObject.FindObjectOfType<LeaderboardTracker>().OnGameStart();
		GameObject.FindObjectOfType<GameRunner>().StartGame();
	}
}
