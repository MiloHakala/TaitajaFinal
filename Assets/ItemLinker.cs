// ItemLinker.cs
using UnityEngine;

public class ItemLinker : MonoBehaviour
{
    [Tooltip("Assign the ItemData asset for this prefab")]
    public ItemData itemData;

    // Expose tag for runtime lookup
    public string Tag => itemData.tag;

    // Call when customer buys this item
    public void OnSold()
    {
        int earned = itemData.SellAmount;
        GameManager.Instance.AddMoney(earned);
        Debug.Log($"{itemData.tag} sold for ${earned}");
        Destroy(gameObject);
    }
}
