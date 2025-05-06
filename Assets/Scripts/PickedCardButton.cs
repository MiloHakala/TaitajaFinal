using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PickedCardButton : MonoBehaviour
{
    public TextMeshProUGUI label;
    private RecipeCard cardData;

    private Transform spawnParent;
    private FryingPan fryingPan; // reference passed in Setup

    public void Setup(RecipeCard card, Transform spawnParent, FryingPan fryingPan = null)
    {
        cardData = card;
        this.spawnParent = spawnParent;
        this.fryingPan = fryingPan;

        label.text = card.recipeName;

        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (cardData.prefabToSpawn == null)
        {
            Debug.LogWarning($"No prefabToSpawn assigned on {cardData.recipeName}");
            return;
        }

        if (cardData.type == RecipeType.Cookable && fryingPan != null)
        {
            if (fryingPan.isFrying)
            {
                Debug.LogWarning("Frying pan is already in use!");
                return;
            }

            // Frying logic
            GameObject spawned = Instantiate(cardData.prefabToSpawn, fryingPan.fryingSpot.position, Quaternion.identity);
            spawned.transform.SetParent(fryingPan.fryingSpot); // optional
            fryingPan.isFrying = true;
            Debug.Log($"Started frying {cardData.recipeName}");
            return;
        }

        // Default spawn logic
        GameObject defaultSpawn = Instantiate(cardData.prefabToSpawn, spawnParent);
        defaultSpawn.transform.localPosition = Vector3.zero;
        Debug.Log($"Spawned {cardData.recipeName} under {spawnParent.name}");
    }
}
