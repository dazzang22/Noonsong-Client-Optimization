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
    public List<NoonsongEntry> entries;         // 모든 도감 항목 리스트
    public GameObject detailsPanel;             // 엔트리 상세 정보를 표시할 패널
    public TextMeshProUGUI detailsNameText;     // 상세 정보의 이름 텍스트

    void Start()
    {
        if (discoveredEntryPrefab == null || undiscoveredEntryPrefab == null)
        {
            Debug.LogError("Entry prefabs are not set in the inspector!");
        }
        if (entryParent == null)
        {
            Debug.LogError("entryParent is not set in the inspector!");
        }
        if (entries == null)
        {
            Debug.LogError("entries list is not set in the inspector!");
        }
        else if (entries.Count == 0)
        {
            Debug.LogError("entries list is empty!");
        }

        if (detailsPanel == null || detailsNameText == null)
        {
            Debug.LogError("DetailsPanel or DetailsNameText is not set in the inspector!");
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
                AddEventTriggerListener(eventTrigger, EventTriggerType.PointerClick, () => ShowDetails(entry));

                var nameText = newEntry.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
                if (nameText != null) nameText.text = entry.noonsongName;

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

    void ShowDetails(NoonsongEntry entry)
    {
        Debug.Log($"ShowDetails called for entry: {entry.noonsongName}");

        if (detailsPanel != null)
        {
            detailsPanel.SetActive(true);
            if (detailsNameText != null)
            {
                detailsNameText.text = entry.noonsongName;
                Debug.Log($"Details panel updated with entry: {entry.noonsongName}");
            }
            else
            {
                Debug.LogError("DetailsNameText is not set in the inspector!");
            }
        }
        else
        {
            Debug.LogError("DetailsPanel is not set in the inspector!");
        }
    }
}

