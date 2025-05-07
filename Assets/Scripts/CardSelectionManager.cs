using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[System.Serializable]
public class TaggedCardPrefab
{
    public string tag;
    public GameObject prefab;
}

public class CardSelectionManager : MonoBehaviour
{
    [Header("Setup")]
    public Transform cardGridParent;
    public List<RecipeCard> allCards;
    public TextMeshProUGUI infoText;
    public Button continueButton;

    [Header("Card Prefabs by Tag")]
    public List<TaggedCardPrefab> cardPrefabs;

    private Dictionary<string, GameObject> prefabMap;
    public List<CardDisplay> selectedCards = new List<CardDisplay>();
    private int maxSelections = 11;
    public string gameScene = "Level1";

    [Header("Distributor")]
    public RecipeSelectionDistributor distributor;

    void Start()
    {
        // Build lookup map for tag → prefab
        prefabMap = new Dictionary<string, GameObject>();
        foreach (var entry in cardPrefabs)
            prefabMap[entry.tag] = entry.prefab;

        SpawnRandomCards(10);
        UpdateUI();
        continueButton.interactable = false;
    }

    /// <summary>
    /// Spawn count cards by selecting random RecipeCard assets and instantiating
    /// the corresponding UI prefab from the tag lookup.
    /// </summary>
    /// <summary>
    /// Spawn count cards by selecting random RecipeCard assets and instantiating
    /// the corresponding UI prefab from the tag lookup.
    /// </summary>
    void SpawnRandomCards(int count)
    {
        Debug.Log($"PrefabMap contains tags: {string.Join(", ", prefabMap.Keys)}");
        for (int i = 0; i < count; i++)
        {
            // pick random data
            RecipeCard randomCard = allCards[Random.Range(0, allCards.Count)];

            // read the Tag field from the RecipeCard ScriptableObject
            string t = randomCard.tag;  // <-- this is where we get the tag string

            // find UI prefab by tag in the dictionary built in Start()
            if (!prefabMap.TryGetValue(t, out GameObject template) || template == null)
            {
                Debug.LogWarning($"No UI prefab for tag '{t}'. Using first available.");
                template = cardPrefabs.Count > 0 ? cardPrefabs[0].prefab : null;
            }
            if (template == null)
                continue;

            // instantiate & setup
            GameObject cardObj = Instantiate(template, cardGridParent);

            CardDisplay disp = cardObj.GetComponent<CardDisplay>();
            if (disp == null)
            {
                Debug.LogError("Prefab missing CardDisplay component");
                Destroy(cardObj);
                continue;
            }
            disp.Setup(randomCard, this);

            // wire selection
            if (cardObj.TryGetComponent<Button>(out var btn))
                btn.onClick.AddListener(disp.OnClick);
        }
    }

    public void ToggleCardSelection(CardDisplay card)
    {
        if (card == null || card.cardImage == null)
            return;

        if (selectedCards.Contains(card))
        {
            selectedCards.Remove(card);
            card.cardImage.color = Color.white;
        }
        else if (selectedCards.Count < maxSelections)
        {
            selectedCards.Add(card);
            card.cardImage.color = Color.green;
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        int rem = maxSelections - selectedCards.Count;
        infoText.text = $"Choose {rem} more card(s)";
        infoText.color = Color.red;
        continueButton.interactable = (selectedCards.Count == maxSelections);
    }

    public void OnContinueButtonPressed()
    {
        if (selectedCards.Count != maxSelections) return;
        var chosen = new List<RecipeCard>();
        foreach (var cd in selectedCards)
            chosen.Add(cd.cardData);

        distributor.DistributeSelections(chosen);
        SceneManager.LoadScene(gameScene);
    }

}