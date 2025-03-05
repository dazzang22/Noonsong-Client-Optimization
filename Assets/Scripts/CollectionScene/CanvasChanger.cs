using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasChanger : MonoBehaviour
{
  [Header ("Canvas")]
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Canvas popupCanvas;
    [SerializeField] private Canvas turyCanvas;
    [SerializeField] private Canvas mapCanvas;
    [SerializeField] private Canvas inventoryCanvas;
    [SerializeField] private Canvas emailVerifyCanvas;
    [SerializeField] private Canvas changePasswordCanvas;
    [SerializeField] private Canvas profileCanvas;
    [SerializeField] GameObject mainTutoCanvas;
    [SerializeField] GameObject mainTutoPanel;

    [Header ("Button")]
    [SerializeField] private Button popupButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button turyButton;
    [SerializeField] private Button turyBackButton;
    [SerializeField] private Button mapButton;
    [SerializeField] private Button mapbackButton;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button inventoryBackButton;
    [SerializeField] private Button nicknameButton;
    [SerializeField] private Button emailVerifyButton;
    [SerializeField] private Button emailVerifyBackButton1;
    [SerializeField] private Button emailVerifyBackButton2;
    [SerializeField] private Button emailVerifyBackButton3;
    [SerializeField] private Button changePasswordButton;
    [SerializeField] private Button changePasswordBackButton1;
    [SerializeField] private Button changePasswordBackButton2;
    [SerializeField] private Button changePasswordBackButton3;




  public MapManager mapManager;

  private void Awake()
  {
    PlayerPrefs.DeleteKey("MainTutoPopupActivated"); //테스트용
    mainCanvas.gameObject.SetActive(true);
    popupCanvas.gameObject.SetActive(false);
    turyCanvas.gameObject.SetActive(false);
    mapCanvas.gameObject.SetActive(false);
    inventoryCanvas.gameObject.SetActive(false);
    profileCanvas.gameObject.SetActive(false);
    emailVerifyCanvas.gameObject.SetActive(false);
    changePasswordCanvas.gameObject.SetActive(false);

    popupButton.onClick.AddListener(ShowPopup);
    backButton.onClick.AddListener(HidePopup);
    turyButton.onClick.AddListener(ToggleTuryCanvas);
    turyBackButton.onClick.AddListener(ToggleTuryCanvas);
    mapButton.onClick.AddListener(ToggleMapCanvas);
    mapbackButton.onClick.AddListener(ToggleMapCanvas);
    inventoryButton.onClick.AddListener(ToggleInventoryCanvas);
    inventoryBackButton.onClick.AddListener(ToggleInventoryCanvas);
    nicknameButton.onClick.AddListener(ToggleProfileCanvas);
    emailVerifyButton.onClick.AddListener(ToggleEmailVerifyCanvas);
    emailVerifyBackButton1.onClick.AddListener(ToggleEmailVerifyCanvas);
    emailVerifyBackButton2.onClick.AddListener(ToggleEmailVerifyCanvas);
    emailVerifyBackButton3.onClick.AddListener(ToggleEmailVerifyCanvas);
    changePasswordButton.onClick.AddListener(ToggleChangePasswordCanvas);
    changePasswordBackButton1.onClick.AddListener(ToggleChangePasswordCanvas);
    changePasswordBackButton2.onClick.AddListener(ToggleChangePasswordCanvas);
    changePasswordBackButton3.onClick.AddListener(ToggleChangePasswordCanvas);
  
  }

    void Start()
    {
      if (PlayerPrefs.GetInt("MainTutoPopupActivated", 0) == 0) 
      {
          StartCoroutine(ShowMainTuto());
          PlayerPrefs.SetInt("MainTutoPopupActivated", 1); // 상태 저장
          PlayerPrefs.Save(); // 저장 확정
      }
      else
      {
          mainTutoCanvas.SetActive(false);
      }
      
        
    }

    private IEnumerator ShowMainTuto()
    {
        Time.timeScale = 0f;
        mainTutoCanvas.SetActive(true);

        // 첫 번째 클릭 대기
        while (!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }
        yield return new WaitForSecondsRealtime(0.1f);

        // mainPanel 활성화
        mainTutoPanel.SetActive(true);

        // 두 번째 클릭 대기
        while (!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }

        // mainPanel & mainCanvas 비활성화
        mainTutoPanel.SetActive(false);
        mainTutoCanvas.SetActive(false);
        Time.timeScale = 1f;

    }


    private void ShowPopup()
    {
        popupCanvas.gameObject.SetActive(true);
        mainCanvas.gameObject.SetActive(false);
    }

    private void HidePopup()
    {
        mainCanvas.gameObject.SetActive(true);
        popupCanvas.gameObject.SetActive(false);
    }

    private void ToggleTuryCanvas()
    {
        // 현재 활성 상태를 반전
        turyCanvas.gameObject.SetActive(!turyCanvas.gameObject.activeSelf);
    }

    private void ToggleMapCanvas()
    {
        mapCanvas.gameObject.SetActive(!mapCanvas.gameObject.activeSelf);
        mapManager.LoadMapState();
    }

    private void ToggleInventoryCanvas()
    {
      inventoryCanvas.gameObject.SetActive(!inventoryCanvas.gameObject.activeSelf);
    }

    private void ToggleProfileCanvas()
    {
      profileCanvas.gameObject.SetActive(!profileCanvas.gameObject.activeSelf);
    }

    private void ToggleEmailVerifyCanvas()
    {
      emailVerifyCanvas.gameObject.SetActive(!emailVerifyCanvas.gameObject.activeSelf);
      EmailVerifyManager emailVerifyManager = GameObject.Find("EmailVerifyManager").GetComponent<EmailVerifyManager>();
      emailVerifyManager.ClearInputText();
    }

    private void ToggleChangePasswordCanvas()
    {
      changePasswordCanvas.gameObject.SetActive(!changePasswordCanvas.gameObject.activeSelf);
      ChangePasswordManager changePasswordManager = GameObject.Find("ChangePasswordManager").GetComponent<ChangePasswordManager>();
      changePasswordManager.ClearInputText();
    }
}
