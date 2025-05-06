using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public Image cardImage;  // Card image component (child of CardPrefab)
    public TextMeshProUGUI cardLabel;  // Card label component (child of CardPrefab)
    public RecipeCard cardData;  // The card's data (from RecipeCard)

    private CardSelectionManager selectionManager;  // Reference to the manager

    public void Setup(RecipeCard data, CardSelectionManager manager)
    {
        // Set the card data and selection manager
        cardData = data;
        selectionManager = manager;

        // Set up the image and label from the card data
        if (cardImage != null)
            cardImage.sprite = cardData.cardImage;

        if (cardLabel != null)
            cardLabel.text = cardData.recipeName;
    }

    // Called when the button is clicked
    public void OnClick()
    {
        // Call the selection manager's toggle selection method
        selectionManager.ToggleCardSelection(this);
    }
}
