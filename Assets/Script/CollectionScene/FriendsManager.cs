using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class FriendsManager : MonoBehaviour
{
    public GameObject discoveredEntryPrefab;    // 발견된 항목 프리팹
    public GameObject undiscoveredEntryPrefab;  // 발견되지 않은 항목 프리팹
    public Transform entryParent;               // 도감 항목의 부모 오브젝트
    public GameObject detailsPanel;             // 엔트리 상세 정보를 표시할 패널
    public TextMeshProUGUI detailsNameText;     // 상세 정보의 이름 텍스트
    public TextMeshProUGUI detailsDescriptionText;
    public Image detailsImage;
    public Button view3DButton;                 // 3D 보기 버튼
    public Canvas collectionCanvas;             // Collection 화면
    public Canvas cameraCanvas;                 // 3D 보기 화면
    public Camera renderCamera;             // 3D 프리팹을 렌더링할 Render Texture

    [SerializeField]
    private NoonsongFriendsEntryManager noonsongFriendsEntryManager; // NoonsongFriendsEntryManager 참조

    private List<NoonsongFriendsEntry> entries; // 모든 도감 항목 리스트
    private GameObject lastClickedEntry = null; // 마지막 클릭된 항목
    private GameObject currentNoonsongObject;

    void Start()
    {
        // entries 리스트를 NoonsongFriendsEntryManager에서 가져옴
        if (noonsongFriendsEntryManager != null)
        {
            entries = new List<NoonsongFriendsEntry>(noonsongFriendsEntryManager.GetNoonsongEntries());
        }
        else
        {
            Debug.LogError("NoonsongFriendsEntryManager is not assigned in the inspector!");
        }

        PopulateNoonsong();
    }

    public void PopulateNoonsong()
    {
        // 기존의 모든 항목 제거
        foreach (Transform child in entryParent)
        {
            Destroy(child.gameObject);
        }

        // 새로운 항목 생성 및 UI 업데이트
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
                AddEventTriggerListener(eventTrigger, EventTriggerType.PointerClick, () => OnEntryClick(entry, newEntry));

                var noonsongFriendImage = newEntry.transform.Find("NoonsongFriendImage").GetComponent<Image>();
                if (noonsongFriendImage != null) noonsongFriendImage.sprite = entry.noonsongSprite;
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

    void OnEntryClick(NoonsongFriendsEntry entry, GameObject newEntry)
    {
        Debug.Log($"OnEntryClick called for entry: {entry.noonsongFriendName}");

        // 이전에 클릭된 항목이 있으면 원래 이미지와 스케일로 되돌림
        if (lastClickedEntry != null && lastClickedEntry != newEntry)
        {
            var lastEntryImage = lastClickedEntry.transform.Find("NoonsongFriendImage").GetComponent<Image>();
            if (lastEntryImage != null)
            {
                var lastEntryScript = entries.Find(e => e.noonsongSprite == lastEntryImage.sprite || e.clickedNoonsongSprite == lastEntryImage.sprite);
                if (lastEntryScript != null)
                {
                    lastEntryImage.sprite = lastEntryScript.noonsongSprite;
                    lastEntryImage.rectTransform.localScale = Vector3.one; // 스케일을 원래 크기로 되돌림
                }
            }
        }

        // 현재 클릭된 항목의 이미지를 변경하고 스케일을 키움
        var currentEntryImage = newEntry.transform.Find("NoonsongFriendImage").GetComponent<Image>();
        if (currentEntryImage != null)
        {
            currentEntryImage.sprite = entry.clickedNoonsongSprite;
            currentEntryImage.rectTransform.localScale = new Vector3(1.3f, 1.3f, 1.3f); // 스케일을 1.3배로 키움
        }

        // 상세 정보 패널 업데이트
        ShowDetails(entry);

        // 현재 항목을 마지막 클릭 항목으로 설정
        lastClickedEntry = newEntry;
    }

    public void ShowDetails(NoonsongFriendsEntry entry)
    {
        Debug.Log($"ShowDetails called for entry: {entry.noonsongFriendName}");

        if (detailsPanel != null)
        {
            detailsPanel.SetActive(true);
            if (detailsNameText != null)
            {
                detailsNameText.text = entry.noonsongFriendName;
                detailsDescriptionText.text = entry.description;
                detailsImage.sprite = entry.displaySprite;

                // 스케일 조정
                detailsImage.rectTransform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

                Debug.Log($"Details panel updated with entry: {entry.noonsongFriendName}");
                if (view3DButton != null)
                {
                    view3DButton.onClick.RemoveAllListeners();
                    view3DButton.onClick.AddListener(() => Open3DView(entry));
                }
            }
        }
        else
        {
            Debug.LogError("DetailsPanel is not set in the inspector!");
        }
    }
    public void Open3DView(NoonsongFriendsEntry entry)
    {
        // CollectionCanvas를 비활성화
        if (collectionCanvas != null)
        {
            collectionCanvas.gameObject.SetActive(false);
        }

        // CameraCanvas를 활성화
        if (cameraCanvas != null)
        {
            cameraCanvas.gameObject.SetActive(true);
        }

        if (renderCamera != null && entry.prefab != null)
        {
            // 카메라 뷰포트에서 중앙 좌표 설정 (0.5, 0.5)
            Vector3 randomViewportPosition = new Vector3(0.5f, 0.5f, renderCamera.nearClipPlane + 2f);
            Vector3 randomWorldPosition = renderCamera.ViewportToWorldPoint(randomViewportPosition);

            // 3D 오브젝트 생성 및 참조 저장
            currentNoonsongObject = Instantiate(entry.prefab, randomWorldPosition, Quaternion.identity);
            currentNoonsongObject.transform.position = new Vector3(0, -3f, -5f);
            currentNoonsongObject.transform.localScale = new Vector3(4f, 4f, 4f);
            currentNoonsongObject.transform.LookAt(renderCamera.transform);
        }
    }

    // Back 버튼이 눌렸을 때 호출될 메서드
    public void OnBackButtonPressed()
    {
        // CameraCanvas를 비활성화
        if (cameraCanvas != null)
        {
            cameraCanvas.gameObject.SetActive(false);
        }

        // CollectionCanvas를 활성화
        if (collectionCanvas != null)
        {
            collectionCanvas.gameObject.SetActive(true);
        }

        // 생성된 3D 오브젝트 제거
        if (currentNoonsongObject != null)
        {
            Destroy(currentNoonsongObject);
        }
    }
}
