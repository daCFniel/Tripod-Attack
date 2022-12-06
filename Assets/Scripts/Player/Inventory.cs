using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Daniel Bielech db662 - COMP6100
/// Attachable to an Inventory game object which is a child of a Player object
/// </summary>
public class Inventory : MonoBehaviour
{
    public List<InventoryItem> inventory;
    [SerializeField] GameObject itemInfoPanel;
    [SerializeField] TextMeshProUGUI itemInfoText = default;
    [SerializeField] Image itemInfoImage = default;
    [SerializeField] int displayInfoTime = 5;

    private void Awake()
    {
        inventory = new List<InventoryItem>();
    }
    private void OnEnable()
    {
        TestItem.OnCollect += Add;

    }
    private void OnDisable()
    {
        TestItem.OnCollect -= Add;
    }
    public void Add(ItemData itemData)
    {
        InventoryItem newItem = new(itemData);
        inventory.Add(newItem);
        Debug.Log($"Added {itemData.displayName} to the Inventory.");
        // Display information about the item
        itemInfoImage.sprite = itemData.icon;
        itemInfoText.text = $"You just picked {itemData.displayName}. \n {itemData.description}";
        itemInfoPanel.SetActive(true);
        StartCoroutine(HideItemInfoDialog());
    }

    public void Remove(ItemData itemData)
    {
        InventoryItem item = new(itemData);
        if (inventory.Contains(item))
        {
            inventory.Remove(item);
        }
    }

    private IEnumerator HideItemInfoDialog()
    {
        yield return new WaitForSeconds(displayInfoTime);
        itemInfoPanel.SetActive(false);
    }
}
