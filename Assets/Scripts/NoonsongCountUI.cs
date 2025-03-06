using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class NoonsongCountUI : MonoBehaviour
{
    public TextMeshProUGUI friendCountText;
    public TextMeshProUGUI accountfriendCountText;
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
            Debug.LogWarning($"NoonsongEntry�� ã�� �� �����ϴ�.");
        }
    }

    public void UpdateFriendCount()
    {
        if (noonsongEntries == null)
            return;

        //int friendCount = noonsongEntries.Count(entry => entry.isFriend);
        int friendCount= UserDogamManager.Instance.getDogamNum();
        friendCountText.text = $"{friendCount}";
        accountfriendCountText.text = $"{friendCount}";
    }
}
