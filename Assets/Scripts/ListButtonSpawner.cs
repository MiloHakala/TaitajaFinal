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
    private List<GameObject> spawnedButtons;

    // internal lookup
    private Dictionary<string, GameObject> prefabMap;

    // number of cards to pick each time
    private const int PickCount = 4;

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
        allSelected = dist.cookableRecipes
                       .Concat(dist.choppableRecipes)
                       .Concat(dist.seasoningRecipes)
                       .Concat(dist.fryingPanTools)
                       .Concat(dist.knifeTools)
                       .ToList();

        spawnedButtons = new List<GameObject>();
        for (int i = 0; i < allSelected.Count && i < 10; i++)
        {
            SpawnGridButton(allSelected[i]);
        }
        RefreshButtons();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            PickFirstFour();
    }

    private void SpawnGridButton(RecipeCard card)
    {
        if (!prefabMap.TryGetValue(card.tag, out GameObject template) || template == null)
        {
            Debug.LogWarning($"No button prefab for tag '{card.tag}'");
            return;
        }
        var go = Instantiate(template, gridParent);
        spawnedButtons.Add(go);
    }

    private void PickFirstFour()
    {
        if (allSelected.Count < PickCount || spawnedButtons.Count < PickCount) return;

        // Clear previously picked
        foreach (Transform t in pickedParent)
            Destroy(t.gameObject);

        // Grab the top N cards
        var picks = allSelected.Take(PickCount).ToList();

        // Display each picked card
        foreach (var card in picks)
            SpawnPickedButton(card);

        // Move those cards to bottom of deck
        allSelected.RemoveRange(0, PickCount);
        allSelected.AddRange(picks);

        // Cycle the corresponding grid buttons
        var btns = spawnedButtons.Take(PickCount).ToList();
        spawnedButtons.RemoveRange(0, PickCount);
        spawnedButtons.AddRange(btns);
        foreach (var btn in btns)
            btn.transform.SetAsLastSibling();

        RefreshButtons();
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
        button.onClick.AddListener(() => OnPickedButtonClicked(card));
    }

    private void OnPickedButtonClicked(RecipeCard card)
    {
        Debug.Log($"Button clicked for tag: {card.tag}");
        if (card.tag == "Fryingpan")
        {
            GameObject.FindGameObjectWithTag("stove").GetComponent<stove>().spawnPan();
        }
        else if (card.tag == "Beef")
        {
            GameObject.FindGameObjectWithTag("stove").GetComponent<stove>().SpawnBeef();
        }
        else if (card.tag == "Chicken")
        {
            GameObject.FindGameObjectWithTag("stove").GetComponent<stove>().SpawnChicken();
        }
        else if (card.tag == "Letuce")
        {
            GameObject.FindGameObjectWithTag("choppingArea").GetComponent<ChoppingBlock>().SpawnLetuce();
        }
        RefreshButtons();
    }

    private void RefreshButtons()
    {
        for (int i = 0; i < spawnedButtons.Count; i++)
        {
            var label = spawnedButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (label != null && i < allSelected.Count)
                label.text = allSelected[i].recipeName;
        }
    }
}
