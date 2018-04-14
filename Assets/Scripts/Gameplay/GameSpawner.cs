using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScreenLogic;
using NDream.AirConsole;

public class GameSpawner : MonoBehaviour
{
    public int[] humansToSpawnPerPlayer; 

    public Canvas effectCanvas;
    public SpawnPositionEffect spawnPositionEffect;

    public static GameSpawner FindInScene()
    {
        return FindObjectOfType<GameSpawner>();
    }


    public void StartGame(List<GlobalPlayer> players)
    {
        AirConsoleBridge.Instance.BroadcastLoadingScreen("WATCH THE MAIN SCREEN");
        if (players == null)
        {
            Debug.LogError("NO PLAYERS PROVIDED!");
        }
        if (players.Count <= 0)
        {
            Debug.LogError("NO PLAYERS GIVEN!");
        }
        List<PrefabReplacer> humanSpawns = new List<PrefabReplacer>();
        List<PrefabReplacer> playerSpawns = new List<PrefabReplacer>();
        foreach (PrefabReplacer replacer in GameObject.FindObjectsOfType<PrefabReplacer>())
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

        if (humanSpawns.Count < 0)
        {
            Debug.LogError("NO HUMAN SPAWNS FOUND!");
        }
        if (playerSpawns.Count < 0)
        {
            Debug.LogError("NO PLAYER SPAWNS FOUND!");
        }


        int playersToSpawn = players.Count;
        for (int playerId = 0; playerId < playersToSpawn; playerId++)
        {
            int replacerIndex = Mathf.RoundToInt(Random.value * (playerSpawns.Count - 1));
            while (replacerIndex >= playerSpawns.Count)
            {
                replacerIndex = Mathf.RoundToInt(Random.value * (playerSpawns.Count - 1));
            }
            PrefabReplacer replacer = playerSpawns[replacerIndex];
            playerSpawns.Remove(replacer);
            GameObject newPlayer = replacer.SpawnPrefab();
            GlobalPlayer thisPlayer = players[playerId];
            newPlayer.GetComponent<MonsterAnimationController>().SetCharacter(thisPlayer.LobbyPlayerData.Character);
            newPlayer.GetComponent<ScoreTracker>().Character = thisPlayer.LobbyPlayerData.Character;

            SpawnPositionEffect effect = Instantiate(spawnPositionEffect);
            effect.transform.SetParent(effectCanvas.transform);
            effect.transform.localScale = Vector3.one;
            Debug.Log("New Spawn effect! " + thisPlayer.LobbyPlayerData.Character + " " +  newPlayer + " " +  AirConsole.instance.GetNickname(thisPlayer.LobbyPlayerData.Id));
            effect.SetCharacter(thisPlayer.LobbyPlayerData.Character, newPlayer, AirConsole.instance.GetNickname(thisPlayer.LobbyPlayerData.Id), playerId * effect.displayTime);
        }
        float totalDisplayTime = spawnPositionEffect.displayTime * playersToSpawn;
        foreach (PrefabReplacer replacer in playerSpawns)
        {
            Destroy(replacer.gameObject);
        }
        int humansToSpawn = humansToSpawnPerPlayer[playersToSpawn-1];
        for (int humanId = 0; humanId < humansToSpawn; humanId++)
        {
            PrefabReplacer replacer = humanSpawns[Mathf.RoundToInt(Random.value * (humanSpawns.Count - 1))];
            humanSpawns.Remove(replacer);
            replacer.SpawnPrefab();
        }
        foreach (PrefabReplacer replacer in humanSpawns)
        {
            Destroy(replacer.gameObject);
        }

        GameObject.FindObjectOfType<LeaderboardTracker>().OnGameStart();

        StartCoroutine(StartInASecond(totalDisplayTime));
    }

    private IEnumerator StartInASecond(float spawnDisplayTime)
    {
        yield return new WaitForSeconds(spawnDisplayTime + .5f);
        GameObject.FindObjectOfType<GameRunner>().StartGame();
    }
}