using UnityEngine;
using UnityEngine.UI;

public class AlarmManager : MonoBehaviour
{
    public GameObject alarmPanel; // 알람 패널
    public Button alarmButton; // 알람 버튼
    public Button closeButton; // 닫기 버튼
    public Text alarmText; // 알람 텍스트

    void Start()
    {
        // 초기 상태에서 알람 패널 비활성화
        alarmPanel.SetActive(false);

        // 버튼 클릭 이벤트 리스너 추가
        alarmButton.onClick.AddListener(ShowAlarm);
        closeButton.onClick.AddListener(CloseAlarm);
    }

    void ShowAlarm()
    {
        // 알람 메시지 설정 (원하는 메시지로 변경 가능)
        alarmText.text = "This is an alarm message!";

        // 알람 패널 활성화
        alarmPanel.SetActive(true);
    }

    void CloseAlarm()
    {
        // 알람 패널 비활성화
        alarmPanel.SetActive(false);
    }
}