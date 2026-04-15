using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;

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
        syncwithEntryDB();
    }
    void Start()
    {
        if (noonsongEntries == null || noonsongEntries.Count == 0)
        {
            Debug.LogError("Noonsong entries are not assigned in the inspector!");
        }
        else
        {
            Debug.Log("AR Session active: " + (FindObjectOfType<ARSession>() != null));
            Debug.Log("XR Origin active: " + (FindObjectOfType<XROrigin>() != null));
            Debug.Log("AR Camera Background active: " + (FindObjectOfType<ARCameraBackground>() != null));

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
            int num=0;
            if(entry.isBestFriend ){    num= 5;       }
            if(!entry.isFriend ){    num= entry.loveLevel/10;       }
            if(entry.isFriend && !entry.isBestFriend){num=entry.loveLevel/10-5;}
            UserDogamManager.Instance.noonsongInsert(entry.noonsongName,num,entry.loveLevel,entry.isFriend,entry.isBestFriend);
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

    public List<NoonsongEntry> GetNoonsongEntries()
    {
        Debug.Log($"Retrieving all entries. Total count: {noonsongEntries.Count}");
        return new List<NoonsongEntry>(noonsongEntries); // 반환 시 복사본을 만들어서 반환
    }

    //db랑 noonsongentry 싱크 맞추기
    public void syncwithEntryDB()
    {
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
            Debug.Log("UserDogam DB와 싱크 완료");
        }
        else
        {
            Debug.Log("도감 비어있음");

        }
    }
    
}



