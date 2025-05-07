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

    [Header("Upgrade Overlay")]
    public GameObject upgradeButtonPrefab;      // small overlay button prefab (tagged "UpgradeButton")

    // runtime data
    private List<RecipeCard> allSelected;
    private List<GameObject> spawnedButtons;

    [Header("Spawn settings")]
    public Transform spawnParent;               // fallback spawn parent

    [Header("Cooking Settings")]
    //public FryingPan fryingPan;
    //public Transform fryingLocation;
    //public Transform choppingLocation;

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

        // clear previous picked
        foreach (Transform t in pickedParent)
            Destroy(t.gameObject);

        // grab first two cards
        var firstCard = allSelected[0];
        var secondCard = allSelected[1];

        // spawn their buttons
        SpawnPickedButton(firstCard);
        SpawnPickedButton(secondCard);

        // rotate data list
        allSelected.RemoveRange(0, 2);
        allSelected.Add(firstCard);
        allSelected.Add(secondCard);

        // rotate button list
        var btn0 = spawnedButtons[0];
        var btn1 = spawnedButtons[1];
        spawnedButtons.RemoveRange(0, 2);
        spawnedButtons.Add(btn0);
        spawnedButtons.Add(btn1);

        // ensure they render on top in grid (if needed)
        btn0.transform.SetAsLastSibling();
        btn1.transform.SetAsLastSibling();

        RefreshButtons();
    }

    private void SpawnPickedButton(RecipeCard card)
    {
        if (card == null) return;

        // --- spawn the pick‐button as before ---
        if (!prefabMap.TryGetValue(card.tag, out GameObject template) || template == null)
        {
            Debug.LogWarning($"No button prefab for tag '{card.tag}'");
            return;
        }
        var go = Instantiate(template, pickedParent);

        // set label & click
        var button = go.GetComponent<Button>();
        var label = go.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null) label.text = card.recipeName;
        button.onClick.AddListener(() => OnPickedButtonClicked(card));

        // --- now spawn the upgrade button in the same parent ---
        var up = Instantiate(upgradeButtonPrefab, pickedParent);
        up.tag = "UpgradeButton";

        // position it above its sibling pick-button
        var goRt = go.GetComponent<RectTransform>();
        var upRt = up.GetComponent<RectTransform>();
        upRt.anchorMin = goRt.anchorMin;
        upRt.anchorMax = goRt.anchorMax;
        upRt.pivot = goRt.pivot;
        upRt.sizeDelta = goRt.sizeDelta;
        upRt.anchoredPosition = goRt.anchoredPosition + new Vector2(0, goRt.sizeDelta.y * 0.5f + 10f);

        // set the TMP text child to "Upgrade: [ItemName]"
        var upLabel = up.GetComponentInChildren<TextMeshProUGUI>();
        if (upLabel != null)
            upLabel.text = $"Upgrade: {card.recipeName}";

        // hook its click
        var upBtn = up.GetComponent<Button>();
        upBtn.onClick.AddListener(() => OnUpgradeClicked(card));
    }


    private void OnUpgradeClicked(RecipeCard card)
    {
        // set upgrade globally for all matching ItemData assets
        var allItems = Resources.FindObjectsOfTypeAll<ItemData>();
        foreach (var item in allItems)
        {
            if (item.tag == card.tag)
                item.moneyUpgradeAcquired = true;
        }
        Debug.Log($"Global upgrade acquired for all '{card.tag}' items.");

        // remove all upgrade overlay buttons
        var upgrades = pickedParent.GetComponentsInChildren<Transform>()
                       .Where(t => t.CompareTag("UpgradeButton"))
                       .Select(t => t.gameObject).ToList();
        foreach (var u in upgrades)
            Destroy(u);
    }


    private void OnPickedButtonClicked(RecipeCard card)
    {
        Debug.Log($"Button clicked for tag: {card.tag}");
        if (card.tag == "Fryingpan")
        {
            GameObject stoveObj = GameObject.FindGameObjectWithTag("stove");
            stove stoveScript = stoveObj.GetComponent<stove>();
            stoveScript.spawnPan();
        }
        else if (card.tag == "Beef")
        {
            GameObject stoveObj = GameObject.FindGameObjectWithTag("stove");
            stove stoveScript = stoveObj.GetComponent<stove>();
            stoveScript.SpawnBeef();
        }
        else if (card.tag == "Chicken")
        {
            GameObject stoveObj = GameObject.FindGameObjectWithTag("stove");
            stove stoveScript = stoveObj.GetComponent<stove>();
            stoveScript.SpawnChicken();
        }
        else if (card.tag == "Letuce")
        {
            GameObject stoveObj = GameObject.FindGameObjectWithTag("choppingArea");
            ChoppingBlock choppingBlock = stoveObj.GetComponent<ChoppingBlock>();
            choppingBlock.SpawnLetuce();
        }
        RefreshButtons();
        /*switch (card.type)
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
            }*/
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
