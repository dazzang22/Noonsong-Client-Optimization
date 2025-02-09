using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject inventoryUI;
    public GameObject slotPrefab;
    public Transform slotContainer;
    public GameObject popupPanel;
    public TMPro.TextMeshProUGUI popupName;
    public UnityEngine.UI.Image popupImage;
    public TMPro.TextMeshProUGUI popupDescription;
    public UnityEngine.UI.Button closePopupButton;
    public UnityEngine.UI.Button inventoryToggleButton;
    public UnityEngine.UI.Button backButton;

    private List<Item> items = new List<Item>();

    [Header("Test Items")]
    public List<Item> TestItems;

    private void Start()
    {
        inventoryUI.SetActive(false);
        popupPanel.SetActive(false);
        closePopupButton.onClick.AddListener(ClosePopup);
        inventoryToggleButton.onClick.AddListener(ToggleInventory); 
        backButton.onClick.AddListener(HideInventory);

        foreach (Item item in TestItems)
        {
            AddItem(item);
        }
    }

    public void ToggleInventory()
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
    }

    public void HideInventory()
    {
        inventoryUI.SetActive(false);
    }

    public void AddItem(Item newItem)
    {
        items.Add(newItem);
        UpdateInventoryUI();
    }

    public List<Item> GetItems()
    {
        return items;
    }

    private void UpdateInventoryUI()
    {
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (Item item in items)
        {
            GameObject slot = Instantiate(slotPrefab, slotContainer);
            Slot slotScript = slot.GetComponent<Slot>();
            slotScript.Setup(item, this);
        }
    }

    public void ShowItemDetails(Item item)
    {
        popupName.text = item.itemName;
        popupImage.sprite = item.itemImage;
        popupDescription.text = item.itemDescription;
        popupPanel.SetActive(true);
    }

    private void ClosePopup()
    {
        popupPanel.SetActive(false);
    }
}