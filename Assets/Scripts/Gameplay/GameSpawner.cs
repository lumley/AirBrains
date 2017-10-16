using System.Collections.Generic;
using ScreenLogic;
using UnityEngine;

public class GameSpawner : MonoBehaviour
{
    public int humansToSpawnPerPlayer = 2; //TODO: Make this to a scene-specific variable?

    public static GameSpawner FindInScene()
    {
        return FindObjectOfType<GameSpawner>();
    }

    public void StartGame(List<GlobalPlayer> players)
    {
        List<PrefabReplacer> humanSpawns = new List<PrefabReplacer>();
        List<PrefabReplacer> playerSpawns = new List<PrefabReplacer>();
        foreach (PrefabReplacer replacer in FindObjectsOfType<PrefabReplacer>())
        {
            if (replacer.gameObject.name.Contains("Human"))
            {
                humanSpawns.Add(replacer);
            }
            else if (replacer.gameObject.name.Contains("Player"))
            {
                playerSpawns.Add(replacer);
            }
        }

        int playersToSpawn = players.Count;
        for (int playerId = 0; playerId < playersToSpawn; playerId++)
        {
            PrefabReplacer replacer = playerSpawns[Mathf.RoundToInt(Random.value * (playerSpawns.Count - 1))];
            playerSpawns.Remove(replacer);
            GameObject newPlayer = replacer.SpawnPrefab();
            GlobalPlayer thisPlayer = players[playerId];
            //TODO: Update the player object with animations based on thisPlayer.AvatarIndex
        }

        for (var i = 0; i < playerSpawns.Count; i++)
        {
            PrefabReplacer replacer = playerSpawns[i];
            Destroy(replacer.gameObject);
        }

        int humansToSpawn = playersToSpawn * humansToSpawnPerPlayer;
        
        for (int humanId = 0; humanId < humansToSpawn; humanId++)
        {
            PrefabReplacer replacer = humanSpawns[Mathf.RoundToInt(Random.value * (humanSpawns.Count - 1))];
            humanSpawns.Remove(replacer);
            replacer.SpawnPrefab();
        }

        for (var i = 0; i < humanSpawns.Count; i++)
        {
            PrefabReplacer replacer = humanSpawns[i];
            Destroy(replacer.gameObject);
        }

        FindObjectOfType<LeaderboardTracker>().OnGameStart();
        FindObjectOfType<GameRunner>().StartGame();
    }
}