using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GiftInventory : MonoBehaviour
{
    public GameObject inventoryUI;
    public Transform inventorySlotContainer;
    public GameObject inventorySlotPrefab;

    public GameObject itemPopupUI; // 아이템 팝업 UI
    public TextMeshProUGUI itemNameText; // 아이템 이름 텍스트
    public Image itemImage; // 아이템 이미지
    public TextMeshProUGUI itemDescriptionText; // 아이템 설명 텍스트
    public Button popupGiftButton; // 팝업의 선물하기 버튼
    public Button popupCloseButton; // 팝업의 닫기 버튼

    private Inventory inventory;
    private Item selectedItem;

    void Start()
    {
        inventory = FindObjectOfType<Inventory>();

        if (inventory == null)
        {
            Debug.LogError("Inventory 스크립트를 찾을 수 없습니다.");
        }

        popupGiftButton.onClick.AddListener(GiveGift);
        popupCloseButton.onClick.AddListener(CloseItemPopup);
    }

    public void OpenInventory()
    {
        inventoryUI.SetActive(true);
        UpdateInventoryUI();
    }

    public void CloseInventory()
    {
        inventoryUI.SetActive(false);
    }

    private void UpdateInventoryUI()
    {
        foreach (Transform child in inventorySlotContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (Item item in inventory.GetItems())
        {
            GameObject slot = Instantiate(inventorySlotPrefab, inventorySlotContainer);
            Slot slotScript = slot.GetComponent<Slot>();
            slotScript.Setup(item, inventory);

            Button slotButton = slot.GetComponent<Button>();
            slotButton.onClick.AddListener(() => OpenItemPopup(item));
        }
    }

    public void OpenItemPopup(Item item)
    {
        selectedItem = item;
        itemNameText.text = item.itemName;
        itemImage.sprite = item.itemImage;
        itemDescriptionText.text = item.itemDescription;
        itemPopupUI.SetActive(true);
    }

    public void CloseItemPopup()
    {
        itemPopupUI.SetActive(false);
        selectedItem = null;
    }

    public void GiveGift()
    {
        if (selectedItem != null)
        {
            Debug.Log($"{selectedItem.itemName}을(를) 선물했습니다.");
            selectedItem = null; // 선택 초기화
        }
        else
        {
            Debug.Log("선물할 아이템을 선택하지 않았습니다.");
        }
        itemPopupUI.SetActive(false);
        CloseInventory();
    }
}
