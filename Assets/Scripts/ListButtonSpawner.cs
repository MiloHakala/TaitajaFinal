using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ListButtonSpawner : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public Transform gridParent;          // GridLayoutGroup for the initial 5
    public GameObject gridButtonPrefab;   // prefab for the initial‑grid buttons

    public Transform pickedParent;        // GridLayoutGroup for the picked 2
    public GameObject pickedButtonPrefab; // prefab for the picked‑grid buttons

    // runtime data
    private List<RecipeCard> allSelected;
    private List<GameObject> spawnedButtons;

    [Header("Spawn settings")]
    public Transform spawnParent;

    void Start()
    {
        var dist = RecipeSelectionDistributor.Instance;
        if (dist == null)
        {
            Debug.LogError("No RecipeSelectionDistributor found!");
            return;
        }

        allSelected = dist.toolRecipes
                       .Cast<RecipeCard>()
                       .Concat(dist.seasoningRecipes)
                       .Concat(dist.foodRecipes)
                       .ToList();

        // spawn the initial 5 using gridButtonPrefab
        spawnedButtons = new List<GameObject>();
        for (int i = 0; i < allSelected.Count && i < 5; i++)
        {
            var go = Instantiate(gridButtonPrefab, gridParent);
            spawnedButtons.Add(go);
        }
        RefreshButtons();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            PickFirstTwo();
    }

    private void PickFirstTwo()
    {
        if (allSelected.Count < 2 || spawnedButtons.Count < 2) return;

        // clear any existing picks
        foreach (Transform t in pickedParent)
            Destroy(t.gameObject);

        // take first two cards
        var firstCard = allSelected[0];
        var secondCard = allSelected[1];

        // spawn them under the picked grid
        SpawnPickedButton(firstCard);
        SpawnPickedButton(secondCard);

        // rotate your data list
        allSelected.RemoveAt(0);
        allSelected.RemoveAt(0);
        allSelected.Add(firstCard);
        allSelected.Add(secondCard);

        // rotate your button list and their sibling order
        var btn0 = spawnedButtons[0];
        var btn1 = spawnedButtons[1];
        spawnedButtons.RemoveAt(0);
        spawnedButtons.RemoveAt(0);
        spawnedButtons.Add(btn0);
        spawnedButtons.Add(btn1);

        // move those two in the hierarchy to the bottom so GridLayoutGroup re‐positions them
        btn0.transform.SetAsLastSibling();
        btn1.transform.SetAsLastSibling();

        // update all labels
        RefreshButtons();
    }
    /*
    private void SpawnPickedButton(RecipeCard card)
    {
        var go = Instantiate(pickedButtonPrefab, pickedParent);
        var label = go.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
            label.text = card.recipeName;
    }*/

    private void SpawnPickedButton(RecipeCard card)
    {
        var go = Instantiate(pickedButtonPrefab, pickedParent);

        var button = go.GetComponent<PickedCardButton>();
        if (button != null)
        {
            button.Setup(card, spawnParent); // assign data and hook up click
        }
        else
        {
            Debug.LogWarning("PickedButtonPrefab is missing the PickedCardButton component.");
        }
    }

    private void RefreshButtons()
    {
        for (int i = 0; i < spawnedButtons.Count; i++)
        {
            var label = spawnedButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
                label.text = allSelected[i].recipeName;
        }
    }
}
