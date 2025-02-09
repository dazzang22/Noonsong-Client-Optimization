using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Gift : MonoBehaviour
{
    public GameObject greetingUI;
    public TextMeshProUGUI greetingText;
    public GameObject talkUI;
    public GameObject endTalkPopup;

    private Queue<string> dialogues = new Queue<string>();
    private string currentCharacterName;

    private Dictionary<string, List<string>> characterDialogues = new Dictionary<string, List<string>>
    {
        { "캐릭터1", new List<string> { "안녕하세요!", "오늘도 좋은 하루 되세요!" } },
        { "캐릭터2", new List<string> { "반갑습니다!", "무엇을 도와드릴까요?" } },
        { "캐릭터3", new List<string> { "좋은 날이에요!", "함께 놀아요!" } }
    };

    public void SetCharacter(string characterName)
    {
        currentCharacterName = characterName;
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
        endTalkPopup.SetActive(false);
    }

    public void EnterTalkScreen()
    {
        greetingUI.SetActive(false);
        talkUI.SetActive(true);
        PrepareDialogues();
        ShowNextDialogue();
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
        Debug.Log("대화 종료");
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
            Debug.Log(nextDialogue);
        }
        else
        {
            Debug.Log("대화가 끝났습니다.");
        }
    }
}
