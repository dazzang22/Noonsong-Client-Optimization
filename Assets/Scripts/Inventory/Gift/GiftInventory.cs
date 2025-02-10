using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftInventory : MonoBehaviour
{
    public GameObject giftInventoryUI;
    public GameObject giftSlotPrefab;
    public Transform giftSlotContainer;

    private List<ItemEntry> giftItems = new List<ItemEntry>();
    private InventoryManager inventoryManager;
    private EncounterUI encounterUI;

    private ItemEntry selectedGiftItem;
    public GameObject giftPopup;

    public void Initialize(InventoryManager inventory, EncounterUI ui)
    {
        inventoryManager = inventory;
        encounterUI = ui;
        SyncWithInventoryManager();
    }

    public void ToggleGiftInventory()
    {
        giftInventoryUI.SetActive(!giftInventoryUI.activeSelf);
        giftPopup.SetActive(false);
    }

    public void ShowGiftPopup(ItemEntry item)
    {
        selectedGiftItem = item;
        encounterUI.ShowGiftPopup(item);
    }

    public void GiveGift()
    {
        if (selectedGiftItem != null)
        {
            string university = encounterUI.GetCurrentNoonsongUniversity();
            ItemEntry.PreferenceLevel preference = selectedGiftItem.GetPreferenceForDepartment(university);

            int affectionChange = 1;
            switch (preference)
            {
                case ItemEntry.PreferenceLevel.Love:
                    affectionChange = 5;
                    break;
                case ItemEntry.PreferenceLevel.Like:
                    affectionChange = 3;
                    break;
                case ItemEntry.PreferenceLevel.Dislike:
                    affectionChange = 0;
                    break;
            }

            encounterUI.UpdateNoonsongAffection(affectionChange);

            selectedGiftItem.itemCount--;
            if (selectedGiftItem.itemCount <= 0)
            {
                giftItems.Remove(selectedGiftItem);
            }

            inventoryManager.UpdateInventory();
            SyncWithInventoryManager();
            encounterUI.GiveGift(selectedGiftItem);
            giftPopup.SetActive(false);
        }
    }

    public void SyncWithInventoryManager()
    {
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager가 연결되지 않았습니다");
            return;
        }

        giftItems = new List<ItemEntry>();
        foreach (var item in inventoryManager.itemEntries)
        {
            if (item.itemCount > 0)
            {
                giftItems.Add(item);
            }
        }
        UpdateGiftInventoryUI();
    }

    private void UpdateGiftInventoryUI()
    {
        foreach (Transform child in giftSlotContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (ItemEntry item in giftItems)
        {
            GameObject slot = Instantiate(giftSlotPrefab, giftSlotContainer);
            GiftSlot slotScript = slot.GetComponent<GiftSlot>();
            slotScript.Setup(item, this);
        }
    }
}