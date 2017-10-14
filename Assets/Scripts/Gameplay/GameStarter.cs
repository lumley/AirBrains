using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviour {

	public string[] availableLevelScenes;
	public Canvas[] toWireToCamera;
	public int humansToSpawnPerPlayer = 2; //TODO: Make this to a scene-specific variable?

	void Start () {
		SceneManager.LoadScene (availableLevelScenes[Mathf.RoundToInt(Random.value * availableLevelScenes.Length)], LoadSceneMode.Additive);
		foreach (Canvas canvas in toWireToCamera) {
			canvas.worldCamera = Camera.main;
		}

		List<PrefabReplacer> humanSpawns = new List<PrefabReplacer> ();
		List<PrefabReplacer> playerSpawns = new List<PrefabReplacer> ();
		foreach (PrefabReplacer replacer in GameObject.FindObjectsOfType<PrefabReplacer>()) {
			if (replacer.gameObject.name.Contains ("Human")) {
				humanSpawns.Add (replacer);
			} else if (replacer.gameObject.name.Contains ("Player")) {
				playerSpawns.Add (replacer);
			}
		}
		int playersToSpawn = 0; //TODO: Get this value
		for (int playerId = 0; playerId < playersToSpawn; playerId++) {
			PrefabReplacer replacer = playerSpawns [Mathf.RoundToInt (Random.value * playerSpawns.Count)];
			playerSpawns.Remove (replacer);
			GameObject newPlayer = replacer.SpawnPrefab ();
			//TODO: Tie this player object to a player controller instance
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
