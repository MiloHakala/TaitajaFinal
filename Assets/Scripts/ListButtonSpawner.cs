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

    [Header("Cooking Settings")]
    public FryingPan fryingPan;
    public Transform fryingLocation;
    public Transform choppingLocation;

    public KitchenManager kitchenManager;

    void Start()
    {
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

        foreach (Transform t in pickedParent)
            Destroy(t.gameObject);

        var firstCard = allSelected[0];
        var secondCard = allSelected[1];

        SpawnPickedButton(firstCard);
        SpawnPickedButton(secondCard);

        allSelected.RemoveAt(0);
        allSelected.RemoveAt(0);
        allSelected.Add(firstCard);
        allSelected.Add(secondCard);

        var btn0 = spawnedButtons[0];
        var btn1 = spawnedButtons[1];
        spawnedButtons.RemoveAt(0);
        spawnedButtons.RemoveAt(0);
        spawnedButtons.Add(btn0);
        spawnedButtons.Add(btn1);

        btn0.transform.SetAsLastSibling();
        btn1.transform.SetAsLastSibling();

        RefreshButtons();
    }

    private void SpawnPickedButton(RecipeCard card)
    {
        if (card == null)
        {
            Debug.LogWarning("RecipeCard is null.");
            return;
        }

        var go = Instantiate(pickedButtonPrefab, pickedParent);

        // Check if button and label components are found
        var button = go.GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("Button component missing from picked button prefab.");
            return;
        }

        var label = go.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
            label.text = card.recipeName;

        // Check for null in prefab
        if (card.prefabToSpawn == null)
        {
            Debug.LogWarning($"No prefab to spawn for: {card.recipeName}");
            return;
        }

        button.onClick.AddListener(() =>
        {
            if (kitchenManager == null)
            {
                Debug.LogWarning("KitchenManager is not assigned.");
                return;
            }

            switch (card.type)
            {
                case RecipeType.FryingPan:
                    kitchenManager.hasFryingPan = true;
                    kitchenManager.UpdateKitchenStations();
                    Debug.Log("Frying Pan acquired and activated.");
                    return;

                case RecipeType.Knife:
                    kitchenManager.hasKnife = true;
                    kitchenManager.UpdateKitchenStations();
                    Debug.Log("Knife acquired and activated.");
                    return;

                case RecipeType.Cookable:
                    // Check if the frying pan is available and has the correct spot
                    FryingPan fryingPan = kitchenManager.GetFryingPan();
                    if (fryingPan == null || fryingPan.fryingSpot == null)
                    {
                        Debug.LogWarning("Frying Pan or frying spot not assigned.");
                        return;
                    }

                    GameObject cooked = Instantiate(card.prefabToSpawn, fryingPan.fryingSpot.position, Quaternion.identity);
                    cooked.transform.SetParent(fryingPan.fryingSpot);
                    fryingPan.isFrying = true;
                    Debug.Log($"Started frying {card.recipeName}");
                    break;

                case RecipeType.Choppable:
                    // Check if the chopping location is available
                    Transform chopLocation = kitchenManager.GetChoppingLocation();
                    if (chopLocation == null)
                    {
                        Debug.LogWarning("Knife not acquired. Cannot chop.");
                        return;
                    }

                    GameObject chopped = Instantiate(card.prefabToSpawn, chopLocation.position, Quaternion.identity);
                    chopped.transform.SetParent(chopLocation);
                    Debug.Log($"Chopped: {card.recipeName}");
                    break;

                default:
                    Debug.LogWarning($"Unknown RecipeType for {card.recipeName}");
                    break;
            }
        });
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
