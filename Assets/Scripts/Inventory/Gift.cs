using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gift : MonoBehaviour
{
    public GameObject greetingUI; // 인사말 UI
    public Text greetingText; // 인사말 텍스트
    public Button talkButton; // 대화하기 버튼
    public GameObject talkUI; // 대화하기 화면
    public GameObject interactionButtonsPopup; // 상호작용 버튼 팝업
    public Button giftButton; // 선물하기 버튼
    public Button greetButton; // 인사하기 버튼
    public Button endTalkButton; // 대화 종료 버튼
    public GameObject inventoryUI; // 인벤토리 UI
    public Button selectCompleteButton; // 아이템 선택 완료 버튼
    public Button closeInventoryButton; // 인벤토리 닫기 버튼
    public GameObject endTalkPopup; // 대화 종료 확인 팝업
    public Button confirmEndTalkButton; // 대화 종료 확인 버튼
    public Button cancelEndTalkButton; // 대화 종료 취소 버튼

    private Queue<string> dialogues = new Queue<string>(); // 대화 내용을 담는 큐
    private Transform characterTransform; // 캐릭터의 Transform

    // 캐릭터별 대화 데이터
    private Dictionary<string, List<string>> characterDialogues = new Dictionary<string, List<string>>
    {
        { "캐릭터1", new List<string> { "안녕하세요!", "오늘도 좋은 하루 되세요!" } },
        { "캐릭터2", new List<string> { "반갑습니다!", "무엇을 도와드릴까요?" } },
        { "캐릭터3", new List<string> { "좋은 날이에요!", "함께 놀아요!" } }
    };

    private string currentCharacterName;

    void Start()
    {
        talkButton.onClick.AddListener(EnterTalkScreen);
        endTalkButton.onClick.AddListener(ShowEndTalkPopup);
        confirmEndTalkButton.onClick.AddListener(EndConversation);
        cancelEndTalkButton.onClick.AddListener(CloseEndTalkPopup);
        giftButton.onClick.AddListener(OpenInventory);
        closeInventoryButton.onClick.AddListener(CloseInventory);
        selectCompleteButton.onClick.AddListener(GiveGift);

        InitializeGreeting();
    }

    public void SetCharacter(string characterName, Transform character)
    {
        currentCharacterName = characterName;
        characterTransform = character;
        InitializeGreeting();
    }

    private void InitializeGreeting()
    {
        greetingUI.SetActive(true);
        if (characterDialogues.ContainsKey(currentCharacterName))
        {
            greetingText.text = characterDialogues[currentCharacterName][0];
        }
        else
        {
            greetingText.text = "안녕하세요!";
        }
        talkUI.SetActive(false);
        interactionButtonsPopup.SetActive(false);
        inventoryUI.SetActive(false);
        endTalkPopup.SetActive(false);
    }

    public void EnterTalkScreen()
    {
        greetingUI.SetActive(false);
        talkUI.SetActive(true);
        CenterCharacter();
        PrepareDialogues();
        ShowNextDialogue();
    }

    private void CenterCharacter()
    {
        if (characterTransform != null)
        {
            characterTransform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.0f; // 예: 화면 앞쪽 2미터
        }
    }

    private void PrepareDialogues()
    {
        dialogues.Clear();
        if (characterDialogues.ContainsKey(currentCharacterName))
        {
            foreach (string dialogue in characterDialogues[currentCharacterName])
            {
                dialogues.Enqueue(dialogue);
            }
        }
    }

    private void ShowNextDialogue()
    {
        if (dialogues.Count > 0)
        {
            string nextDialogue = dialogues.Dequeue();
            // 대화창 UI에 텍스트 표시
            Debug.Log(nextDialogue);
        }
        else
        {
            Debug.Log("대화가 끝났습니다.");
        }
    }

    public void ShowEndTalkPopup()
    {
        endTalkPopup.SetActive(true);
    }

    public void CloseEndTalkPopup()
    {
        endTalkPopup.SetActive(false);
    }

    public void EndConversation()
    {
        endTalkPopup.SetActive(false);
        greetingUI.SetActive(false);
        talkUI.SetActive(false);
        interactionButtonsPopup.SetActive(false);
        inventoryUI.SetActive(false);
        Debug.Log("대화 종료");
    }

    public void OpenInventory()
    {
        inventoryUI.SetActive(true);
    }

    public void CloseInventory()
    {
        inventoryUI.SetActive(false);
    }

    public void GiveGift()
    {
        Debug.Log("아이템을 선물했습니다.");
        inventoryUI.SetActive(false);
        talkUI.SetActive(true);
    }
}