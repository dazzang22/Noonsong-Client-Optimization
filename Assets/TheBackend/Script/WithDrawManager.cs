using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using TMPro;
using UnityEngine.SceneManagement;

public class WithdrawManager : MonoBehaviour
{
    [Header("탈퇴 UI 패널")]
    public GameObject withdrawConfirmPanel;  // 패널1 (정말 탈퇴하시겠습니까?)
    public GameObject withdrawCompletePanel; // 패널2 (탈퇴 완료)

    [Header("버튼")]
    public Button confirmWithdrawButton; // 패널1 '예' 버튼
    public Button cancelWithdrawButton;  // 패널1 '아니오' 버튼
    public Button completeWithdrawButton; // 패널2 '확인' 버튼

    void Start()
    {
        // 처음에 패널 비활성화
        withdrawConfirmPanel.SetActive(false);
        withdrawCompletePanel.SetActive(false);

        // 버튼 이벤트 등록
        confirmWithdrawButton.onClick.AddListener(WithdrawAccount);
        cancelWithdrawButton.onClick.AddListener(CloseConfirmPanel);
        completeWithdrawButton.onClick.AddListener(ReturnToStartScene);
    }

    // 회원 탈퇴 확인 패널 표시
    public void OpenConfirmPanel()
    {
        withdrawConfirmPanel.SetActive(true);
    }

    // 회원 탈퇴 확인 패널 닫기
    void CloseConfirmPanel()
    {
        withdrawConfirmPanel.SetActive(false);
    }

    // 회원 탈퇴 처리
    void WithdrawAccount()
    {
        Debug.Log("회원 탈퇴 요청 중...");

        Backend.BMember.WithdrawAccount(callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log("회원 탈퇴 성공");
                withdrawConfirmPanel.SetActive(false);  // 패널1 닫기
                withdrawCompletePanel.SetActive(true); // 패널2 열기
            }
            else
            {
                Debug.LogError($"회원 탈퇴 실패 {callback}");
            }
        });
    }

    //시작 씬으로 이동
    void ReturnToStartScene()
    {
        SceneManager.LoadScene("LoginScene"); // StartScene 이름 맞게 변경
    }
}
