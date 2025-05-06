using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public Image cardImage;
    public TextMeshProUGUI cardLabel;
    public RecipeCard cardData;

    private CardSelectionManager selectionManager;

    public void Setup(RecipeCard data, CardSelectionManager manager)
    {
        cardData = data;
        selectionManager = manager;

        if (cardImage != null)
            cardImage.sprite = cardData.cardImage;

        if (cardLabel != null)
            cardLabel.text = cardData.recipeName;
    }

    public void OnClick()
    {
        selectionManager.ToggleCardSelection(this);
    }
}
