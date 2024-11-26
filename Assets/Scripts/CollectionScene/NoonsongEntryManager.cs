using UnityEngine;
using System.Collections.Generic;

public class NoonsongEntryManager : MonoBehaviour
{
    [SerializeField]
    private List<NoonsongEntry> noonsongEntries = new List<NoonsongEntry>();

    void Start()
    {
        if (noonsongEntries == null || noonsongEntries.Count == 0)
        {
            Debug.LogError("Noonsong entries are not assigned in the inspector!");
        }
        else
        {
            Debug.Log($"NoonsongEntryManager initialized with {noonsongEntries.Count} entries.");
        }
    }

    public void AddNoonsongEntry(NoonsongEntry entry)
    {
        if (entry == null)
        {
            Debug.LogError("Attempted to add a null entry.");
            return;
        }

        if (!noonsongEntries.Contains(entry))
        {
            noonsongEntries.Add(entry);
            Debug.Log($"Added to the collection: {entry.noonsongName}");
        }
        else
        {
            Debug.Log($"Entry already in collection: {entry.noonsongName}");
        }

        Debug.Log($"Current number of entries in the collection: {noonsongEntries.Count}");
    }

    public bool IsEntryInCollection(NoonsongEntry entry)
    {
        if (entry == null)
        {
            Debug.LogError("Attempted to check a null entry.");
            return false;
        }

        bool isInCollection = noonsongEntries.Contains(entry);
        Debug.Log($"Is entry '{entry.noonsongName}' in collection? {isInCollection}");
        return isInCollection;
    }

    public NoonsongEntry[] GetNoonsongEntries()
    {
        Debug.Log($"Retrieving all entries. Total count: {noonsongEntries.Count}");
        return noonsongEntries.ToArray();
    }
    
}



