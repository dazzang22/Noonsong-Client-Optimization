using System;
using UnityEngine;

[Serializable]
public class UserInventoryEntry
{
    public string userId;     
    public int itemId;  
    public int itemCount;
    private static ItemEntry[] cachedItems = null;

    public UserInventoryEntry(string userId, int itemId, int count)
    {
        this.userId = userId;
        this.itemId = itemId;
        this.itemCount = count;
    }

    public ItemEntry GetItemEntry()
    {
        if (cachedItems == null)
        {
            cachedItems = Resources.LoadAll<ItemEntry>("ItemEntries");
        }

        foreach (var item in cachedItems)
        {
            if (item.itemID == this.itemId)
            {
                return item;
            }
        }
        return null;
    }
}
