using UnityEngine;

[System.Serializable]
public class SpawnedObject
{
    public GameObject GameObject { get; private set; }
    public NoonsongEntry NoonsongEntry { get; private set; }

    public SpawnedObject(GameObject gameObject, NoonsongEntry noonsongEntry)
    {
        GameObject = gameObject;
        NoonsongEntry = noonsongEntry;
    }
}
