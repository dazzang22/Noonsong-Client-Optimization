using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using BackEnd;
using UnityEngine.XR.ARFoundation;

public class EncounterUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject encounterPanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI noonsongNameText;

    [SerializeField] private GameObject dialogueWindow;
    [SerializeField] private GameObject exitPopup;

    [Header("Gift")]
    [SerializeField] private GiftInventory giftInventory;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private NoonsongCountUI noonsongCountUI;
    [SerializeField] private GameObject giftPopup;
    [SerializeField] private TextMeshProUGUI giftItemName;
    [SerializeField] private Image giftItemImage;
    [SerializeField] private TextMeshProUGUI giftItemDescription;

    [Header("friend")]
    [SerializeField] private GameObject friendRequestPopup;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private NoonsongEntry currentCharacter;
    private System.Action onCloseCallback;
    [Header("effect")]
    public List<GameObject> EffectPrefabs;
    public GameObject collectEffectPrefab;
    private Param param2 = new Param();


    private bool isDialogueActive = false;
    [Header("button")]
    [SerializeField] private Button dialogueButton;
    [SerializeField] private Button HiButton;
    [SerializeField] private Button GiftButton;

    private Dictionary<int, List<string>> affectionDialogue = new Dictionary<int, List<string>>
    {
        { 0, new List<string> { "안녕! 반가워!" } },
        { 5, new List<string> { "안녕! 보고 싶었어! 더 자주 만나면 좋겠다." } },
        { 10, new List<string> { "안녕! 만나서 정말 좋아! 우린 정말 좋은 친구야." } },
    };

    private List<string> commonDialogues = new List<string>
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

    private Dictionary<string, List<string>> departmentDialogues = new Dictionary<string, List<string>>
    {
        { "LiberalArts", new List<string> { "다음 만남도 하나의 멋진 이야기가 되겠지? 기대할게!", "짧은 이별은 더 깊은 만남을 위한 예고편이지~ 다음에 더 깊은 대화 나누자." , "나와의 시간에 긴 여운이 남길 바라~" } },
        { "Science", new List<string> { "다음에 또 보자. 내 계산에 따르면 우리가 또 만날 확률은... 100%거든!", "너와의 대화는 내게 언제나 유의미한 데이터야~ 다음에도 잘 부탁해." } },
        { "Science(Physical)", new List<string> { "잠깐 숨 고르고, 다음에도 최고의 컨디션으로 만나자~"} },
        { "Engineering", new List<string> { "내 알고리즘에 너를 넣어뒀으니, 우린 다시 만나게 될 거야!", "오늘, 거의 무한동력 장치 만든 기분이었어! 다음에 또 보자~", "너와의 시간이 새로운 프로젝트의 실마리가 될지도 모르겠어. 좋은 영감을 줘서 고마워!" } },
        { "HumanEcology", new List<string> { "일상 속에서 늘 행복하길 바랄게. 다음에 또 만나자~", "좋은 한 끼가 몸을 살리듯, 오늘의 대화도 내 마음을 살린 것 같아. 고마워~" } },
        { "SocialSciences", new List<string> { "앞으로도 각자의 위치에서 더 나은 사회를 만들어보자!", "네 덕에 세상을 보는 시야가 넓어진 것 같아. 유익하고 즐거운 시간이었어!" } },
        { "Law", new List<string> { "공정한 하루 보내~!" } },
        { "Business", new List<string> { "오늘 만남이 창출해낸 가치를 잊지 못할 거야. 다음에도 잘 부탁해!" } },
        { "Music", new List<string> { "너랑 나의 멜로디는 늘 조화로운 것 같아! 오늘도 정말 좋았어~", "너의 하루가 아름다운 멜로디로 가득하길 바랄게~" } },
        { "Pharmacy", new List<string> { "다음에 보자. 그때까지 아프지 말고, 영양분도 잘 챙겨!" } },
        { "Art", new List<string> { "너의 하루가 아름다운 색들로 가득하길 바랄게~", "미적으로 완벽한 시간이었어. 다음에 보자!" } },
        { "GlobalService", new List<string> { "너와 나의 연결도 하나의 글로벌한 연결이겠지. 앞으로도 잘 지내보자~" } },
        { "GlobalConvergence", new List<string> { "다음에는 더 다양한 문화에 대해 이야기하자!" } },
        { "English", new List<string> { "다시 만나자! somewhere, sometime~" } },
        { "Media", new List<string> { "다음에도 흥미로운 소식 있으면 꼭 전해줘, 알았지?"} },
    };

    private Transform originalParent;

    [Header("OriginalNoonsong")]
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

        //if (currentTarget != null)
        //{
        //    Debug.Log($"현재 오브젝트 상태: {currentTarget.activeSelf}, 위치: {currentTarget.transform.position}");
        //}
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
            if (currentTarget != null && currentTarget.transform.parent != Camera.main.transform)
            {
                currentTarget.transform.SetParent(Camera.main.transform);
            }

            ARAnchorManager anchorManager = FindObjectOfType<ARAnchorManager>();
            if (anchorManager != null)
            {
                ARAnchor anchor = currentTarget.AddComponent<ARAnchor>();
            }

            Vector3 fixedPosition = new Vector3(0, 0, 3);
            currentTarget.transform.localPosition = fixedPosition;
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

    private void LateUpdate()
    {
        GameObject currentTarget = arObjectCatch.GetCurrentTarget();

        if (currentTarget != null)
        {
            if (currentTarget.transform.parent != Camera.main.transform)
            {
                currentTarget.transform.SetParent(Camera.main.transform);
            }

            currentTarget.transform.localPosition = new Vector3(0, 0, 3);
            currentTarget.transform.localRotation = Quaternion.Euler(0, 180, 0);
            currentTarget.transform.localScale = Vector3.one * 1f;
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

        List<string> combinedDialogues = new List<string>(commonDialogues);

        if (currentCharacter != null)
        {
            string dept = GetCurrentNoonsongUniversity(); // 기존에 만들어둔 함수
            if (departmentDialogues.ContainsKey(dept))
            {
                combinedDialogues.AddRange(departmentDialogues[dept]);
            }
        }

        if (combinedDialogues.Count > 0)
        {
            int randomIndex = Random.Range(0, combinedDialogues.Count);
            dialogueText.text = combinedDialogues[randomIndex];
        }
        else
        {
            dialogueText.text = "안녕! 다음에 또 봐~";
        }

        GameObject currentTarget = arObjectCatch.GetCurrentTarget();
        if (currentTarget != null)
        {
            Debug.Log(currentTarget.name);
            if (currentTarget.name == "noonsong remake 0202(Clone)")
            {
                IncreasePopUp.gameObject.SetActive(true);
                dialogueButton.interactable = true;
                currencyManager.AddCurrency(NOONSONG_INCREMENT);
                Debug.Log($"기본눈송이 : {NOONSONG_INCREMENT}개의 재화 추가.");

                //db 반영
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
        giftPopup.SetActive(false);
        giftInventory.SyncWithInventoryManager();
        dialogueButton.interactable = true;
        HiButton.interactable = false;
        GiftButton.interactable = false;
        //DB 연결
        GiftManager.Instance.sendGiftItem(item.itemID);
    }

    public void CloseEncounter()
    {
        HiButton.interactable = true;
        GiftButton.interactable = true;
        dialogueButton.interactable = false;

        encounterPanel.SetActive(false);
        dialogueWindow.SetActive(false);
        collectEffectPrefab.SetActive(false);
        foreach (GameObject obj in EffectPrefabs)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
        onCloseCallback?.Invoke();
    }

    public string GetCurrentNoonsongUniversity()
    {
        if (currentCharacter.name == "체육교육과" || currentCharacter.name == "무용과")
        {
            string physicalString = "Science(Physical)";
            return physicalString;
        }
        return currentCharacter.university;
    }

    public int UpdateNoonsongAffection(int amount)
    {
        currentCharacter.loveLevel += amount;
        //db 업데이트
        UserDogamManager.Instance.noonsongInsert(currentCharacter.noonsongName,amount,currentCharacter.university);
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
