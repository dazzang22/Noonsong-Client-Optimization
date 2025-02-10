using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookManager : MonoBehaviour
{
    [SerializeField] private List<NoonsongEntry> allNoonsongs;

    public List<NoonsongEntry> GetDiscoveredNoonsongs()
    {
        return allNoonsongs.FindAll(noonsong => noonsong.isDiscovered);
    }
}