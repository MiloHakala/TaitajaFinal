using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
    // Call this to load a scene with delay
    public void LoadScene(string sceneName)
    {
        StartCoroutine(DelayedSceneLoad(sceneName));
    }

    // Call this to quit the game with delay
    public void QuitGame()
    {
        StartCoroutine(DelayedQuit());
    }

    private System.Collections.IEnumerator DelayedSceneLoad(string sceneName)
    {
        yield return new WaitForSeconds(0.4f);
        SceneManager.LoadScene(sceneName);
    }

    private System.Collections.IEnumerator DelayedQuit()
    {
        yield return new WaitForSeconds(0.4f);
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
