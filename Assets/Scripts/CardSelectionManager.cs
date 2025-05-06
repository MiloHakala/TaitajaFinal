using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CardSelectionManager : MonoBehaviour
{
    [Header("Setup")]
    public Transform cardGridParent;  // The parent where cards will be instantiated
    public GameObject cardPrefab;  // The card prefab (with Button, Image, Text)
    public List<RecipeCard> allCards;  // List of all available RecipeCards
    public TextMeshProUGUI infoText;  // The text showing how many cards to choose
    public Button continueButton;  // The button to continue when 5 cards are selected

    private List<CardDisplay> selectedCards = new List<CardDisplay>();  // List of selected cards
    private int maxSelections = 5;  // The max number of cards to select
    public string gameScene = "GameScene";

    [Header("Distributor")]
    public RecipeSelectionDistributor distributor;

    void Start()
    {
        // Spawn 10 random cards at the start
        SpawnRandomCards(10);

        // Update the UI to show the remaining number of cards to choose
        UpdateUI();

        // Make sure the continue button is initially disabled
        continueButton.interactable = false;
    }

    void SpawnRandomCards(int count)
    {
        // Spawn the cards
        for (int i = 0; i < count; i++)
        {
            // Pick a random card from the list
            RecipeCard randomCard = allCards[Random.Range(0, allCards.Count)];

            // Instantiate the card prefab
            GameObject cardObj = Instantiate(cardPrefab, cardGridParent);

            // Get the CardDisplay component attached to the card prefab
            CardDisplay cardDisplay = cardObj.GetComponent<CardDisplay>();

            // Set up the card with the random data
            cardDisplay.Setup(randomCard, this);

            // Get the Button component from the card prefab and add the OnClick listener
            Button btn = cardObj.GetComponent<Button>();
            btn.onClick.AddListener(cardDisplay.OnClick);
        }
    }

    public void ToggleCardSelection(CardDisplay card)
    {
        // Check if the card is null
        if (card == null)
        {
            Debug.LogError("Card is null! This shouldn't happen.");
            return;
        }

        // Check if the cardImage is null (this would cause a NullReferenceException)
        if (card.cardImage == null)
        {
            Debug.LogError("CardImage component is missing on the card prefab!");
            return;
        }

        // If the card is already selected, deselect it
        if (selectedCards.Contains(card))
        {
            selectedCards.Remove(card);
            card.cardImage.color = Color.white;  // Reset the card color
        }
        // If the card is not selected and we have room for more selections
        else if (selectedCards.Count < maxSelections)
        {
            selectedCards.Add(card);
            card.cardImage.color = Color.green;  // Highlight the selected card
        }

        // Update the UI after the selection changes
        UpdateUI();
    }

    void UpdateUI()
    {
        // Update the info text with the remaining number of cards to choose
        int remaining = maxSelections - selectedCards.Count;
        infoText.text = $"Choose {remaining} more card(s)";
        infoText.color = Color.red;

        // Enable the continue button when the player has selected the max number of cards
        continueButton.interactable = selectedCards.Count == maxSelections;
    }

    public void OnContinueButtonPressed()
    {
        if (selectedCards.Count != maxSelections)
            return;

        // build a List<RecipeCard> from your selected CardDisplays
        List<RecipeCard> chosen = new List<RecipeCard>();
        foreach (var cd in selectedCards)
            chosen.Add(cd.cardData);

        // hand it off
        distributor.DistributeSelections(chosen);
        SceneManager.LoadScene(gameScene);

        // now distributor.toolRecipes / seasoningRecipes / foodRecipes are populated
        // proceed to your cooking game…
    }
}
