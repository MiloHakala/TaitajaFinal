using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class TaggedButtonPrefab
{
    public string tag;
    public GameObject prefab;
}

public class ListButtonSpawner : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public Transform gridParent;
    public Transform pickedParent;

    [Header("Button Prefabs by Tag")]
    public List<TaggedButtonPrefab> buttonPrefabs;

    // runtime data
    private List<RecipeCard> allSelected;
    private List<GameObject> spawnedGridButtons;
    private List<RecipeCard> currentPicks;

    // internal lookup
    private Dictionary<string, GameObject> prefabMap;

    void Start()
    {
        // build tag→prefab map
        prefabMap = new Dictionary<string, GameObject>();
        foreach (var entry in buttonPrefabs)
            prefabMap[entry.tag] = entry.prefab;

        var dist = RecipeSelectionDistributor.Instance;
        if (dist == null)
        {
            Debug.LogError("No RecipeSelectionDistributor found!");
            return;
        }

        // build the deck
        allSelected = dist.cookableRecipes
                       .Concat(dist.choppableRecipes)
                       .Concat(dist.seasoningRecipes)
                       .Concat(dist.fryingPanTools)
                       .Concat(dist.knifeTools)
                       .ToList();

        spawnedGridButtons = new List<GameObject>();
        currentPicks = new List<RecipeCard>();

        // spawn grid buttons for first up to 5 cards
        for (int i = 0; i < allSelected.Count && i < 5; i++)
        {
            SpawnGridButton(allSelected[i]);
        }

        // initial draw of two pick cards
        DrawInitialPicks();
        RefreshGridLabels();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            ResetPicks();
    }

    private void SpawnGridButton(RecipeCard card)
    {
        if (!prefabMap.TryGetValue(card.tag, out GameObject template) || template == null)
        {
            Debug.LogWarning($"No button prefab for tag '{card.tag}'");
            return;
        }
        var go = Instantiate(template, gridParent);
        spawnedGridButtons.Add(go);
    }

    private void DrawInitialPicks()
    {
        // clear any existing picks
        foreach (Transform t in pickedParent)
            Destroy(t.gameObject);

        currentPicks.Clear();

        // draw two cards from deck
        for (int i = 0; i < 2 && allSelected.Count > 0; i++)
        {
            RecipeCard draw = allSelected[0];
            allSelected.RemoveAt(0);
            currentPicks.Add(draw);
            SpawnPickedButton(draw);
        }
    }

    private void ResetPicks()
    {
        // put current picks back to bottom in order
        foreach (var card in currentPicks)
            allSelected.Add(card);
        DrawInitialPicks();
        RefreshGridLabels();
    }

    private void SpawnPickedButton(RecipeCard card)
    {
        if (card == null) return;

        if (!prefabMap.TryGetValue(card.tag, out GameObject template) || template == null)
        {
            Debug.LogWarning($"No button prefab for tag '{card.tag}'");
            return;
        }
        var go = Instantiate(template, pickedParent);

        var button = go.GetComponent<Button>();
        var label = go.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null) label.text = card.recipeName;

        // on click, handle pick usage and replacement
        button.onClick.AddListener(() => OnPickedButtonClicked(card, go));
    }

    private void OnPickedButtonClicked(RecipeCard card, GameObject buttonGO)
    {
        // perform the card's action
        HandleCardAction(card);

        // remove the clicked pick button
        Destroy(buttonGO);

        // remove this card from current picks
        currentPicks.Remove(card);

        // move used card to bottom of deck
        allSelected.Add(card);

        // draw one new card from top of deck
        if (allSelected.Count > 0)
        {
            RecipeCard newDraw = allSelected[0];
            allSelected.RemoveAt(0);
            currentPicks.Add(newDraw);
            SpawnPickedButton(newDraw);
        }

        RefreshGridLabels();
    }

    private void HandleCardAction(RecipeCard card)
    {
        switch (card.tag)
        {
            case "Fryingpan":
                GameObject.FindGameObjectWithTag("stove").GetComponent<stove>().spawnPan();
                break;
            case "Beef":
                GameObject.FindGameObjectWithTag("stove").GetComponent<stove>().SpawnBeef();
                break;
            case "Chicken":
                GameObject.FindGameObjectWithTag("stove").GetComponent<stove>().SpawnChicken();
                break;
            case "Letuce":
                GameObject.FindGameObjectWithTag("choppingArea").GetComponent<ChoppingBlock>().SpawnLetuce();
                break;
        }
    }

    private void RefreshGridLabels()
    {
        for (int i = 0; i < spawnedGridButtons.Count; i++)
        {
            var label = spawnedGridButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (label != null && i < allSelected.Count)
                label.text = allSelected[i].recipeName;
        }
    }
}
