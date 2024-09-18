using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class NoonsongManager : MonoBehaviour
{
    public GameObject discoveredEntryPrefab;    // 발견된 항목 프리팹
    public GameObject undiscoveredEntryPrefab;  // 발견되지 않은 항목 프리팹
    public Transform entryParent;               // 도감 항목의 부모 오브젝트
    public GameObject detailsPanel;             // 엔트리 상세 정보를 표시할 패널
    public TextMeshProUGUI detailsNameText;     // 상세 정보의 이름 텍스트
    public TextMeshProUGUI detailsDescriptionText;
    public Image detailsImage;
    public Button view3DButton;
    public Canvas collectionCanvas;
    public Camera renderCamera;                // 3D 프리팹을 렌더링할 카메라

    [SerializeField]
    private NoonsongEntryManager noonsongEntryManager; // NoonsongEntryManager 참조

    private List<NoonsongEntry> entries;         // 모든 도감 항목 리스트
    private GameObject lastClickedEntry;         // 마지막으로 클릭된 항목을 추적

    void Start()
    {
        // entries 리스트를 NoonsongEntryManager에서 가져옴
        if (noonsongEntryManager != null)
        {
            entries = new List<NoonsongEntry>(noonsongEntryManager.GetNoonsongEntries());
        }
        else
        {
            Debug.LogError("NoonsongEntryManager is not assigned in the inspector!");
        }

        PopulateNoonsong();
    }

    public void PopulateNoonsong()
    {
        foreach (Transform child in entryParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var entry in entries)
        {
            GameObject newEntry;
            if (entry.isDiscovered)
            {
                newEntry = Instantiate(discoveredEntryPrefab, entryParent);

                var eventTrigger = newEntry.GetComponent<EventTrigger>();
                if (eventTrigger == null)
                {
                    eventTrigger = newEntry.AddComponent<EventTrigger>();
                }
                AddEventTriggerListener(eventTrigger, EventTriggerType.PointerClick, () => ShowDetails(entry, newEntry));

                var noonsongImage = newEntry.transform.Find("NoonsongImage").GetComponent<Image>();
                if (noonsongImage != null) noonsongImage.sprite = entry.noonsongSprite;
            }
            else
            {
                newEntry = Instantiate(undiscoveredEntryPrefab, entryParent);
            }
        }
    }

    void AddEventTriggerListener(EventTrigger trigger, EventTriggerType eventType, System.Action action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener((eventData) => action());
        trigger.triggers.Add(entry);
    }

    public void DiscoverItem(NoonsongEntry entry)
    {
        entry.isDiscovered = true;
        PopulateNoonsong();
    }

    void ShowDetails(NoonsongEntry entry, GameObject clickedEntry)
    {
        Debug.Log($"ShowDetails called for entry: {entry.noonsongName}");

        if (lastClickedEntry != null && lastClickedEntry != clickedEntry)
        {
            // 이전에 클릭된 항목의 배경을 원래 상태로 되돌림
            var lastBackground = lastClickedEntry.transform.Find("BackgroundImage").gameObject;
            var lastHighlight = lastClickedEntry.transform.Find("HighlightBackground").gameObject;

            if (lastBackground != null && lastHighlight != null)
            {
                lastBackground.SetActive(true);
                lastHighlight.SetActive(false);
            }
        }

        if (detailsPanel != null)
        {
            detailsPanel.SetActive(true);
            if (detailsNameText != null)
            {
                detailsNameText.text = entry.noonsongName;
                detailsDescriptionText.text = entry.description;
                detailsImage.sprite = entry.noonsongSprite;

                Debug.Log($"Details panel updated with entry: {entry.noonsongName}");
            }

            if (view3DButton != null)
            {
                view3DButton.onClick.RemoveAllListeners();
                view3DButton.onClick.AddListener(() => Open3DView(entry));
                Debug.Log("3D View button click event added!");
            }
        
        }
        else
        {
            Debug.LogError("DetailsPanel is not set in the inspector!");
        }

        // 현재 클릭된 항목의 배경을 변경
        var background = clickedEntry.transform.Find("BackgroundImage").gameObject;
        var highlight = clickedEntry.transform.Find("HighlightBackground").gameObject;

        if (background != null && highlight != null)
        {
            background.SetActive(false);
            highlight.SetActive(true);
        }

        // 마지막으로 클릭된 항목을 현재 항목으로 설정
        lastClickedEntry = clickedEntry;
    }
    public void Open3DView(NoonsongEntry entry)
    {
        Debug.Log($"Open3DView called for entry: {entry.noonsongName}");

        // CollectionCanvas를 비활성화
        if (collectionCanvas != null)
        {
            collectionCanvas.gameObject.SetActive(false);
            Debug.Log("CollectionCanvas is now deactivated.");
        }
        else
        {
            Debug.LogError("CollectionCanvas is not set in the inspector!");
            return;
        }

        // 카메라 뷰 내의 랜덤 위치 계산 (휴대폰 화면 내)
        if (renderCamera != null && entry.prefab != null)
        {
            // 카메라 뷰포트에서 랜덤 좌표 생성 (휴대폰 화면 범위 내)
            float randomX = Random.Range(0.1f, 0.9f); // X축에서 0.1~0.9의 범위
            float randomY = Random.Range(0.1f, 0.9f); // Y축에서 0.1~0.9의 범위

            // 휴대폰 화면의 랜덤 좌표를 월드 좌표로 변환
            Vector3 randomViewportPosition = new Vector3(randomX, randomY, renderCamera.nearClipPlane + 2f); // 카메라 앞 2 단위 거리
            Vector3 randomWorldPosition = renderCamera.ViewportToWorldPoint(randomViewportPosition);

            // 3D 프리팹을 랜덤한 위치에 생성
            GameObject entryInstance = Instantiate(entry.prefab, randomWorldPosition, Quaternion.identity);
            Debug.Log($"Prefab instantiated at random position in screen: {randomWorldPosition}");

            // 필요하다면 프리팹의 크기나 회전 설정
            entryInstance.transform.LookAt(renderCamera.transform); // 카메라를 바라보게 설정
        }
        else
        {
            Debug.LogError("RenderCamera or entry.prefab is not set!");
        }
    }

}


