using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class InventoryManager : MonoBehaviour
{
  public List<ItemEntry> itemEntries;  
  public Transform inventoryContentPanel; 
  public GameObject inventoryItemPrefab;
  public GameObject placeholderPrefab;

  private Dictionary<ItemEntry, GameObject> inventoryItems = new Dictionary<ItemEntry, GameObject>();

  public GameObject descriptionPopup;  // 설명 팝업 UI
  public TextMeshProUGUI popupItemName;
  public TextMeshProUGUI popupItemDescription;
  public Image popupItemImage;
  public Button popupCloseButton;

  void Start()
  {
    descriptionPopup.SetActive(false);
        itemEntries[1].itemCount += 10;
    }

    void PopulateInventory()
  {
    foreach (Transform child in inventoryContentPanel)
    {
      Destroy(child.gameObject);
    }
    inventoryItems.Clear();
    int totalItems = 0;
    foreach (var item in itemEntries)
    {
      if (item.itemCount > 0) // 보유 개수가 1 이상일 경우만 표시
      {
        GameObject newItem = Instantiate(inventoryItemPrefab, inventoryContentPanel);

        newItem.transform.Find("Item_Name").GetComponent<TextMeshProUGUI>().text = item.itemName;
        newItem.transform.Find("Item_Image").GetComponent<Image>().sprite = item.itemSprite;
        //newItem.transform.Find("Item_Count").GetComponent<TextMeshProUGUI>().text = $"보유 수량: {item.itemCount} 개";

        Button itemButton = newItem.GetComponent<Button>();
        if (itemButton != null)
        {
          itemButton.onClick.AddListener(() => ShowItemDescription(item));
        }

        inventoryItems[item] = newItem;
        totalItems++;
      }
    }
    if (totalItems < 3)
    {
      int placeholdersNeeded = 4 - totalItems;
      for (int i = 0; i < placeholdersNeeded; i++)
      {
        Instantiate(placeholderPrefab, inventoryContentPanel);
      }
    }
  }

    public void UpdateInventory()
    {
        var keys = new List<ItemEntry>(inventoryItems.Keys); 
        for (int i = 0; i < keys.Count; i++)
        {
            ItemEntry item = keys[i];
            if (item.itemCount <= 0)
            {
                Destroy(inventoryItems[item]); 
                inventoryItems.Remove(item); 
            }
        }

        PopulateInventory();
    }

    void ShowItemDescription(ItemEntry item)
  {
    popupItemName.text = item.itemName;
    popupItemDescription.text = item.description;
    popupItemImage.sprite = item.itemSprite;

    descriptionPopup.SetActive(true); // 팝업 활성화

    // 기존 리스너 제거 후 닫기 버튼에 기능 추가
    popupCloseButton.onClick.RemoveAllListeners();
    popupCloseButton.onClick.AddListener(() => descriptionPopup.SetActive(false));
  }

    internal void AddEmblemBadge()
    {
        if (itemEntries != null && itemEntries.Count > 1) // 리스트가 비어있지 않고, 최소 두 개 이상의 아이템이 있을 경우
        {
            itemEntries[1].itemCount = 1; 
            UpdateInventory(); // 인벤토리 UI 갱신
            Debug.Log("엠블렘 배지 획득!");
        }
        else
        {
            Debug.LogWarning("아이템 리스트가 비어있거나, 최소 두 개 이상의 아이템이 필요합니다.");
        }
    }
}
