// ItemData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Data/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    public string tag;                     // e.g. "Beef", "Chicken", etc.

    [Header("Sale Settings")]
    public int defaultSellAmount = 50;
    public bool moneyUpgradeAcquired = false;

    public int SellAmount => moneyUpgradeAcquired ? 200 : defaultSellAmount;
}
