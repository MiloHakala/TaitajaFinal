using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PickedCardButton : MonoBehaviour
{
    public TextMeshProUGUI label;
    private RecipeCard cardData;

    // Parent under which spawned prefabs will be organized
    private Transform spawnParent;

    /// <summary>
    /// Called by ListButtonSpawner when creating each picked‑grid button.
    /// </summary>
    public void Setup(RecipeCard card, Transform spawnParent)
    {
        cardData = card;
        this.spawnParent = spawnParent;
        label.text = card.recipeName;

        // Add click listener
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (cardData.prefabToSpawn != null)
        {
            // Instantiate as child of spawnParent
            GameObject spawned = Instantiate(cardData.prefabToSpawn, spawnParent);
            // Optionally reset local position
            spawned.transform.localPosition = Vector3.zero;
            Debug.Log($"Spawned {cardData.prefabToSpawn.name} under {spawnParent.name}");
        }
        else
        {
            Debug.LogWarning($"No prefabToSpawn assigned on {cardData.recipeName}");
        }
    }
}
