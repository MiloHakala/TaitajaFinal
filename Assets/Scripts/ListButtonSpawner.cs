using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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

    public GameObject saltPrefab;
    public GameObject pepperPrefab;

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
        else if (card.tag == "Salt")
        {
            GameObject plate = GameObject.FindGameObjectWithTag("plate");
            GameObject pepper = Instantiate(saltPrefab, plate.transform);
            pepper.transform.localPosition = new Vector3(0, 0, 0);
            print("salt prefab spawned");

            plate.GetComponent<PlateScript>().foodOnPlate.Add(pepper);
        }
        else if (card.tag == "Pepper")
        {
            GameObject plate = GameObject.FindGameObjectWithTag("plate");
            GameObject pepper = Instantiate(pepperPrefab, plate.transform);
            pepper.transform.localPosition = new Vector3(0, 0, 0);

            plate.GetComponent<PlateScript>().foodOnPlate.Add(pepper);
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
