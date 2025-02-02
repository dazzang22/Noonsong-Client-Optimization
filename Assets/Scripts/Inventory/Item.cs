using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;
    public Sprite icon;
    public int quantity;

    public Item(string name, Sprite icon, int quantity)
    {
        this.itemName = name;
        this.icon = icon;
        this.quantity = quantity;
    }
}

