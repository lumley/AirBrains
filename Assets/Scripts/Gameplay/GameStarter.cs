using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameStarter : MonoBehaviour
{
    public string[] availableLevelScenes;

    void Start()
    {
        string newScene = availableLevelScenes[Mathf.RoundToInt(Random.value * (availableLevelScenes.Length - 1))];
        Debug.Log("Loading " + newScene);
        StartCoroutine(LoadNewScene(newScene));
    }

    private IEnumerator LoadNewScene(string newScene) {
        var result = SceneManager.LoadSceneAsync(newScene);
        while (!result.isDone) 
        {
            Debug.Log ( "progress: " + result.progress );
            yield return new WaitForEndOfFrame ();
        }
    }

}