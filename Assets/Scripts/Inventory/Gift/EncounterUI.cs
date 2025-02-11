using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class EncounterUI : MonoBehaviour
{
    [SerializeField] private GameObject encounterPanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [SerializeField] private GameObject dialogueWindow;
    [SerializeField] private GameObject exitPopup;

    [SerializeField] private GiftInventory giftInventory;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private GameObject giftPopup;
    [SerializeField] private TextMeshProUGUI giftItemName;
    [SerializeField] private Image giftItemImage;
    [SerializeField] private TextMeshProUGUI giftItemDescription;

    private NoonsongEntry currentCharacter;
    private System.Action onCloseCallback;

    private int dialogueIndex = 0;
    private Dictionary<int, List<string>> affectionDialogue = new Dictionary<int, List<string>>
    {
        { 0, new List<string> { "안녕!", "반가워!" } },
        { 5, new List<string> { "안녕!", "보고 싶었어!", "더 자주 만나면 좋겠다." } },
        { 10, new List<string> { "안녕!", "만나서 정말 좋아!", "우린 정말 좋은 친구야." } },
    };

    private Transform originalParent;

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
        dialogueIndex = 0;

        GameObject currentTarget = arObjectCatch.GetCurrentTarget();
        if (currentTarget != null)
        {
            originalParent = currentTarget.transform.parent;
            currentTarget.transform.SetParent(Camera.main.transform);
            currentTarget.transform.localPosition = Vector3.forward * 2;
            currentTarget.transform.localScale = Vector3.one * 0.7f;
        }
    

        encounterPanel.SetActive(true);
        dialogueWindow.SetActive(true);
        OpenDialogueWindow();
    }

    public void OpenDialogueWindow()
    {
        if (currentCharacter == null)
        {
            Debug.LogError("currentCharacter가 null입니다! Show()가 먼저 호출되었는지 확인하세요.");
            return;
        }

        if (affectionDialogue == null || affectionDialogue.Count == 0)
        {
            Debug.LogError("affectionDialogue 데이터가 없습니다!");
            return;
        }

        int affectionLevel = currentCharacter.loveLevel;
        int closestKey = affectionDialogue.Keys.OrderByDescending(k => k).FirstOrDefault(k => affectionLevel >= k);

        if (!affectionDialogue.ContainsKey(closestKey) || affectionDialogue[closestKey].Count == 0)
        {
            Debug.LogWarning("호감도 대사가 없음!");
            dialogueText.text = "……";
            return;
        }

        List<string> dialogues = affectionDialogue[closestKey];
        if (dialogueIndex >= dialogues.Count)
        {
            dialogueIndex = 0;
        }

        dialogueText.text = dialogues[dialogueIndex];
        Debug.Log($"대화 출력: {dialogues[dialogueIndex]} (Index: {dialogueIndex})");

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

    public void OnDialogueButtonClicked()
    {
        dialogueIndex++;
        OpenDialogueWindow();
    }

    public void CloseDialogueWindow()
    {
        dialogueWindow.SetActive(false);
        //dialoguePopup.SetActive(false);
    }

    public void ShowExitConfirmation()
    {
        exitPopup.SetActive(true);
    }

    public void ConfirmExit()
    {
        CloseEncounter();
        exitPopup.SetActive(false);
    }

    public void CancelExit()
    {
        exitPopup.SetActive(false);
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

    public void ShowGiftDialogue(string message)
    {
        dialogueText.text = message;
        Debug.Log($"선물 대화 출력: {message}");
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

    public void CloseEncounter()
    {
        GameObject currentTarget = arObjectCatch.GetCurrentTarget();
        if (currentTarget != null)
        {
            currentTarget.transform.SetParent(null); // 원래 부모로 복구
        }

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
