using UnityEngine;
using System.Collections.Generic;

public class NoonsongFriendsEntryManager : MonoBehaviour
{
    [SerializeField]
    private List<NoonsongFriendsEntry> noonsongFriendsEntries = new List<NoonsongFriendsEntry>();

    void Start()
    {
        if (noonsongFriendsEntries == null || noonsongFriendsEntries.Count == 0)
        {
            Debug.LogError("NoonsongFriends entries are not assigned in the inspector!");
        }
        else
        {
            Debug.Log($"NoonsongFriendsEntryManager initialized with {noonsongFriendsEntries.Count} entries.");
        }
    }

    public void AddNoonsongEntry(NoonsongFriendsEntry entry)
    {
        if (entry == null)
        {
            Debug.LogError("Attempted to add a null entry.");
            return;
        }

        if (!noonsongFriendsEntries.Contains(entry))
        {
            noonsongFriendsEntries.Add(entry);
            Debug.Log($"Added to the collection: {entry.noonsongFriendName}");
        }
        else
        {
            Debug.Log($"Entry already in collection: {entry.noonsongFriendName}");
        }

        Debug.Log($"Current number of entries in the collection: {noonsongFriendsEntries.Count}");
    }

    public bool IsEntryInCollection(NoonsongFriendsEntry entry)
    {
        if (entry == null)
        {
            Debug.LogError("Attempted to check a null entry.");
            return false;
        }

        bool isInCollection = noonsongFriendsEntries.Contains(entry);
        Debug.Log($"Is entry '{entry.noonsongFriendName}' in collection? {isInCollection}");
        return isInCollection;
    }

    public NoonsongFriendsEntry[] GetNoonsongEntries()
    {
        Debug.Log($"Retrieving all entries. Total count: {noonsongFriendsEntries.Count}");
        return noonsongFriendsEntries.ToArray();
    }
}
