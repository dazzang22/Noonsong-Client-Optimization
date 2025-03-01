using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using BackEnd;
public class EncounterUI : MonoBehaviour
{
    [SerializeField] private GameObject encounterPanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI noonsongNameText;

    [SerializeField] private GameObject dialogueWindow;
    [SerializeField] private GameObject exitPopup;

    [SerializeField] private GiftInventory giftInventory;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private NoonsongCountUI noonsongCountUI;
    [SerializeField] private GameObject giftPopup;
    [SerializeField] private TextMeshProUGUI giftItemName;
    [SerializeField] private Image giftItemImage;
    [SerializeField] private TextMeshProUGUI giftItemDescription;

    [SerializeField] private GameObject friendRequestPopup;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private NoonsongEntry currentCharacter;
    private System.Action onCloseCallback;

    public GameObject EffectPrefabs;
    public GameObject collectEffectPrefab;

    private bool isDialogueActive = false;
    [SerializeField] private Button dialogueButton;

    [SerializeField] private Button HiButton;
    [SerializeField] private Button GiftButton;

    private Dictionary<int, List<string>> affectionDialogue = new Dictionary<int, List<string>>
    {
        { 0, new List<string> { "안녕! 반가워!" } },
        { 5, new List<string> { "안녕! 보고 싶었어! 더 자주 만나면 좋겠다." } },
        { 10, new List<string> { "안녕! 만나서 정말 좋아! 우린 정말 좋은 친구야." } },
    };

    private List<string> randomDialogue = new List<string>
{
    "다음에 또 보자!",
    "오늘도 좋은 하루 보내~",
    "대화 재밌었어! 다음에 만나~",
    "조심히 가, 안녕!",
    "다음에 만나면 또 놀자~",
    "또 만나러 와 줄거지? 잘 가!",
    "나 잊으면 안 돼~ 다음에 또 얘기하자!",
    "안녕, 나중에 봐~",
    "앗, 벌써 시간이... 아쉽지만 다음에 또 놀자!",
    "너랑 노는 거 정말 재밌었어! 안녕~"
};

    private Transform originalParent;

    //기본눈송이
    [SerializeField] private ARObjectCatch arObjectCatch;
    [SerializeField] private CurrencyManager currencyManager;
    [SerializeField] private GameObject IncreasePopUp;
    [SerializeField] private GameObject noPopUp;
     private const int NOONSONG_INCREMENT = 15;

    private void Update()
    {
        GameObject currentTarget = arObjectCatch.GetCurrentTarget();

        if (encounterPanel.activeSelf && currentCharacter == null)
        {
            if (currentTarget == null || currentTarget.name != "noonsong remake 0202(Clone)")
            {
                Debug.LogWarning("currentCharacter가 null이고, 기본 눈송이가 아니므로 UI를 비활성화합니다.");
                CloseEncounter();
            }
        }
    }

    public void Show(NoonsongEntry character, System.Action onClose)
    {
        HiButton.interactable = true;
        GiftButton.interactable = true;
        dialogueButton.interactable = false;

        if (character == null)
        {
            Debug.LogError("character가 null입니다!");
            return;
        }

        EffectPrefabs.SetActive(true);

        currentCharacter = character;
        Debug.Log($"currentCharacter 설정됨: {currentCharacter.name}");
        onCloseCallback = onClose;
        isDialogueActive = false;
        dialogueButton.interactable = false;
        if (currentCharacter.loveLevel >= 2)
        {
            noonsongNameText.text = currentCharacter.noonsongName;

        }
        else
        {
            noonsongNameText.text = "???";
        }

        GameObject currentTarget = arObjectCatch.GetCurrentTarget();
        if (currentTarget != null)
        {
            originalParent = currentTarget.transform.parent;
            currentTarget.transform.SetParent(Camera.main.transform);
            currentTarget.transform.localPosition = new Vector3(0, 0, 3);
            currentTarget.transform.localScale = Vector3.one * 1f;
            currentTarget.transform.localRotation = Quaternion.Euler(0, 180, 0);


            NPCPatrol npcPatrol = currentTarget.GetComponent<NPCPatrol>();
            if (npcPatrol != null)
            {
                Debug.Log("이동중지");
                npcPatrol.StopAllCoroutines(); //  이동 중지
                npcPatrol.SetWalking(false);
            }
            else
            {
                Debug.Log("npuPatrol null");
            }
        }

        encounterPanel.SetActive(true);
        dialogueWindow.SetActive(true);

        int affectionLevel = currentCharacter.loveLevel;
        int closestKey = affectionDialogue.Keys.OrderByDescending(k => k).FirstOrDefault(k => affectionLevel >= k);
        if (affectionDialogue.ContainsKey(closestKey) && affectionDialogue[closestKey].Count > 0)
        {
            dialogueText.text = affectionDialogue[closestKey][0];
        }
        else
        {
            dialogueText.text = "안녕! 반가워~";
        }
    }

