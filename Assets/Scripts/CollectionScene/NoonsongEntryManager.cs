using UnityEngine;
using System.Collections.Generic;

public class NoonsongEntryManager : MonoBehaviour
{
    [SerializeField]
    private List<NoonsongEntry> noonsongEntries = new List<NoonsongEntry>();

    //DB 정보 불러와서 유니티 noonsongEntries에 연결
    void Awake()
    {
        foreach(NoonsongEntry noon in noonsongEntries)
        {
            if(noon.isDiscovered==true)
            {
                noon.isDiscovered=false;
                noon.isBestFriend=false;
                noon.isFriend=false;
                noon.loveLevel=0;
            }
        }
        List<UserDogam> userdogamlist= UserDogamManager.Instance.getUserDogamList();
        if(userdogamlist!=null)
        {
            foreach(UserDogam ud in userdogamlist)
            {
                string name= DogamChartManager.Instance.getNoonsongName(ud.noonsongId);

                foreach(NoonsongEntry noon in noonsongEntries)
                {
                    if(noon.noonsongName == name)
                    {
                        noon.isDiscovered=true;
                        noon.isBestFriend=ud.max;
                        noon.isFriend=ud.getFriend();
                        noon.loveLevel=ud.getFavor();
                        Debug.Log($"눈송이엔트리 업데이트:{noon}");
                    }
                }
            }
        }
        else
        {
            Debug.Log("도감 비어있음");

        }
    }
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
            //DB 에 추가
            Debug.Log("디비 추가");
            UserDogamManager.Instance.noonsongInsert(entry.noonsongName,entry.loveLevel,entry.university);
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



