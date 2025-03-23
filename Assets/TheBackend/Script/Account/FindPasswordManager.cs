using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BackEnd;

public class FindPasswordManager : LoginBase
{
    [SerializeField] private TMP_InputField inputFieldID;
    [SerializeField] private TMP_InputField inputFieldEmail;
    [SerializeField] private TMP_Text errorText;
    [SerializeField] private Button findPasswordButton;
    [SerializeField] private GameObject findPWPanel;
    [SerializeField] private GameObject findPWSuccessPanel;


    private const string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"; 

    private void Start()
    {
        findPasswordButton.onClick.AddListener(OnClickFindPW); 
    }

    public void OnClickFindPW()
    {
        errorText.text = "";
        findPasswordButton.interactable = false;

        if (string.IsNullOrWhiteSpace(inputFieldID.text))
        {
            errorText.text = "아이디를 입력해주세요.";
            findPasswordButton.interactable = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(inputFieldEmail.text))
        {
            errorText.text = "이메일을 입력해주세요.";
            findPasswordButton.interactable = true;
            return;
        }

        if (!Regex.IsMatch(inputFieldEmail.text, emailPattern))
        {
            errorText.text = "이메일 형식이 올바르지 않습니다.";
            findPasswordButton.interactable = true;
            return;
        }

        errorText.text = "메일 발송 중..."; // 상태 메시지 출력
        FindCustomPW();
    }

    private void FindCustomPW()
    {
        Backend.BMember.ResetPassword(inputFieldID.text, inputFieldEmail.text, callback =>
        {
            findPasswordButton.interactable = true;

            if (callback.IsSuccess())
            {
                findPWPanel.SetActive(false);
                findPWSuccessPanel.SetActive(true);
                errorText.text = " ";
                return;
            }

            Debug.LogError($"{callback.GetMessage()}");
            string message;
            switch (int.Parse(callback.GetStatusCode()))
            {
                case 404:
                    message = "해당 이메일을 사용하는 사용자가 없습니다.";
                    break;
                case 400:
                    message = "아이디 또는 이메일을 다시 확인해주세요.";
                    break;
                case 429:
                    message = "24시간 이내에 5회 이상 비밀번호 찾기를 시도했습니다.";
                    break;
                default:
                    message = callback.GetMessage();
                    break;
            }

            errorText.text = message;
        });
    }
}
