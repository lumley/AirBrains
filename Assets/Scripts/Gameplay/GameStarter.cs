using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviour
{
    public string[] availableLevelScenes;

    void Start()
    {
        string newScene = availableLevelScenes[Mathf.RoundToInt(Random.value * (availableLevelScenes.Length - 1))];
        Debug.Log("Loading " + newScene);
        SceneManager.LoadScene(newScene);
    }
}