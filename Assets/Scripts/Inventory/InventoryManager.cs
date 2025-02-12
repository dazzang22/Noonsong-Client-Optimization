using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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
        //itemEntries[1].itemCount += 10;
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
    foreach (var item in inventoryItems.Keys)
    {
      if (item.itemCount > 0)
      {
        //inventoryItems[item].transform.Find("Item_Count").GetComponent<TextMeshProUGUI>().text = $"보유 수량: {item.itemCount} 개";
      }
      else
      {
        Destroy(inventoryItems[item]); // 보유 개수가 0이면 제거
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
}
