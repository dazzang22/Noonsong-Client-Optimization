using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GiftSlot : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public Image itemImage;
    private ItemEntry item;
    private GiftInventory giftInventory;

    public void Setup(ItemEntry newItem, GiftInventory inventory)
    {
        item = newItem;
        giftInventory = inventory;
        itemNameText.text = item.itemName;
        itemImage.sprite = item.itemSprite;
    }

    public void OnClick()
    {
        giftInventory.ShowGiftPopup(item);
    }
}