using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    

    [SerializeField] private Canvas friendsCanvas;
    [SerializeField] private Canvas mapCanvas;
    [SerializeField] private GameObject selectPanel;
    [SerializeField] private GameObject skipPanel;

    [SerializeField] private Button friendsButton;
    [SerializeField] private Button mapButton;
    [SerializeField] private Button interactionButton;
    [SerializeField] private Button greetButton;
    [SerializeField] private Button giftButton;

    private bool isClicked = false;
    [SerializeField] private Button skipButton;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    void Start()
    {
        friendsCanvas.gameObject.SetActive(false);
        mapCanvas.gameObject.SetActive(false);
        skipPanel.SetActive(false);

        greetButton.onClick.AddListener(() => isClicked = true);
        giftButton.onClick.AddListener(() => isClicked = true);
        
        friendsButton.onClick.AddListener(ToggleFriendsCanvas);
        
        skipButton.onClick.AddListener(ToggleSkipPanel);
        yesButton.onClick.AddListener(SkipTutorial);
        noButton.onClick.AddListener(HideSkipPanel);
    }

    public void onClickMapbutton()
    {
        mapButton.onClick.AddListener(ToggleMapCanvas);
    }

    public void onClickInteractoinButton()
    {
        interactionButton.onClick.AddListener(ToggleInteractiveCanvas);
    }


    private void ToggleFriendsCanvas()
    {
        friendsCanvas.gameObject.SetActive(!friendsCanvas.gameObject.activeSelf);
    }

    private void ToggleMapCanvas()
    {
        mapCanvas.gameObject.SetActive(!mapCanvas.gameObject.activeSelf);
    }

    private void ToggleInteractiveCanvas()
    {
        selectPanel.SetActive(!selectPanel.gameObject.activeSelf);
    }

    public bool IsButtonClicked()
    {
        if (isClicked)
        {
            isClicked = false; // 한 번 확인 후 다시 false로 초기화
            return true;
        }
        return false;
    }

    private void ToggleSkipPanel()
    {
        skipPanel.SetActive(!skipPanel.activeSelf);
    }

    private void SkipTutorial()
    {
        // "MainScene"으로 전환
        SceneManager.LoadScene("MainScene(Release)");
    }

    private void HideSkipPanel()
    {
        // skipCanvas 비활성화
        skipPanel.SetActive(false);
    }
}
