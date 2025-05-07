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
    public Transform gridParent;                // GridLayoutGroup for the initial 5
    public Transform pickedParent;              // GridLayoutGroup for the picked 2

    [Header("Button Prefabs by Tag")]
    public List<TaggedButtonPrefab> buttonPrefabs;  // All possible button prefabs, keyed by tag

    // runtime data
    private List<RecipeCard> allSelected;
    private List<GameObject> spawnedButtons;

    [Header("Spawn settings")]
    public Transform spawnParent;               // fallback spawn parent

    [Header("Cooking Settings")]
    public FryingPan fryingPan;
    public Transform fryingLocation;
    public Transform choppingLocation;

    public KitchenManager kitchenManager;

    // internal lookup
    private Dictionary<string, GameObject> prefabMap;

    void Start()
    {
        // build tag→prefab map
        prefabMap = new Dictionary<string, GameObject>();
        foreach (var entry in buttonPrefabs)
            prefabMap[entry.tag] = entry.prefab;

        // get selected cards
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

        // spawn initial grid
        spawnedButtons = new List<GameObject>();
        for (int i = 0; i < allSelected.Count && i < 5; i++)
        {
            SpawnGridButton(allSelected[i], i);
        }
        RefreshButtons();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            PickFirstTwo();
    }

    private void SpawnGridButton(RecipeCard card, int index)
    {
        // lookup prefab by card.tag
        if (!prefabMap.TryGetValue(card.tag, out GameObject template) || template == null)
        {
            Debug.LogWarning($"No button prefab for tag '{card.tag}'");
            return;
        }
        var go = Instantiate(template, gridParent);
        spawnedButtons.Add(go);
    }

    private void PickFirstTwo()
    {
        if (allSelected.Count < 2 || spawnedButtons.Count < 2) return;

        foreach (Transform t in pickedParent)
            Destroy(t.gameObject);

        var firstCard = allSelected[0];
        var secondCard = allSelected[1];

        SpawnPickedButton(firstCard);
        SpawnPickedButton(secondCard);

        // rotate data & buttons
        allSelected.RemoveRange(0, 2);
        allSelected.Add(firstCard);
        allSelected.Add(secondCard);

        var btn0 = spawnedButtons[0];
        var btn1 = spawnedButtons[1];
        spawnedButtons.RemoveRange(0, 2);
        spawnedButtons.Add(btn0);
        spawnedButtons.Add(btn1);

        btn0.transform.SetAsLastSibling();
        btn1.transform.SetAsLastSibling();

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
        switch (card.type)
        {
            case RecipeType.FryingPan:
                kitchenManager.hasFryingPan = true;
                kitchenManager.UpdateKitchenStations();
                break;
            case RecipeType.Knife:
                kitchenManager.hasKnife = true;
                kitchenManager.UpdateKitchenStations();
                break;
            case RecipeType.Cookable:
                var pan = kitchenManager.GetFryingPan();
                if (pan != null && !pan.isFrying)
                {
                    var cooked = Instantiate(card.prefabToSpawn, fryingLocation.position, Quaternion.identity);
                    cooked.transform.SetParent(fryingLocation);
                    pan.isFrying = true;
                }
                break;
            case RecipeType.Choppable:
                var loc = kitchenManager.GetChoppingLocation();
                if (loc != null)
                {
                    var chopped = Instantiate(card.prefabToSpawn, choppingLocation.position, Quaternion.identity);
                    chopped.transform.SetParent(choppingLocation);
                }
                break;
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
