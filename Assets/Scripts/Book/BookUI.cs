using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BookUI : MonoBehaviour
{
    [SerializeField] private GameObject bookPanel;

    [SerializeField] private Transform defaultSlotParent; // 기본 눈송이 슬롯 부모
    [SerializeField] private Transform universitySlotParentTemplate; // 대학별 슬롯 부모 템플릿
    [SerializeField] private GameObject slotPrefab; // 눈송이 슬롯 프리팹

    [SerializeField] private GameObject defaultPage; // 기본 페이지
    [SerializeField] private GameObject universityListPage; // 대학 목록 페이지
    [SerializeField] private GameObject universityDetailPage; // 대학별 눈송이 페이지

    [SerializeField] private Image selectedNoonsongImage; // 선택된 눈송이 이미지
    [SerializeField] private TextMeshProUGUI selectedNoonsongNameText; // 선택된 눈송이 이름
    [SerializeField] private TextMeshProUGUI selectedNoonsongDescriptionText; // 선택된 눈송이 설명
    [SerializeField] private Image[] loveMeterImages; // 호감도 이미지 배열

    [SerializeField] private List<Button> universityButtons; // 대학별 버튼 리스트

    private Dictionary<string, Transform> universitySlotParents; // 대학별 슬롯 부모 매핑
    private Dictionary<string, List<NoonsongEntry>> universityNoonsongs;
    private List<BookSlot> activeSlots = new List<BookSlot>();

    private string currentUniversity;

    public static BookUI Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OpenBook(List<NoonsongEntry> discoveredNoonsongs)
    {
        UpdateBook(discoveredNoonsongs);
        bookPanel.SetActive(true);
    }

    public void CloseBook()
    {
        bookPanel.SetActive(false);
    }

    public void Initialize(List<string> universities, List<NoonsongEntry> discoveredNoonsongs)
    {
        universitySlotParents = new Dictionary<string, Transform>();
        universityNoonsongs = new Dictionary<string, List<NoonsongEntry>>();

        // 기존 UI의 대학 버튼과 연결
        for (int i = 0; i < universities.Count && i < universityButtons.Count; i++)
        {
            string university = universities[i];
            universityButtons[i].onClick.RemoveAllListeners(); // 기존 리스너 제거
            universityButtons[i].onClick.AddListener(() => ShowUniversityPage(university));

            // 대학별 슬롯 부모 설정
            Transform slotParent = Instantiate(universitySlotParentTemplate, universitySlotParentTemplate.parent);
            slotParent.gameObject.name = university + " Slots";
            slotParent.gameObject.SetActive(false); // 기본적으로 비활성화
            universitySlotParents.Add(university, slotParent);
        }
        universitySlotParentTemplate.gameObject.SetActive(false);

        // 도감에 있는 눈송이 분류
        foreach (var noonsong in discoveredNoonsongs)
        {
            if (!string.IsNullOrEmpty(noonsong.university))
            {
                if (!universityNoonsongs.ContainsKey(noonsong.university))
                {
                    universityNoonsongs[noonsong.university] = new List<NoonsongEntry>();
                }
                universityNoonsongs[noonsong.university].Add(noonsong);
            }
        }
    }

    public void UpdateBook(List<NoonsongEntry> discoveredNoonsongs)
    {
        // 기존 슬롯 삭제
        foreach (var slot in activeSlots)
        {
            Destroy(slot.gameObject);
        }
        activeSlots.Clear();

        // 기본 눈송이 표시
        foreach (var noonsong in discoveredNoonsongs)
        {
            Transform targetParent = string.IsNullOrEmpty(noonsong.university)
                ? defaultSlotParent
                : universitySlotParents.TryGetValue(noonsong.university, out var slotParent)
                    ? slotParent
                    : null;

            if (targetParent == null)
            {
                Debug.LogWarning($"등록되지 않은 대학: {noonsong.university}");
                continue;
            }

            // 슬롯 생성 및 초기화
            var slot = Instantiate(slotPrefab, targetParent).GetComponent<BookSlot>();
            slot.Initialize(noonsong, this); // BookUI 전달
            activeSlots.Add(slot);
        }
    }

    public void ShowDefaultPage()
    {
        defaultPage.SetActive(true);
        universityListPage.SetActive(false);
        universityDetailPage.SetActive(false);
    }

    public void ShowUniversityListPage()
    {
        defaultPage.SetActive(false);
        universityListPage.SetActive(true);
        universityDetailPage.SetActive(false);
    }

    public void ShowUniversityPage(string university)
    {
        if (!universityNoonsongs.ContainsKey(university))
        {
            Debug.LogWarning($"대학 {university}에 대한 데이터가 없습니다.");
            return;
        }

        // 페이지 전환
        defaultPage.SetActive(false);
        universityListPage.SetActive(false);
        universityDetailPage.SetActive(true);

        // 현재 대학 페이지 활성화
        foreach (var entry in universitySlotParents)
        {
            entry.Value.gameObject.SetActive(entry.Key == university);
        }

        currentUniversity = university;
    }

    public void DisplayNoonsongDetails(NoonsongEntry noonsong)
    {
        selectedNoonsongImage.sprite = noonsong.noonsongSprite;
        selectedNoonsongNameText.text = noonsong.noonsongName;
        selectedNoonsongDescriptionText.text = noonsong.description;

        // 호감도 UI 업데이트
        for (int i = 0; i < loveMeterImages.Length; i++)
        {
            loveMeterImages[i].enabled = (i < noonsong.loveLevel);
        }
    }
}