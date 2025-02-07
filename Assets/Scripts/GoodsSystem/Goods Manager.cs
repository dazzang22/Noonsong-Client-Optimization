using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GoodsManager : MonoBehaviour
{
    //5분마다 제공되는 재화
    [SerializeField] private CurrencyManager currencyManager;
    private const int DEFAULT_INCREMENT = 3;   // 5분마다 추가할 재화 양
    private const float INTERVAL = 300f;       // 5분(300초)

    //파견시스템
    [Header("Dispatch UI")]
    [SerializeField] private Button[] dispatchBtns;
    [SerializeField] private TextMeshProUGUI dispatchTimeText; // 남은 시간 텍스트
    [SerializeField] private TextMeshProUGUI rewardText; //파견 후 얻은 재화
    //DispatchPanel
    [SerializeField] private GameObject noonsFriendsListPanel;
    [SerializeField] private GameObject acceptPanel;
    [SerializeField] private GameObject goPanel;
    [SerializeField] private GameObject noPanel;
    [SerializeField] private GameObject completePanel;
    [SerializeField] private GameObject receivePanel;


    [Header("Dispatch Settings")]
    private const int DISPATCH_DURATION = 10; // 파견 총 시간(10분)
    private int remainingMinutes = DISPATCH_DURATION; // 초기 남은 시간(10분)
    private int randomReward; // 파견 랜덤 재화  
    private Coroutine dispatchCoroutine;      // 파견 코루틴 저장
    private bool isNoonsSelected = false; // 눈송이 선택 상태 추적

    [Header("Dispatch Friends")]
    [SerializeField]
    private NoonsongFriendsEntryManager noonsongFriendsEntryManager;

    public GameObject noonsongItemPrefab; // ScrollView의 개별 항목 버튼 프리팹
    public Transform contentParent; // ScrollView의 Content 오브젝트
    public Image noonsFriendsImage; // 눈송이 이미지
    public  TextMeshProUGUI noonsFriendsNameText; // 눈송이 이름
    public  TextMeshProUGUI noonsFriendsDescriptionText; // 눈송이 설명
    private List<NoonsongFriendsEntry> entries = new List<NoonsongFriendsEntry>();


    void Start()
    {
        // CurrencyManager가 연결되어 있는지 확인
        if (currencyManager != null)
        {
            currencyManager = CurrencyManager.Instance;
        }
        // 5분마다 재화를 추가하는 코루틴 시작
        StartCoroutine(AddCurrencyPeriodically());

        if (noonsongFriendsEntryManager != null)
        {
            entries = new List<NoonsongFriendsEntry>(noonsongFriendsEntryManager.GetNoonsongEntries());
        }
        else
        {
            Debug.LogError("NoonsongFriendsEntryManager is not assigned in the inspector!");
        }
        PopulateList();
    }

    private void PopulateList()
    {
        foreach (var entry in entries)
        {
            GameObject newItem = Instantiate(noonsongItemPrefab, contentParent);
            Button button = newItem.GetComponent<Button>();
            Image itemImage = newItem.GetComponent<Image>();

            if (button != null)
            {
                // "눈송이"인 경우 SelectNoons 연결
                if (entry.noonsongFriendName == "눈송이")
                {
                    button.onClick.AddListener(() => SelectNoons(entry));
                }
                else
                {
                    // 나머지 항목에 대해서는 DisplayNoonsongInfo 연결
                    button.onClick.AddListener(() => DisplayNoonsongInfo(entry));
                }
            }

            if (itemImage != null)
            {
                itemImage.sprite = entry.displaySprite;
            }
        }
    }

    public void DisplayNoonsongInfo(NoonsongFriendsEntry noonsongEntry)
    {
        if (noonsongEntry == null) return;

        noonsFriendsImage.sprite = noonsongEntry.noonsongSprite;
        noonsFriendsNameText.text = noonsongEntry.noonsongFriendName;
        isNoonsSelected = false;
        // noonsFriendsDescriptionText.text = noonsongEntry.description;

    }

    // 5분마다 재화를 추가
    private IEnumerator AddCurrencyPeriodically()
    {
        while (true) // 반복
        {
            yield return new WaitForSeconds(INTERVAL); // 5분 대기
            if (currencyManager != null)
            {
                currencyManager.IncreaseCurrency("Default", DEFAULT_INCREMENT);
                Debug.Log($"5분 경과: {DEFAULT_INCREMENT}개의 재화 추가.");
            }
        }
    }

    public void DispatchBtnClick()
    {
        dispatchBtns[0].gameObject.SetActive(false);
        dispatchBtns[1].gameObject.SetActive(true);
        noonsFriendsListPanel.gameObject.SetActive(true);

    }

    public void DispatchBackBtnClick()
    {
        noonsFriendsListPanel.gameObject.SetActive(false);
        dispatchBtns[1].gameObject.SetActive(false);
        dispatchBtns[0].gameObject.SetActive(true);
    }

    //파견 눈송이 선택 및 확정
    public void SelectNoons(NoonsongFriendsEntry noonsongEntry)
    {
        if(!isNoonsSelected)
        {
            DisplayNoonsongInfo(noonsongEntry);
            isNoonsSelected = true;
        }
        else
        {
            //한번 더 클릭 시 아래 문장 진행
            noonsFriendsListPanel.gameObject.SetActive(false);
            acceptPanel.gameObject.SetActive(true);
            isNoonsSelected = false;
        }
    }

    //파견 진행
    public void AccepBtnClick()
    {
        acceptPanel.gameObject.SetActive(false);
        goPanel.gameObject.SetActive(true);
    }

    public void AcceptBackBtnClick()
    {
        acceptPanel.gameObject.SetActive(false);
        dispatchBtns[1].gameObject.SetActive(false);
        dispatchBtns[0].gameObject.SetActive(true);
    }

    public void GoBackBtnClick()
    {
        goPanel.gameObject.SetActive(false);
        dispatchBtns[1].gameObject.SetActive(false);
        dispatchBtns[2].gameObject.SetActive(true);
        Dispatch();
    }
    
    public void NoBackBtnClick()
    {
        noPanel.gameObject.SetActive(false);
    }

    public void CompletBtnClick()
    {
        completePanel.gameObject.SetActive(true);
    }

    public void ReceiveBtnClick()
    {
        completePanel.gameObject.SetActive(false);
        currencyManager.IncreaseCurrency("Default", randomReward);
        Debug.Log($"파견 완료: {randomReward}개의 재화 지급.");
        if (rewardText != null)
        {
            rewardText.text = $"눈의 결정 X {randomReward}";
        }
        receivePanel.gameObject.SetActive(true);
    }

    public void ReceiveBackBtnClick()
    {
        receivePanel.gameObject.SetActive(false);
        dispatchBtns[3].gameObject.SetActive(false);
        dispatchBtns[0].gameObject.SetActive(true);
    }

    // 파견 시스템
    public void Dispatch()
    {
        // 기존에 파견 중인 경우 중단
        if (dispatchCoroutine != null)
        {
            noPanel.gameObject.SetActive(true);
        }
        else
        {
            // 파견 코루틴 시작
            dispatchCoroutine = StartCoroutine(HandleDispatch());
        }
    }

    // 파견 처리 코루틴
    private IEnumerator HandleDispatch()
    {

        while (remainingMinutes > 0)
        {
            // 남은 시간을 텍스트에 표시
            if (dispatchTimeText != null)
            {
                dispatchTimeText.text = $"{remainingMinutes}분";
            }

            yield return new WaitForSeconds(60f); // 1분 대기(60f)
            remainingMinutes--; // 남은 시간 감소
        }

        // 파견 완료 시 보상 지급
        if (currencyManager != null)
        {
            randomReward = Random.Range(1, 11); // 1에서 10까지의 랜덤 값 생성 (포함)
            ChangeCompletImg();
        }
        remainingMinutes = DISPATCH_DURATION;
        dispatchCoroutine = null; // 코루틴 초기화
    }

    //복귀 완료 이미지로 변경 함수
    public void ChangeCompletImg()
    {
        dispatchBtns[2].gameObject.SetActive(false);
        dispatchBtns[3].gameObject.SetActive(true);
    }




}
