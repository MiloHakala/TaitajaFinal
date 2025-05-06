using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsScroller : MonoBehaviour
{
    public RectTransform creditsText; // The UI text or container to scroll
    public float scrollDuration = 10f; // Total scroll time in seconds
    public float scrollDistance = 800f; // How far to move upward
    public string returnScene = "MainMenu";

    private Vector2 startPos;
    private float elapsedTime;

    void Start()
    {
        startPos = creditsText.anchoredPosition;
        elapsedTime = 0f;
    }

    void Update()
    {
        // existing code ...
        if (elapsedTime >= scrollDuration)
            SceneManager.LoadScene(returnScene); // Add using UnityEngine.SceneManagement;
        if (elapsedTime < scrollDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / scrollDuration);
            creditsText.anchoredPosition = startPos + Vector2.up * scrollDistance * t;
        }
    }
}
