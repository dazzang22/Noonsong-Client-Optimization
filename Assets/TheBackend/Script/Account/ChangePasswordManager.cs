using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BackEnd;
using System.Text.RegularExpressions; 

public class ChangePasswordManager : MonoBehaviour
{
    public GameObject oldPasswordPanel;
    public GameObject newPasswordPanel;
    public GameObject successPanel;

    public TMP_InputField oldPasswordInput;
    public TMP_Text oldPasswordResultText;
    public TMP_InputField newPasswordInput;
    public TMP_Text newPasswordResultText;
    public TMP_InputField confirmNewPasswordInput;

    public Button confirmOldPasswordButton;
    public Button confirmNewPasswordButton;

    private string oldPassword;
    private string newPassword;

    void Start()
    {
        SetupPasswordChangeFlow();
    }

    void SetupPasswordChangeFlow()
    {
        oldPasswordPanel.SetActive(true);
        newPasswordPanel.SetActive(false);
        successPanel.SetActive(false);

        confirmOldPasswordButton.onClick.AddListener(OnConfirmOldPassword);
        confirmNewPasswordButton.onClick.AddListener(OnConfirmNewPassword);
    }

    public void OnConfirmOldPassword()
    {
        oldPassword = oldPasswordInput.text;
        Debug.Log($"입력된 기존 비밀번호: '{oldPassword}'");


        Backend.BMember.ConfirmCustomPassword(oldPassword, (callback) =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log("비밀번호 확인 성공");
                oldPasswordResultText.text = "비밀번호 확인 완료!";
                oldPasswordResultText.color = Color.green;

                oldPasswordPanel.SetActive(false);
                newPasswordPanel.SetActive(true);
            }
            else
            {
                Debug.LogError("비밀번호 확인 실패: " + callback);
                oldPasswordResultText.text = "잘못된 비밀번호입니다.";
                oldPasswordResultText.color = Color.red;
            }
        });
    }

    public void OnConfirmNewPassword()
    {
        newPassword = newPasswordInput.text;
        string confirmPassword = confirmNewPasswordInput.text;

        if (!IsValidPassword(newPassword))
        {
            newPasswordResultText.text = "비밀번호는 8~12자이며, 대문자, 소문자, 숫자를 각각 1개 이상 포함해야 합니다.";
            newPasswordResultText.color = Color.red;
            return;
        }

        if (newPassword != confirmPassword)
        {
            newPasswordResultText.text = "비밀번호가 일치하지 않습니다.";
            newPasswordResultText.color = Color.red;
            return;
        }

        Backend.BMember.UpdatePassword(oldPassword, newPassword, (callback) =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log("비밀번호 변경 성공!");
                newPasswordPanel.SetActive(false);
                successPanel.SetActive(true);
                //db 연결
                UserDataManager.Instance.ChangePassword(newPassword);
            }
            else
            {
                Debug.LogError("비밀번호 변경 실패: " + callback);
                newPasswordResultText.text = "비밀번호 변경 실패! 다시 시도하세요.";
                newPasswordResultText.color = Color.red;
            }
        });
    }

    public void OnCloseSuccessPanel()
    {
        successPanel.SetActive(false);
        Debug.Log("비밀번호 변경 완료!");
    }

    private bool IsValidPassword(string password)
    {
        return Regex.IsMatch(password, @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)[A-Za-z\d]{8,12}$");
    }

    public void ClearInputText()
    {
        oldPasswordInput.text = "";
        newPasswordInput.text = "";
        confirmNewPasswordInput.text = "";
        oldPasswordPanel.SetActive(true);
        newPasswordPanel.SetActive(false);
        successPanel.SetActive(false);
    }
}
