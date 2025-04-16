using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using BackEnd;
public class TuryItemManager : MonoBehaviour
{
  public InventoryManager inventoryManager;

  public List<ItemEntry> itemEntries;  // 아이템 리스트 (스크립터블 오브젝트)
  public Transform contentPanel;       // 스크롤뷰의 Content 오브젝트
  public GameObject itemPrefab;        // 아이템 UI 프리팹
  public GameObject placeholderPrefab;
  public TextMeshProUGUI currencyText; // 현재 보유 재화 표시 UI

  public GameObject purchasePopup;     // 메인 팝업 판넬 (전체 UI 판넬)
  public GameObject buyPopup;          // 구매 확인 팝업 
  public GameObject cantBuyPopup;      // 재화 부족 팝업 

  public TextMeshProUGUI popupItemName; // 구매 팝업 아이템 이름
  //public TextMeshProUGUI popupItemPrice; // 구매 팝업 아이템 가격 표시
  public TextMeshProUGUI popupItemDescription; // 구매 팝업 아이템 설명 표시
  public Image popupItemImage;         // 구매 팝업 아이템 이미지 표시

  public TextMeshProUGUI cantBuyPopupItemName;
  //public TextMeshProUGUI cantBuyPopupItemPrice;
  public TextMeshProUGUI cantBuyPopupItemDescription;
  public Image cantBuyPopupItemImage;

  public Button confirmBuyButton;       // 구매 팝업 내 "구매" 버튼 
  public Button cancelBuyButton;        // 구매 팝업 내 "취소" 버튼 
  public Button cantBuyConfirmButton;   // 재화 부족 팝업 "확인" 버튼 

  private ItemEntry selectedItem; // 현재 선택된 아이템 저장

  void Start()
  {
    purchasePopup.SetActive(false);
    buyPopup.SetActive(false);
    cantBuyPopup.SetActive(false);

    UpdateCurrencyUI();
    PopulateShop();
  }

  void PopulateShop()
  {
    int totalItems = itemEntries.Count;

    foreach (var item in itemEntries)
    {
      GameObject newItem = Instantiate(itemPrefab, contentPanel);

      newItem.transform.Find("Top/Item_Name").GetComponent<TextMeshProUGUI>().text = item.itemName;
      newItem.transform.Find("Top/Item_Image").GetComponent<Image>().sprite = item.itemSprite;
      newItem.transform.Find("Bottom/Price").GetComponent<TextMeshProUGUI>().text = $"{item.itemPrice}";


      Button itemButton = newItem.GetComponent<Button>();
      if (itemButton != null)
      {
        itemButton.onClick.AddListener(() => OnItemClick(item));
      }
      else
      {
        Debug.LogWarning($"'{item.itemName}' 아이템에 버튼이 없습니다.");
      }
    }

    if (totalItems < 4)
    {
      int placeholdersNeeded = 4 - totalItems;
      for (int i = 0; i < placeholdersNeeded; i++)
      {
        Instantiate(placeholderPrefab, contentPanel);
      }
    }

  }

  void OnItemClick(ItemEntry item)
  {
    selectedItem = item; // 선택된 아이템 저장
    purchasePopup.SetActive(true); // 메인 판넬 활성화

    if (CurrencyManager.Instance.GetCurrencyAmount() >= selectedItem.itemPrice)
    {
      ShowBuyPopup();
    }
    else
    {
      ShowCantBuyPopup();
    }
  }

  void ShowBuyPopup()
  {
    popupItemName.text = selectedItem.itemName;
    //popupItemPrice.text = $"{selectedItem.itemPrice} Gold";
    popupItemDescription.text = selectedItem.description;
    popupItemImage.sprite = selectedItem.itemSprite;

    buyPopup.SetActive(true);

    // 기존 리스너 제거 후 새 이벤트 추가 (중복 방지)
    confirmBuyButton.onClick.RemoveAllListeners();
    confirmBuyButton.onClick.AddListener(() => ConfirmPurchase());

    cancelBuyButton.onClick.RemoveAllListeners();
    cancelBuyButton.onClick.AddListener(() => CloseAllPopups());
  }

  void ShowCantBuyPopup()
  {
    cantBuyPopupItemName.text = selectedItem.itemName;
    //cantBuyPopupItemPrice.text = $"{selectedItem.itemPrice} Gold";
    cantBuyPopupItemDescription.text = selectedItem.description;
    cantBuyPopupItemImage.sprite = selectedItem.itemSprite;

    cantBuyPopup.SetActive(true);

    // 기존 리스너 제거 후 새 이벤트 추가 (중복 방지)
    cantBuyConfirmButton.onClick.RemoveAllListeners();
    cantBuyConfirmButton.onClick.AddListener(() => CloseAllPopups());
  }

  void ConfirmPurchase()
  {
    if (selectedItem != null && CurrencyManager.Instance.UseCurrency(selectedItem.itemPrice))
    {
      string userId = UserDataManager.Instance.getUserID();
      //재화 소비 DB 반영
      Param pay = new Param();
      pay= UserBalanceManager.Instance.paysnow(selectedItem.itemPrice);
      UserBalanceManager.Instance.updateBalance(pay);

      //인벤토리 DB 반영
      UserInventoryEntry existingItem = UserInventoryManager.Instance.userInventoryEntries
           .Find(e => e.itemId == selectedItem.itemID);
      if (existingItem != null)
      {
        //기존 아이템이 존재하면 개수 업데이트
        int newCount = existingItem.itemCount + 1;
        UserInventoryManager.Instance.UpdateItemCount(userId, selectedItem.itemID, newCount);
        selectedItem.itemCount = newCount; 
      }
      else
      {
        //기존에 없던 아이템이라면 새롭게 추가
        UserInventoryManager.Instance.InsertUserInventory(userId, selectedItem.itemID, 1);
        selectedItem.itemCount = 1;
      }

      Debug.Log($"{selectedItem.itemName}을(를) 구매했습니다. 현재 보유량: {selectedItem.itemCount}");


      UserInventoryManager.Instance.ReloadInventory();  //인벤토리 업데이트
    }

    CloseAllPopups();
  }


  void CloseAllPopups()
  {
    buyPopup.SetActive(false);
    cantBuyPopup.SetActive(false);
    purchasePopup.SetActive(false);
    UpdateCurrencyUI();
  }

  void UpdateCurrencyUI()
  {
    currencyText.text = $"{CurrencyManager.Instance.GetCurrencyAmount()}";
  }
}