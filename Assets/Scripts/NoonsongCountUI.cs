using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class NoonsongCountUI : MonoBehaviour
{
    public TextMeshProUGUI friendCountText;
    private NoonsongEntry[] noonsongEntries;

    private void Start()
    {
        LoadNoonsongEntries();
        UpdateFriendCount();
    }

    private void LoadNoonsongEntries()
    {
        noonsongEntries = Resources.LoadAll<NoonsongEntry>("MajorNoonsong");

        if (noonsongEntries == null || noonsongEntries.Length == 0)
        {
            Debug.LogWarning($"NoonsongEntry를 찾을 수 없습니다.");
        }
    }

    public void UpdateFriendCount()
    {
        if (noonsongEntries == null)
            return;

        int friendCount = noonsongEntries.Count(entry => entry.isFriend);
        friendCountText.text = $"{friendCount}";
    }
}
