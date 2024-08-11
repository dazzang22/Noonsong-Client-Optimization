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
}


