using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public TMPro.TextMeshProUGUI itemNameText;
    public UnityEngine.UI.Image itemImage;
    public Item item;
    private Inventory inventory;

    public void Setup(Item newItem, Inventory inventoryManager)
    {
        item = newItem;
        inventory = inventoryManager;
        itemNameText.text = item.itemName;
        itemImage.sprite = item.itemImage;
    }

    public void OnClick()
    {
        inventory.ShowItemDetails(item);
    }
}