    public void ShowDefaultDialogue(GameObject noonsongPrefeb, System.Action onClose)
    {
        HiButton.interactable = true;
        GiftButton.interactable = true;
        dialogueButton.interactable = false;

        currentCharacter = null;
        onCloseCallback = onClose;

        GameObject currentTarget = arObjectCatch.GetCurrentTarget();
        if (currentTarget != null)
        {
            originalParent = currentTarget.transform.parent;
            currentTarget.transform.SetParent(Camera.main.transform);
            currentTarget.transform.localPosition = new Vector3(0, 0, 3);
            currentTarget.transform.localScale = Vector3.one * 1f;
            // currentTarget.transform.localRotation = Quaternion.identity;
            currentTarget.transform.localRotation = Quaternion.Euler(0, 180, 0);
            
            NPCPatrol npcPatrol = currentTarget.GetComponent<NPCPatrol>();
            if (npcPatrol != null)
            {
                Debug.Log("이동중지");
                npcPatrol.StopAllCoroutines(); //  이동 중지
                npcPatrol.SetWalking(false);
            }
            else
            {
                Debug.Log("npuPatrol null");
            }

        }

        noonsongNameText.text = "눈송이";

        encounterPanel.SetActive(true);
        dialogueWindow.SetActive(true);
        dialogueText.text = "안녕! 반가워~";
    }

    public void OnDialogueButtonClicked()
    {
        if (!isDialogueActive)
        {
            isDialogueActive = true;
            dialogueButton.interactable = true;
            HiButton.interactable = false;
            GiftButton.interactable = false;
        }
        ShowNextDialogue();
    }

    public void ShowNextDialogue()
    {
        if (!isDialogueActive) return;

        if (affectionDialogue == null || affectionDialogue.Count == 0)
        {
            Debug.LogWarning("affectionDialogue 데이터가 없지만, 기본 처리 진행.");
        }
        else
        {
            int randomIndex = Random.Range(0, randomDialogue.Count);
            dialogueText.text = randomDialogue[randomIndex];
        }

        GameObject currentTarget = arObjectCatch.GetCurrentTarget();
        if (currentTarget != null)
        {
            Debug.Log(currentTarget.name);
            if (currentTarget.name == "noonsong remake 0202(Clone)")
            {
                IncreasePopUp.gameObject.SetActive(true);
                currencyManager.AddCurrency(NOONSONG_INCREMENT);
                Debug.Log($"기본눈송이 : {NOONSONG_INCREMENT}개의 재화 추가.");

                //db 반영
                Param param2 = new Param();
                param2= UserBalanceManager.Instance.addsnow(NOONSONG_INCREMENT);
                UserBalanceManager.Instance.updateBalance(param2);

                Destroy(currentTarget);
                CloseEncounter();

            }
            else
            {
                dialogueWindow.SetActive(true);
            }
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
        GameObject currentTarget = arObjectCatch.GetCurrentTarget();

        if (currentTarget != null)
        {
            Destroy(currentTarget);
            Debug.Log($"Destroyed: {currentTarget.name}");
        }
        else
        {
            Debug.LogWarning("currentTarget is null, cannot destroy.");
        }

        CloseEncounter();
        exitPopup.SetActive(false);
    }

    public void CancelExit()
    {
        exitPopup.SetActive(false);
        dialogueButton.interactable = true;
    }


    public void OpenGiftInventory()
    {
        dialogueButton.interactable = false;

        GameObject currentTarget = arObjectCatch.GetCurrentTarget();

        if (currentTarget != null && currentTarget.name == "noonsong remake 0202(Clone)")
        {
            noPopUp.gameObject.SetActive(true);
        }
        else
        {
            giftInventory.Initialize(inventoryManager, this, arObjectCatch);
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
        dialogueButton.interactable = true;
        HiButton.interactable = false;
        GiftButton.interactable = false;
    }

    public void CloseEncounter()
    {
        HiButton.interactable = true;
        GiftButton.interactable = true;
        dialogueButton.interactable = false;

        encounterPanel.SetActive(false);
        dialogueWindow.SetActive(false);
        EffectPrefabs.SetActive(false);
        onCloseCallback?.Invoke();
    }

    public string GetCurrentNoonsongUniversity()
    {
        return currentCharacter.university;
    }

    public int UpdateNoonsongAffection(int amount)
    {
        currentCharacter.loveLevel += amount;
        return currentCharacter.loveLevel;
    }

    public void ShowFriendRequestPopup()
    {
        friendRequestPopup.SetActive(true);

        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() => BecomeFriends());

        noButton.onClick.RemoveAllListeners();
        noButton.onClick.AddListener(() => friendRequestPopup.SetActive(false));
    }

    public void BecomeFriends()
    {
        currentCharacter.isFriend = true;
        friendRequestPopup.SetActive(false);
        noonsongCountUI.UpdateFriendCount();
        if (collectEffectPrefab != null)
        {
            collectEffectPrefab.SetActive(true);
            StartCoroutine(DisableEffectAfterDelay(3f, collectEffectPrefab));
        }
    }

    public NoonsongEntry GetCurrentNoonsongEntry()
    {
        return currentCharacter;
    }

    private IEnumerator DisableEffectAfterDelay(float delay, GameObject effect)
    {
        yield return new WaitForSeconds(delay);

        if (effect != null)
        {
            effect.SetActive(false);
        }
    }
}
