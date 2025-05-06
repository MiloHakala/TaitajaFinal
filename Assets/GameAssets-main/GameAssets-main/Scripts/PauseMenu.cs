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

    private bool isPaused = false;

    void Start()
    {
        // Disable panels at start.
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
       
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }
            
        /*
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
        }*/
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
    }
    
    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f; // Pause game time (animations use unscaled time)
        pauseMenuUI.SetActive(true);
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
