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
    [SerializeField] private TextMeshProUGUI noonsongNameText;

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
    private bool isDialogueActive = false;
    [SerializeField] private Button dialogueButton;
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

    private void Start()
    {
        dialogueButton.interactable = false;
    }

    public void Show(NoonsongEntry character, System.Action onClose)
    {
        if (character == null)
        {
            Debug.LogError("character가 null입니다!");
            return;
        }

        currentCharacter = character;
        Debug.Log($"currentCharacter 설정됨: {currentCharacter.name}");
        onCloseCallback = onClose;
        dialogueIndex = 0;
        isDialogueActive = false;
        dialogueButton.interactable = false;
        noonsongNameText.text = currentCharacter.noonsongName;

        GameObject currentTarget = arObjectCatch.GetCurrentTarget();
        if (currentTarget != null)
        {
            originalParent = currentTarget.transform.parent;
            currentTarget.transform.SetParent(Camera.main.transform);
            currentTarget.transform.localPosition = new Vector3(0, 0, 3);
            currentTarget.transform.localScale = Vector3.one * 1f;
            currentTarget.transform.localRotation = Quaternion.identity;
        }
    

        encounterPanel.SetActive(true);
        dialogueWindow.SetActive(true);
        dialogueText.text = "안녕! 반가워~"; // 초기 인사말, 추후 수정 예정
    }

    public void ShowDefaultDialogue(GameObject noonsongPrefeb, System.Action onClose)
    {
        currentCharacter = null;
        onCloseCallback = onClose;
        dialogueIndex = 0;

        GameObject instance = Instantiate(noonsongPrefeb, Camera.main.transform);
        instance.transform.localPosition = new Vector3(0, 0, 3);
        instance.transform.localScale = Vector3.one * 1f;
        instance.transform.localRotation = Quaternion.Euler(0, 180, 0);

        noonsongNameText.text = "눈송이";

        encounterPanel.SetActive(true);
        dialogueWindow.SetActive(true);
        dialogueText.text = "안녕! 반가워~";

        onCloseCallback = () =>
        {
            Destroy(instance);
            onClose?.Invoke();
        };
    }

    public void OnDialogueButtonClicked()
    {
        if (!isDialogueActive)
        {
            isDialogueActive = true;
            dialogueButton.interactable = true;
            dialogueIndex = 0;
        }
        ShowNextDialogue();
    }

    public void ShowNextDialogue()
    {
        if (affectionDialogue == null || affectionDialogue.Count == 0)
        {
            Debug.LogError("affectionDialogue 데이터가 없습니다!");
            return;
        }

        if (!isDialogueActive) return;

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
            Debug.Log("대화 종료!");
            isDialogueActive = false;
            return;
        }

        dialogueText.text = dialogues[dialogueIndex];
        Debug.Log($"🗨️ 대화 출력: {dialogueText.text} (Index: {dialogueIndex})");
        dialogueIndex++;

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
        isDialogueActive = false;
    }

    public void ShowExitConfirmation()
    {
        exitPopup.SetActive(true);
        dialogueButton.interactable = false;
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
        dialogueButton.interactable = false;

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
