using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardSelectionManager : MonoBehaviour
{
    [Header("Setup")]
    public Transform cardGridParent;
    public GameObject cardPrefab;
    public List<RecipeCard> allCards;
    public TextMeshProUGUI infoText;
    public Button continueButton;

    private List<CardDisplay> selectedCards = new();
    private int maxSelections = 5;

    void Start()
    {
        SpawnRandomCards(10);
        UpdateUI();
        continueButton.interactable = false;
    }

    void SpawnRandomCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            RecipeCard randomCard = allCards[Random.Range(0, allCards.Count)];
            GameObject cardObj = Instantiate(cardPrefab, cardGridParent);
            CardDisplay cardDisplay = cardObj.GetComponent<CardDisplay>();
            cardDisplay.Setup(randomCard, this);

            Button btn = cardObj.GetComponent<Button>();
            btn.onClick.AddListener(cardDisplay.OnClick);
        }
    }

    public void ToggleCardSelection(CardDisplay card)
    {
        if (selectedCards.Contains(card))
        {
            selectedCards.Remove(card);
            card.GetComponent<Image>().color = Color.white;
        }
        else if (selectedCards.Count < maxSelections)
        {
            selectedCards.Add(card);
            card.GetComponent<Image>().color = Color.green;
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        int remaining = maxSelections - selectedCards.Count;
        infoText.text = $"Choose {remaining} more card(s)";
        infoText.color = Color.red;
        continueButton.interactable = selectedCards.Count == maxSelections;
    }

    public void OnContinueButtonPressed()
    {
        if (selectedCards.Count == maxSelections)
        {
            // Store selected cards globally or via DontDestroyOnLoad
            // Then load cooking game scene
            Debug.Log("Proceeding with selected cards...");
        }
    }

    public List<RecipeCard> GetSelectedRecipes()
    {
        return selectedCards.ConvertAll(card => card.cardData);
    }
}
