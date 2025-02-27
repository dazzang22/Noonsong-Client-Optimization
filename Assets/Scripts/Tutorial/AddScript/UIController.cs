using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    
    [Header("Canvas")]
    [SerializeField] private Canvas friendsCanvas;
    [SerializeField] private Canvas mapCanvas;
    [SerializeField] private Canvas inventoryCanvas;
    [SerializeField] private Canvas turyCanvas;
    [SerializeField] private Canvas bookCanvas;
    [SerializeField] private Canvas giftCanvas;
    
    
    [Header("Panel")]
    [SerializeField] private GameObject selectPanel;
    [SerializeField] private GameObject skipPanel;
    [SerializeField] private GameObject goodsPanel;
    [SerializeField] private GameObject mapPanel;
    [SerializeField] private GameObject bookPanel;
    [SerializeField] private GameObject likePanel;
    [SerializeField] private GameObject turyPanel;

    [Header("MainButton")]
    [SerializeField] private Button friendsButton;
    [SerializeField] private Button mapButton;
    [SerializeField] private Button interactionButton;
    [SerializeField] private Button interactionButton2;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button turyButton;
    [SerializeField] private Button bookButton;
    [SerializeField] private Button greetButton;
    [SerializeField] private Button giftButton;
    [SerializeField] private Button skipButton;

    [Header("AcceptButton")]
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    [SerializeField] private Button goodsPopButton;
    [SerializeField] private Button mapPopButton;
    [SerializeField] private Button bookPopButton;
    [SerializeField] private Button likePopButton;
    [SerializeField] private Button turyPopButton;
    [SerializeField] private Button bookXButton;


    private bool isClicked = false;

    void Start()
    {
        //캔버스 초기화
        friendsCanvas.gameObject.SetActive(false);
        mapCanvas.gameObject.SetActive(false);
        inventoryCanvas.gameObject.SetActive(false);
        turyCanvas.gameObject.SetActive(false);
        bookCanvas.gameObject.SetActive(false);
        giftCanvas.gameObject.SetActive(false);

        //패널 초기화
        skipPanel.SetActive(false);
        goodsPanel.SetActive(false);
        mapPanel.SetActive(false);
        bookPanel.SetActive(false);
        likePanel.SetActive(false);
        turyPanel.SetActive(false);


        //버튼이벤트 추가
        greetButton.onClick.AddListener(() => isClicked = true);
        giftButton.onClick.AddListener(() => isClicked = true);
        goodsPopButton.onClick.AddListener(() => isClicked = true);
        mapPopButton.onClick.AddListener(() => isClicked = true);
        bookPopButton.onClick.AddListener(() => isClicked = true);
        likePopButton.onClick.AddListener(() => isClicked = true);
        turyPopButton.onClick.AddListener(() => isClicked = true);
        bookXButton.onClick.AddListener(() => isClicked = true);
        
        friendsButton.onClick.AddListener(ToggleFriendsCanvas);
        skipButton.onClick.AddListener(ToggleSkipPanel);

        yesButton.onClick.AddListener(SkipTutorial);
        noButton.onClick.AddListener(HideSkipPanel);
    }

    //onClick
    public void onClickMapButton()
    {
        mapButton.onClick.AddListener(ToggleMapCanvas);
    }

    public void onClickInventoryButton()
    {
        inventoryButton.onClick.AddListener(ToggleInventoryCanvas);
    }

    public void onClickInteractoinButton()
    {
        interactionButton.onClick.AddListener(ToggleInteractionCanvas);
        interactionButton2.onClick.AddListener(ToggleInteractionCanvas);
    }

    public void onClickTuryButton()
    {
        turyButton.onClick.AddListener(ToggleTuryCanvas);
    }

    public void onClickBookButton()
    {
        bookButton.onClick.AddListener(ToggleBookCanvas);
    }

    public void onClickGiftButton()
    {
        giftButton.onClick.AddListener(ActivateGiftCanvas);
    }

    


    //toggleCanvas
    private void ToggleFriendsCanvas()
    {
        friendsCanvas.gameObject.SetActive(!friendsCanvas.gameObject.activeSelf);
    }

    private void ToggleMapCanvas()
    {
        mapCanvas.gameObject.SetActive(!mapCanvas.gameObject.activeSelf);
    }

    private void ToggleBookCanvas()
    {
        bookCanvas.gameObject.SetActive(!bookCanvas.gameObject.activeSelf);
    }

    private void ToggleTuryCanvas()
    {
        turyCanvas.gameObject.SetActive(!turyCanvas.gameObject.activeSelf);
    }

    private void ToggleInventoryCanvas()
    {
        inventoryCanvas.gameObject.SetActive(!inventoryCanvas.gameObject.activeSelf);
    }

    private void ToggleInteractionCanvas()
    {
        selectPanel.SetActive(!selectPanel.gameObject.activeSelf);
    }

     private void ToggleSkipPanel()
    {
        skipPanel.SetActive(!skipPanel.activeSelf);
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

    public bool IsPanelActive()
    {
        return bookPanel.activeSelf || likePanel.activeSelf;
    }



    //ActivePanel
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

    private void ActivateGiftCanvas()
    {
        selectPanel.SetActive(false);
        giftCanvas.gameObject.SetActive(true);
    }


    //popup
    public void PopUpGoodsPanel()
    {
        goodsPanel.SetActive(true);
    }

    public void PopUpMapPanel()
    {
        mapPanel.SetActive(true);
    }

    public void PopUPBookPanel()
    {
        bookPanel.SetActive(true);
    }

    public void PopUpLikePanel()
    {
        likePanel.SetActive(true);
    }
    
    public void PopUpTuryPanel()
    {
        turyPanel.SetActive(true);
    }
}
