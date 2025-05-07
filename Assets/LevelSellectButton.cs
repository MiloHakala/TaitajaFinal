using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class RecipeButtonSpawner : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform spawnParent;
    public Vector3 startPosition;
    public Vector3 offset;
    public int maxSelectableCards = 3;
    public string levelName;
    public TextMeshProUGUI infoText;
    public string levelToLoad;

    private Dictionary<RecipeCard, GameObject> cardButtonMap = new Dictionary<RecipeCard, GameObject>();
    private List<RecipeCard> selectedCards = new List<RecipeCard>();

    private void Start()
    {
        infoText.enabled = false; // Hide info text at start
    }
    public void ShowCards()
    {
        if (GameManager.Instance.recipeCards.Count > 0)
        {
            Debug.Log("Cards selected, loading level...");
            LoadLevel();
        }
        else
        {
            infoText.enabled = true; // Hide info text at start
            infoText.text = "sellect the Cocking thechniques cards for  " + levelName + ". After choosing card press level button again to start upt the level";
            // Clear old buttons and selection
            foreach (Transform child in spawnParent)
                Destroy(child.gameObject);

            selectedCards.Clear();
            GameManager.Instance.recipeCards.Clear();
            cardButtonMap.Clear();

            for (int i = 0; i < GameManager.Instance.allRecipeCards.Count; i++) // Assuming allRecipeCards holds the full list
            {
                GameObject newButton = Instantiate(buttonPrefab, spawnParent);

                RectTransform rect = newButton.GetComponent<RectTransform>();
                rect.anchoredPosition = startPosition + i * offset;

                RecipeCard cardData = GameManager.Instance.allRecipeCards[i];

                TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = cardData.recipeName;

                Button btn = newButton.GetComponent<Button>();
                btn.onClick.AddListener(() => ToggleCardSelection(cardData, newButton));

                cardButtonMap[cardData] = newButton;
            }
        }
        
    }

    void ToggleCardSelection(RecipeCard card, GameObject buttonObj)
    {
        Image btnImage = buttonObj.GetComponent<Image>();

        if (selectedCards.Contains(card))
        {
            selectedCards.Remove(card);
            GameManager.Instance.recipeCards.Remove(card);
            btnImage.color = Color.white; // Deselect visual
        }
        else
        {
            if (selectedCards.Count >= maxSelectableCards)
            {
                Debug.Log("Max cards selected.");
                return;
            }

            selectedCards.Add(card);
            GameManager.Instance.recipeCards.Add(card);
            btnImage.color = Color.green; // Selected visual
        }
    }
    void LoadLevel()
    {
        // Make sure the levelToLoad is set to a valid scene name
        if (!string.IsNullOrEmpty(levelToLoad))
        {
            SceneManager.LoadScene(levelToLoad);
        }
        else
        {
            Debug.LogError("Level name is not set!");
        }
    }
}
