using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    public string mainMenuName;

    [Header("Panel References")]
    public GameObject pauseMenuUI;    // Contains Resume, Options, Main Menu, Quit buttons
    public GameObject howToPlayPanel;

    [Header("Pause Menu Elements")]
    public Button[] pauseButtons;// Pause menu buttons in order

    [Header("Animation Settings")]
    public float moveOffset = 20f;       // How far right the selected button slides
    public float scaleFactor = 1.2f;     // Scale multiplier for the selected button (and its text)
    public float transitionSpeed = 0.2f; // Duration for the smooth transition (using unscaled time)

    [Header("Audio")]
    public AudioSource buttonHoverSource;
    public AudioClip hoverClip;

    // Internal variables for current menu selection
    private Button[] currentButtons;
    private Vector2[] currentOriginalPositions;
    private Vector3[] currentOriginalScales;
    private int selectedIndex = 0;

    // Stored original positions and scales for each menu (using anchoredPosition)
    private Vector2[] pauseOriginalPositions;
    private Vector3[] pauseOriginalScales;

    private bool isPaused = false;

    void Start()
    {
        // Disable panels at start.
        pauseMenuUI.SetActive(false);
        howToPlayPanel.SetActive(false);

        // Store original positions/scales for pause menu buttons.
        if (pauseButtons.Length > 0)
        {
            pauseOriginalPositions = new Vector2[pauseButtons.Length];
            pauseOriginalScales = new Vector3[pauseButtons.Length];
            for (int i = 0; i < pauseButtons.Length; i++)
            {
                RectTransform rt = pauseButtons[i].GetComponent<RectTransform>();
                pauseOriginalPositions[i] = rt.anchoredPosition;
                pauseOriginalScales[i] = rt.localScale;
            }
        }
    }

    void Update()
    {
        // Toggle pause with Escape.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        // Always allow arrow key navigation when paused.
        if (isPaused)
        {
            HandleKeyboardNavigation();
        }
    }

    void HandleKeyboardNavigation()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = (selectedIndex + 1) % currentButtons.Length;
            UpdateSelection();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = (selectedIndex - 1 + currentButtons.Length) % currentButtons.Length;
            UpdateSelection();
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            currentButtons[selectedIndex].onClick.Invoke();
        }
    }

    // Called by pointer events (via EventTrigger) when the cursor hovers over a button.
    public void OnButtonPointerEnter(int index)
    {
        selectedIndex = index;
        UpdateSelection();
        EventSystem.current.SetSelectedGameObject(currentButtons[selectedIndex].gameObject);
    }

    // Updates the selection visuals for the current menu.
    void UpdateSelection()
    {
        for (int i = 0; i < currentButtons.Length; i++)
        {
            RectTransform rt = currentButtons[i].GetComponent<RectTransform>();
            if (i == selectedIndex)
            {
                StartCoroutine(AnimateSelection(
                    rt,
                    currentOriginalPositions[i] + new Vector2(moveOffset, 0),
                    currentOriginalScales[i] * scaleFactor,
                    true));
            }
            else
            {
                StartCoroutine(AnimateSelection(
                    rt,
                    currentOriginalPositions[i],
                    currentOriginalScales[i],
                    false));
            }
        }

        if (buttonHoverSource != null && hoverClip != null)
        {
            buttonHoverSource.PlayOneShot(hoverClip);
        }
    }

    // Animates a button’s anchoredPosition and scale.
    IEnumerator AnimateSelection(RectTransform rt, Vector2 targetPos, Vector3 targetScale, bool showBackground)
    {
        float elapsed = 0f;
        Vector2 startPos = rt.anchoredPosition;
        Vector3 startScale = rt.localScale;

        while (elapsed < transitionSpeed)
        {
            elapsed += Time.unscaledDeltaTime;
            rt.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsed / transitionSpeed);
            rt.localScale = Vector3.Lerp(startScale, targetScale, elapsed / transitionSpeed);
            yield return null;
        }

        rt.anchoredPosition = targetPos;
        rt.localScale = targetScale;
    }

    // --- Menu Management Methods ---

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f; // Pause game time (animations use unscaled time)
        pauseMenuUI.SetActive(true);

        // Set current menu arrays to pause menu.
        currentButtons = pauseButtons;
        currentOriginalPositions = new Vector2[pauseButtons.Length];
        currentOriginalScales = new Vector3[pauseButtons.Length];
        for (int i = 0; i < pauseButtons.Length; i++)
        {
            RectTransform rt = pauseButtons[i].GetComponent<RectTransform>();
            currentOriginalPositions[i] = pauseOriginalPositions[i];
            currentOriginalScales[i] = pauseOriginalScales[i];
        }
        selectedIndex = 0;
        UpdateSelection();
        EventSystem.current.SetSelectedGameObject(currentButtons[selectedIndex].gameObject);
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void ShowPauseMenu()
    {
        pauseMenuUI.SetActive(true);
        howToPlayPanel.SetActive(false);

        currentButtons = pauseButtons;
        currentOriginalPositions = new Vector2[pauseButtons.Length];
        currentOriginalScales = new Vector3[pauseButtons.Length];
        for (int i = 0; i < pauseButtons.Length; i++)
        {
            RectTransform rt = pauseButtons[i].GetComponent<RectTransform>();
            currentOriginalPositions[i] = pauseOriginalPositions[i];
            currentOriginalScales[i] = pauseOriginalScales[i];
        }
        selectedIndex = 0;
        UpdateSelection();
        EventSystem.current.SetSelectedGameObject(currentButtons[selectedIndex].gameObject);
    }

    // --- Button Methods for Pause Menu ---
    public void OnResume() { Resume(); }
    public void OnMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuName);
    }
    public void OnHowToPlay()
    {
        howToPlayPanel.SetActive(true);
        pauseMenuUI.SetActive(false);
    }
    public void OnQuit() 
    { 
        Application.Quit();
        print("Quitting...");
    }
    public void OnBack() { ShowPauseMenu(); }
}
