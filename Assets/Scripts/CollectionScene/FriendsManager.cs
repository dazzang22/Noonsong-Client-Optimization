using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class FriendsManager : MonoBehaviour
{
    public GameObject discoveredEntryPrefab;    // �߰ߵ� �׸� ������
    public GameObject undiscoveredEntryPrefab;  // �߰ߵ��� ���� �׸� ������
    public Transform entryParent;               // ���� �׸��� �θ� ������Ʈ
    public GameObject detailsPanel;             // ��Ʈ�� �� ������ ǥ���� �г�
    public TextMeshProUGUI detailsNameText;     // �� ������ �̸� �ؽ�Ʈ
    public TextMeshProUGUI detailsDescriptionText;
    public Image detailsImage;
    public Button view3DButton;                 // 3D ���� ��ư
    public Canvas collectionCanvas;             // Collection ȭ��
    public Canvas cameraCanvas;                 // 3D ���� ȭ��
    public Camera renderCamera;             // 3D �������� �������� Render Texture

    [SerializeField]
    private NoonsongFriendsEntryManager noonsongFriendsEntryManager; // NoonsongFriendsEntryManager ����

    private List<NoonsongFriendsEntry> entries; // ��� ���� �׸� ����Ʈ
    private GameObject lastClickedEntry = null; // ������ Ŭ���� �׸�
    private GameObject currentNoonsongObject;

    private bool isFriends3DViewActive = false;

    void Start()
    {
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

        if (lastClickedEntry != null && lastClickedEntry != newEntry)
        {
            var lastEntryImage = lastClickedEntry.transform.Find("NoonsongFriendImage").GetComponent<Image>();
            if (lastEntryImage != null)
            {
                var lastEntryScript = entries.Find(e => e.noonsongSprite == lastEntryImage.sprite || e.clickedNoonsongSprite == lastEntryImage.sprite);
                if (lastEntryScript != null)
                {
                    lastEntryImage.sprite = lastEntryScript.noonsongSprite;
                    lastEntryImage.rectTransform.localScale = Vector3.one; // �������� ���� ũ��� �ǵ���
                }
            }
        }

        var currentEntryImage = newEntry.transform.Find("NoonsongFriendImage").GetComponent<Image>();
        if (currentEntryImage != null)
        {
            currentEntryImage.sprite = entry.clickedNoonsongSprite;
            currentEntryImage.rectTransform.localScale = new Vector3(1.3f, 1.3f, 1.3f); // �������� 1.3��� Ű��
        }

        ShowDetails(entry);

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

                detailsImage.rectTransform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

                Debug.Log($"Details panel updated with entry: {entry.noonsongFriendName}");
                if (view3DButton != null)
                {
                    view3DButton.onClick.RemoveAllListeners();
                    view3DButton.onClick.AddListener(() =>
                    {
                        View3DButtonPressed();
                        Open3DView(entry);

                    });
                }
            }
        }
        else
        {
            Debug.LogError("DetailsPanel is not set in the inspector!");
        }
    }
    public bool View3DButtonPressed()
    {
        // 3D 뷰가 활성화되었음을 알려줌
        isFriends3DViewActive = true;
        Debug.Log("View 3D Button pressed.");
        return true;
    }

    public void Open3DView(NoonsongFriendsEntry entry)
    {
        if (collectionCanvas != null)
        {
            collectionCanvas.gameObject.SetActive(false);
        }

        if (cameraCanvas != null)
        {
            cameraCanvas.gameObject.SetActive(true);
        }

        if (renderCamera != null && entry.prefab != null)
        {
            Vector3 randomViewportPosition = new Vector3(0.5f, 0.5f, renderCamera.nearClipPlane + 2f);
            Vector3 randomWorldPosition = renderCamera.ViewportToWorldPoint(randomViewportPosition);

            currentNoonsongObject = Instantiate(entry.prefab, randomWorldPosition, Quaternion.identity);
            currentNoonsongObject.transform.position = new Vector3(0, -3f, -5f);
            currentNoonsongObject.transform.localScale = new Vector3(4f, 4f, 4f);
            currentNoonsongObject.transform.LookAt(renderCamera.transform);

            // PhotoManager 스크립트를 추가하여 오브젝트 조작 가능하게 설정
            var photoManager = currentNoonsongObject.AddComponent<PhotoManager>();
        }
    }

    public void OnBackButtonPressed()
    {
        if (cameraCanvas != null)
        {
            cameraCanvas.gameObject.SetActive(false);
        }

        if (collectionCanvas != null)
        {
            collectionCanvas.gameObject.SetActive(true);
        }

        if (currentNoonsongObject != null)
        {
            Destroy(currentNoonsongObject);
        }
        isFriends3DViewActive = false;
        Debug.Log("Back Button pressed, 3D View is now inactive.");
    }

    public bool Is3DViewActive()
    {
        return isFriends3DViewActive;
    }
}