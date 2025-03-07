using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class InventoryManager : MonoBehaviour
{
  public Transform inventoryContentPanel; 
  public GameObject inventoryItemPrefab;
  public GameObject placeholderPrefab;

  private Dictionary<int, GameObject> inventoryItems = new Dictionary<int, GameObject>();

  public GameObject descriptionPopup;  // 설명 팝업 UI
  public TextMeshProUGUI popupItemName;
  public TextMeshProUGUI popupItemDescription;
  public Image popupItemImage;
  public Button popupCloseButton;

  void Start()
  {
    descriptionPopup.SetActive(false);
    FindUserItemAndIncreaseCount(2, 10);
    PopulateInventory();
  }

  private void FindUserItemAndIncreaseCount(int itemId, int amount)
  {
    UserInventoryEntry userItem = UserInventoryManager.Instance.userInventoryEntries.Find(e => e.itemId == itemId);

    if (userItem != null)
    {
      userItem.itemCount += amount;
      UserInventoryManager.Instance.UpdateItemCount(userItem.userId, userItem.itemId, userItem.itemCount);
    }
  }


  void PopulateInventory()
  {
    foreach (Transform child in inventoryContentPanel)
    {
      Destroy(child.gameObject);
    }
    inventoryItems.Clear();
    int totalItems = 0;
    foreach (var item in UserInventoryManager.Instance.userInventoryEntries)
    {
      if (item.itemCount > 0) // 보유 개수가 1 이상일 경우만 표시
      {
        ItemEntry itemEntry = item.GetItemEntry();
        if (itemEntry == null) continue;

        GameObject newItem = Instantiate(inventoryItemPrefab, inventoryContentPanel);

        newItem.transform.Find("Item_Name").GetComponent<TextMeshProUGUI>().text = itemEntry.itemName;
        newItem.transform.Find("Item_Image").GetComponent<Image>().sprite = itemEntry.itemSprite;

        //newItem.transform.Find("Item_Count").GetComponent<TextMeshProUGUI>().text = $"보유 수량: {item.itemCount} 개";

        Button itemButton = newItem.GetComponent<Button>();
        if (itemButton != null)
        {
          itemButton.onClick.AddListener(() => ShowItemDescription(itemEntry));
        }

        inventoryItems[item.itemId] = newItem;
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
        var keys = new List<int>(inventoryItems.Keys);
    foreach (var itemId in keys)
    {
      UserInventoryEntry userItem = UserInventoryManager.Instance.userInventoryEntries.Find(e => e.itemId == itemId);
      if (userItem == null || userItem.itemCount <= 0)
      {
        if (inventoryItems.ContainsKey(itemId)) 
        {
          Destroy(inventoryItems[itemId]);
          inventoryItems.Remove(itemId);
        }
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
    if (UserInventoryManager.Instance.userInventoryEntries.Count > 1)
    {
      // 리스트가 비어있지 않고, 최소 두 개 이상의 아이템이 있을 경우
      UserInventoryEntry userItem = UserInventoryManager.Instance.userInventoryEntries.Find(e => e.itemId == 2);

      if (userItem != null)
      {
        userItem.itemCount = 1;
        UserInventoryManager.Instance.UpdateItemCount(userItem.userId, userItem.itemId, userItem.itemCount);
        UpdateInventory(); // 인벤토리 UI 갱신
        Debug.Log("엠블렘 배지 획득!");
      }
      else
      {
        Debug.LogWarning("엠블렘 배지를 찾을 수 없습니다.");
      }
    }
    else
    {
      Debug.LogWarning("인벤토리에 최소 두 개 이상의 아이템이 필요함.");
    }
  }
}
