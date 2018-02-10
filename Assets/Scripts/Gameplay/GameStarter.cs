using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameStarter : MonoBehaviour
{
    public string[] availableLevelScenes;

    void Start()
    {
        string newScene = availableLevelScenes[Mathf.RoundToInt(Random.value * (availableLevelScenes.Length - 1))];
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        Debug.Log("Loading " + newScene);
        StartCoroutine(LoadNewScene(newScene));
    }

    private IEnumerator LoadNewScene(string newScene) {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadSceneAsync(newScene);
        yield return new WaitForSeconds(.5f);
    }

}