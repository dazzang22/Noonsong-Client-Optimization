using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public Transform itemListParent;
    public GameObject itemSlotPrefab;

    private Item selectedItem;

    public void RefreshUI()
    {
        foreach (Transform child in itemListParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Item item in inventory.items)
        {
            GameObject slot = Instantiate(itemSlotPrefab, itemListParent);
            slot.transform.Find("ItemIcon").GetComponent<Image>().sprite = item.icon;
            slot.transform.Find("ItemName").GetComponent<Text>().text = item.itemName;
            slot.transform.Find("ItemQuantity").GetComponent<Text>().text = "x" + item.quantity;

            Button button = slot.GetComponent<Button>();
            button.onClick.AddListener(() => SelectItem(item));
        }
    }

    private void SelectItem(Item item)
    {
        selectedItem = item;
        Debug.Log(item.itemName + " º±≈√µ ");
    }

    public Item GetSelectedItem()
    {
        return selectedItem;
    }
}