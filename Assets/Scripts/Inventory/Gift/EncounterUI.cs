using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class EncounterUI : MonoBehaviour
{
    [SerializeField] private GameObject encounterPanel;
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI greetingText;

    [SerializeField] private GameObject dialogueWindow;
    //[SerializeField] private GameObject dialoguePopup;

    [SerializeField] private GiftInventory giftInventory;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private GameObject giftPopup;
    [SerializeField] private TextMeshProUGUI giftItemName;
    [SerializeField] private Image giftItemImage;
    [SerializeField] private TextMeshProUGUI giftItemDescription;

    private NoonsongEntry currentCharacter;
    private System.Action onCloseCallback;

    //기본눈송이
    [SerializeField] private ARObjectCatch arObjectCatch;
    [SerializeField] private CurrencyManager currencyManager;
    [SerializeField] private GameObject IncreasePopUp;
    [SerializeField] private GameObject noPopUp;
     private const int NOONSONG_INCREMENT = 15; 
    

    public void Show(NoonsongEntry character, System.Action onClose)
    {
        currentCharacter = character;
        onCloseCallback = onClose;

        characterImage.sprite = character.noonsongSprite;
        greetingText.text = $"{character.noonsongName}: �ȳ�! ���� {character.university} ����̾�!";

        encounterPanel.SetActive(true);
    }

    public void OpenDialogueWindow()
    {
        GameObject currentTarget = arObjectCatch.GetCurrentTarget();
        Debug.Log(currentTarget.name);
        if (currentTarget != null && currentTarget.name == "nunsong(Clone)")
        {
            IncreasePopUp.gameObject.SetActive(true);
            currencyManager.AddCurrency(NOONSONG_INCREMENT);
            Debug.Log($"기본눈송이 : {NOONSONG_INCREMENT}개의 재화 추가.");
        }
        else
        {
            dialogueWindow.SetActive(true);
        }
    }

    public void CloseDialogueWindow()
    {
        dialogueWindow.SetActive(false);
        //dialoguePopup.SetActive(false);
    }

    public void ShowDialoguePopup()
    {
        //dialoguePopup.SetActive(true);
    }

    public void CloseDialoguePopup()
    {
        //dialoguePopup.SetActive(false);
    }

    public void OpenGiftInventory()
    {
        GameObject currentTarget = arObjectCatch.GetCurrentTarget();

        if (currentTarget != null && currentTarget.name == "nunsong(Clone)")
        {
            noPopUp.gameObject.SetActive(true);
        }
        else
        {
            giftInventory.Initialize(inventoryManager, this);
            giftInventory.SyncWithInventoryManager();
            giftInventory.ToggleGiftInventory();
        }
    }

    public void ShowGiftPopup(ItemEntry item)
    {
        giftItemName.text = item.itemName;
        giftItemImage.sprite = item.itemSprite;
        giftItemDescription.text = item.description;
        giftPopup.SetActive(true);
    }

    public void CloseGiftPopup()
    {
        giftPopup.SetActive(false);
    }

    public void GiveGift(ItemEntry item)
    {
        Debug.Log($"{item.itemName}��(��) ����");
        giftInventory.SyncWithInventoryManager();
        giftPopup.SetActive(false);
    }

    public void CloseAll()
    {
        encounterPanel.SetActive(true);
    }

    public void CloseEncounter()
    {
        encounterPanel.SetActive(false);
        dialogueWindow.SetActive(false);
        onCloseCallback?.Invoke();
    }

    public string GetCurrentNoonsongUniversity()
    {
        return currentCharacter.university;
    }

    public void UpdateNoonsongAffection(int amount)
    {
        currentCharacter.loveLevel += amount;
    }
}
