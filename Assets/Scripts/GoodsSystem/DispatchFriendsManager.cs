using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DispatchFriendsManager : MonoBehaviour
{
   [SerializeField]
    private NoonsongFriendsEntryManager noonsongFriendsEntryManager;

    [Header("UI Elements")]
    public GameObject noonsongItemPrefab; // ScrollView의 개별 항목 버튼 프리팹
    public Transform contentParent; // ScrollView의 Content 오브젝트
    public GameObject noonsInfoPanel; // 선택된 눈송이를 표시하는 UI 패널
    public Image noonsongImage; // 눈송이 이미지
    public  TextMeshProUGUI noonsongNameText; // 눈송이 이름
    public  TextMeshProUGUI noonsongDescriptionText; // 눈송이 설명
    private List<NoonsongFriendsEntry> entries = new List<NoonsongFriendsEntry>();


    private void Start()
    {
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
                button.onClick.AddListener(() => DisplayNoonsongInfo(entry));
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

        noonsongImage.sprite = noonsongEntry.noonsongSprite;
        noonsongNameText.text = noonsongEntry.noonsongFriendName;
        noonsongDescriptionText.text = noonsongEntry.description;

        noonsInfoPanel.SetActive(true);
    }
}
