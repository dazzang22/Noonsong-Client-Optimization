using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BackEnd;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("메인 캔버스")]
    public GameObject mainCanvas; // Main_Canvas (기본 화면)
    public GameObject settingCanvas; // Setting_Canvas (설정 화면)

    [Header("각 세부 설정 패널")]
    public GameObject settingAccountPanel; // 계정 설정 패널
    public GameObject settingNoticePanel; // 알림 패널
    public GameObject settingInfoPanel; // 정보 패널
    public GameObject settingSoundPanel; // 사운드 설정 패널

    [Header("버튼")]
    public Button menuButton; // 메뉴 버튼
    public Button accountButton; // 계정 설정 버튼
    public Button noticeButton; // 알림 버튼
    public Button infoButton; // 정보 버튼
    public Button soundButton; // 사운드 버튼
    public Button closeSettingButton; // 설정 닫기 버튼 (전체 닫기)

    [Header("계정 설정 패널 버튼")]
    public Button[] logoutButtons; // 로그아웃 버튼
    public Button[] deleteAccountButtons; // 계정 탈퇴 버튼
    public Button exitButton; // 로그아웃 버튼

    [Header("패널 닫기 버튼")]
    public Button accountCloseButton; // 계정 패널 닫기 버튼
    public Button noticeCloseButton; // 알림 패널 닫기 버튼
    public Button infoCloseButton; // 정보 패널 닫기 버튼
    public Button soundCloseButton; // 사운드 패널 닫기 버튼

    [Header("계정 정보 UI")]
    public TMP_Text nicknameText; // 닉네임 표시 텍스트
    public TMP_Text idText; // 아이디 표시 텍스트

    [Header("사운드 설정 UI")]
    public Slider bgmSlider; // 배경음 조절 슬라이더
    public Slider sfxSlider; // 효과음 조절 슬라이더
    public Slider vibrationSlider; // 진동 조절 슬라이더

    void Start()
    {
        // 패널 초기 상태 비활성화
        settingCanvas.SetActive(false);
        settingAccountPanel.SetActive(false);
        settingNoticePanel.SetActive(false);
        settingInfoPanel.SetActive(false);
        settingSoundPanel.SetActive(false);

        // 버튼 이벤트 연결
        menuButton.onClick.AddListener(OpenSettings);
        closeSettingButton.onClick.AddListener(CloseSettings);

        accountButton.onClick.AddListener(OpenAccountPanel);
        noticeButton.onClick.AddListener(() => OpenPanel(settingNoticePanel));
        infoButton.onClick.AddListener(() => OpenPanel(settingInfoPanel));
        soundButton.onClick.AddListener(OpenSoundPanel);

        // 닫기 버튼 이벤트 연결
        accountCloseButton.onClick.AddListener(() => ClosePanel(settingAccountPanel));
        noticeCloseButton.onClick.AddListener(() => ClosePanel(settingNoticePanel));
        infoCloseButton.onClick.AddListener(() => ClosePanel(settingInfoPanel));
        soundCloseButton.onClick.AddListener(() => ClosePanel(settingSoundPanel));

        // 로그아웃 & 계정 삭제 버튼 이벤트 연결
        foreach (Button logoutBtn in logoutButtons)
        {
            logoutBtn.onClick.AddListener(Logout);
        }

        foreach (Button deleteBtn in deleteAccountButtons)
        {
            deleteBtn.onClick.AddListener(DeleteAccount);
        }

        exitButton.onClick.AddListener(ExitGame);

        // 슬라이더 값 변경 이벤트 추가
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        vibrationSlider.onValueChanged.AddListener(SetVibration);

        // 저장된 설정 불러오기
        LoadSoundSettings();
    }

    // 설정 화면 활성화 (메인 화면 비활성화)
    private void OpenSettings()
    {
        settingCanvas.SetActive(true);
        mainCanvas.SetActive(false);
    }

    // 설정 화면 닫기 (메인 화면 다시 활성화)
    private void CloseSettings()
    {
        settingCanvas.SetActive(false);
        mainCanvas.SetActive(true);
    }
    // 계정 패널 열기 + 닉네임 & 아이디 불러오기
    private void OpenAccountPanel()
    {
        settingAccountPanel.SetActive(true);
        LoadUserInfo();
    }

    // 사운드 설정 패널 열기
    private void OpenSoundPanel()
    {
        settingSoundPanel.SetActive(true);
        LoadSoundSettings(); // 저장된 설정 불러오기
    }

    // 특정 패널 열기
    private void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    // 특정 패널 닫기
    private void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    // 로그아웃 기능
    private void Logout()
    {
        Backend.BMember.Logout(); // 백엔드 로그아웃 처리
        Debug.Log("로그아웃 완료");

        SceneManager.LoadScene("LoginScene");
    }

    private void ExitGame()
    {
        Debug.Log("exit");
        Application.Quit(); // 애플리케이션 종료

        // 유니티 에디터 빼도됨
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void DeleteAccount()
    {
        BackendReturnObject bro = Backend.BMember.WithdrawAccount(); // 계정 탈퇴 요청
        if (bro.IsSuccess())
        {
            Debug.Log("계정 탈퇴 완료");

            SceneManager.LoadScene("LoginScene");
        }
        else
        {
            Debug.LogError("계정 탈퇴 실패: " + bro.GetMessage());
        }
    }


    private void LoadUserInfo()
    {
        BackendReturnObject bro = Backend.BMember.GetUserInfo();
        if (bro.IsSuccess())
        {
            string nickname = bro.GetReturnValuetoJSON()["row"]["nickname"].ToString();
            string userID = bro.GetReturnValuetoJSON()["row"]["gamer_id"].ToString(); // 아이디 가져오기

            nicknameText.text = $"닉네임: {nickname}";
            idText.text = $"아이디: {userID}";
        }
        else
        {
            Debug.LogError("유저 정보 가져오기 실패: " + bro.GetMessage());
            nicknameText.text = "닉네임 불러오기 실패";
            idText.text = "아이디 불러오기 실패";
        }
    }

    // 저장된 사운드 설정 불러오기
    private void LoadSoundSettings()
    {
        bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1.0f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        vibrationSlider.value = PlayerPrefs.GetFloat("Vibration", 1.0f);
    }

    // BGM 볼륨 조절
    private void SetBGMVolume(float volume)
    {
        AudioListener.volume = volume; // 전체 오디오 볼륨 조절 (예제)
        PlayerPrefs.SetFloat("BGMVolume", volume);
    }

    // 효과음 볼륨 조절
    private void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    // 진동 설정
    private void SetVibration(float intensity)
    {
        PlayerPrefs.SetFloat("Vibration", intensity);
        if (intensity > 0)
        {
            Handheld.Vibrate(); // 진동 실행
        }
    }
}
