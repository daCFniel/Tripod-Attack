
using UnityEngine;
using System;
/// <summary>
/// Daniel Bielech db662 - COMP6100
/// </summary>

[Serializable]
public class InventoryItem
{
    public ItemData itemData;

    public InventoryItem(ItemData itemData)
    {
        this.itemData = itemData;
    }
}
