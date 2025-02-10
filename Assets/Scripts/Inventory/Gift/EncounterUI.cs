using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EncounterUI : MonoBehaviour
{
    [SerializeField] private GameObject encounterPanel;
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI greetingText;

    [SerializeField] private GameObject dialogueWindow;
    [SerializeField] private GameObject dialoguePopup;

    [SerializeField] private GiftInventory giftInventory;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private GameObject giftPopup;
    [SerializeField] private TextMeshProUGUI giftItemName;
    [SerializeField] private Image giftItemImage;
    [SerializeField] private TextMeshProUGUI giftItemDescription;

    private NoonsongEntry currentCharacter;
    private System.Action onCloseCallback;

    

    public void Show(NoonsongEntry character, System.Action onClose)
    {
        currentCharacter = character;
        onCloseCallback = onClose;

        characterImage.sprite = character.noonsongSprite;
        greetingText.text = $"{character.noonsongName}: 안녕! 나는 {character.university} 출신이야!";

        encounterPanel.SetActive(true);
    }

    public void OpenDialogueWindow()
    {
        dialogueWindow.SetActive(true);
    }

    public void CloseDialogueWindow()
    {
        dialogueWindow.SetActive(false);
        dialoguePopup.SetActive(false);
    }

    public void ShowDialoguePopup()
    {
        dialoguePopup.SetActive(true);
    }

    public void CloseDialoguePopup()
    {
        dialoguePopup.SetActive(false);
    }

    public void OpenGiftInventory()
    {
        giftInventory.Initialize(inventoryManager, this);
        giftInventory.SyncWithInventoryManager();
        giftInventory.ToggleGiftInventory();
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
        Debug.Log($"{item.itemName}을(를) 선물");
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
}
