using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class UpgradeButtons
{
    public string tag;
    public GameObject prefab;
}

public class UpgradePanel : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public List<UpgradeButtons> upgradeButtons;  // order: Beef, Chicken, Salad...
    public Transform upgradeParent;                  // where to spawn buttons
    public int upgradeCost = 150;                    // cost per upgrade

    private List<GameObject> spawnedButtons = new List<GameObject>();

    void Start()
    {
        SpawnAllUpgradeButtons();
    }

    void SpawnAllUpgradeButtons()
    {
        foreach (var tb in upgradeButtons)
        {
            var btnGO = Instantiate(tb.prefab, upgradeParent);
            btnGO.tag = "UpgradeButton";

            // set its text child
            var tm = btnGO.GetComponentInChildren<TextMeshProUGUI>();
            if (tm != null) tm.text = $"Upgrade {tb.tag} (${upgradeCost})";

            string thisTag = tb.tag;
            btnGO.GetComponent<Button>()
                 .onClick.AddListener(() => OnUpgradeButtonClicked(thisTag));

            spawnedButtons.Add(btnGO);
        }
    }

    void OnUpgradeButtonClicked(string tag)
    {
        // check cost
        if (MoneyManager.Instance.money < upgradeCost)
        {
            Debug.Log("Not enough money for upgrade!");
            return;
        }

        // spend money
        MoneyManager.Instance.Spend(upgradeCost);

        // apply global upgrade
        var allItems = Resources.FindObjectsOfTypeAll<ItemData>();
        foreach (var item in allItems)
        {
            if (item.tag == tag)
                item.moneyUpgradeAcquired = true;
        }
        Debug.Log($"Global upgrade acquired for all '{tag}' items.");
    }
}
