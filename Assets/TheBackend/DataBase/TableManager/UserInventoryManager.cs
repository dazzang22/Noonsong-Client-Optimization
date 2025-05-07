using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using LitJson;

public class UserInventoryManager
{
    public List<UserInventoryEntry> userInventoryEntries = new List<UserInventoryEntry>();

    private static UserInventoryManager _instance = null;

    public static UserInventoryManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UserInventoryManager();
            }

            return _instance;
        }
    }
    private string gameDataRowInDate = string.Empty;

    // 유저 인벤토리 불러오기 (뒤끝 DB에서 UserInventory 테이블 조회)
    public void LoadUserInventory(string userId)
    {
        Where where = new Where();
        where.Equal("userId", userId);
        var bro = Backend.GameData.Get("UserInventory", where, 100);
        if (!bro.IsSuccess())
        {
            Debug.LogError($"유저 인벤토리 로드 실패: {bro}");
            return;
        }

        userInventoryEntries.Clear();
        foreach (JsonData row in bro.FlattenRows())
        {
            int itemId = int.Parse(row["itemId"].ToString());
            int itemCount = int.Parse(row["itemCount"].ToString());

            
            UserInventoryEntry userItem = new UserInventoryEntry(userId, itemId, itemCount);
            userInventoryEntries.Add(userItem);
        }

        Debug.Log("유저 인벤토리 로드 완료");
    }

    public void InsertUserInventory(string userId, int itemId, int itemCount = 1)
    {
        //이미 인벤토리에 존재하는지 확인
        Where where = new Where();
        where.Equal("userId", userId);
        where.Equal("itemId", itemId);
        var bro1 = Backend.GameData.Get("UserInventory", where, 1);

        if (bro1.IsSuccess() && bro1.FlattenRows().Count > 0)
        {
            Debug.Log($"아이템({itemId})이 이미 존재합니다.");
            return;
        }

        //새로운 아이템 추가
        UserInventoryEntry newItem = new UserInventoryEntry(userId, itemId, itemCount);
        userInventoryEntries.Add(newItem);

        //DB에 추가
        Param param = new Param();
        param.Add("userId", userId);
        param.Add("itemId", itemId);
        param.Add("itemCount", itemCount);

        var bro2 = Backend.GameData.Insert("UserInventory", param);
        if (bro2.IsSuccess())
        {
            Debug.Log($"아이템 추가 성공: {itemId} x{itemCount}");
        }
        else
        {
            Debug.LogError($"아이템 추가 실패: {bro2}");
        }
    }

    //아이템 개수 업데이트
    public void UpdateItemCount(string userId, int itemId, int newCount)
    {
        UserInventoryEntry entry = userInventoryEntries.Find(e => e.itemId == itemId);
        if (entry != null)
        {
            if (newCount <= 0)
            {
                // 아이템 수량이 0 이하이면 인벤토리에서 제거
                userInventoryEntries.Remove(entry);

                // DB에서 아이템 삭제
                Where where = new Where();
                where.Equal("userId", userId);
                where.Equal("itemId", itemId);
                var bro = Backend.GameData.Delete("UserInventory", where);
                if (!bro.IsSuccess())
                {
                    Debug.LogError($"아이템 삭제 실패: {bro}");
                }
            }
            else
            {
                // 아이템 수량 업데이트
                entry.itemCount = newCount;

                // DB에서 해당 아이템의 inDate 조회
                Where where = new Where();
                where.Equal("userId", userId);
                where.Equal("itemId", itemId);
                var bro1 = Backend.GameData.Get("UserInventory", where, 1);
                if (!bro1.IsSuccess() || bro1.FlattenRows().Count == 0)
                {
                    Debug.LogError($"유저 인벤토리 조회 실패 (inDate 찾을 수 없음): {bro1}");
                    return;
                }

                string inDate = bro1.FlattenRows()[0]["inDate"].ToString();

                // DB 업데이트
                Param param = new Param();
                param.Add("itemCount", newCount);
                var bro2 = Backend.GameData.UpdateV2("UserInventory", inDate, Backend.UserInDate, param);
                if (!bro2.IsSuccess())
                {
                    Debug.LogError($"아이템 개수 업데이트 실패: {bro2}");
                }
            }
        }
        else
        {
            Debug.LogError($"업데이트할 아이템을 찾을 수 없음: {itemId}");
        }
    }



    //아이템 삭제
    public void RemoveItem(string userId, int itemId)
    {
        UserInventoryEntry entry = userInventoryEntries.Find(e => e.itemId == itemId);
        if (entry != null)
        {
            userInventoryEntries.Remove(entry);

            //뒤끝 DB에서 삭제
            Where where = new Where();
            where.Equal("userId", userId);
            where.Equal("itemId", itemId);

            var bro = Backend.GameData.Delete("UserInventory", where);
            if (bro.IsSuccess())
            {
                Debug.Log($"아이템 삭제 성공: {itemId}");
            }
            else
            {
                Debug.LogError($"아이템 삭제 실패: {bro}");
            }
        }
    }

    //아이템 제거 (인자 itemID)
    public void ConsumeItem(int itemID)
    {
        string userId = UserDataManager.Instance.getUserID();

        UserInventoryEntry existingItem = UserInventoryManager.Instance.userInventoryEntries
            .Find(e => e.itemId == itemID);

        if (existingItem == null)
        {
            Debug.LogWarning($"ConsumeItem: 유저 인벤토리에서 itemID {itemID}를 찾을 수 없습니다.");
            return;
        }

        int newCount = existingItem.itemCount - 1;
        if (newCount < 0) newCount = 0;

        UserInventoryManager.Instance.UpdateItemCount(userId, itemID, newCount);

        Debug.Log($"ConsumeItem: itemID {itemID} 사용됨. 남은 수량: {newCount}");

        UserInventoryManager.Instance.ReloadInventory();
    }


    public void ReloadInventory()
    {
        string userId = UserDataManager.Instance.getUserID();
        Debug.Log($" 인벤토리 새로고침: userId={userId}");

        InventoryManager inventoryManager = GameObject.FindObjectOfType<InventoryManager>();
        GiftInventory giftInventory = GameObject.FindObjectOfType<GiftInventory>();
        if (inventoryManager != null)
        {
            inventoryManager.UpdateInventory();
            if (giftInventory != null)
            {
                giftInventory.UpdateGiftInventoryUI();
            }
            Debug.Log("인벤토리 UI 업데이트 완료");
        }
        else
        {
            Debug.LogWarning("InventoryManager를 찾을 수 없습니다.");
        }
    }

    //유저 인벤토리 데이터 출력
    public void PrintInventory()
    {
        foreach (var item in userInventoryEntries)
        {
            Debug.Log($"아이템 ID: {item.itemId}, 개수: {item.itemCount}");
        }
    }
}
