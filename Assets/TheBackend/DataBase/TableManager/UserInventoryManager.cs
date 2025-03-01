/*using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using BackEnd;


public class UserInventoryManager
{
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

    // 인벤토리 저장소
    private Dictionary<string, int> inventory = new Dictionary<string, int>();

    // 인벤토리 불러오기
    public void LoadInventory(string userId)
    {
        var query = new Where();
        query.Equal("userId", userId);
        var result = Backend.GameData.GetMyData("Inventory", query, 100);

        if (result.IsSuccess())
        {
            inventory.Clear();
            var items = result.GetReturnValuetoJSON()["rows"];

            foreach (var item in items)
            {
                string itemName = item["itemName"].ToString();
                int itemCount = int.Parse(item["itemCount"].ToString());

                if (!inventory.ContainsKey(itemName))
                {
                    inventory.Add(itemName, itemCount);
                }
                else
                {
                    inventory[itemName] = itemCount;
                }
            }
            Debug.Log("인벤토리 불러오기 성공");
        }
        else
        {
            Debug.LogError("인벤토리 불러오기 실패: " + result.GetMessage());
        }
    }

    // 아이템 추가 및 업데이트
    public void SaveItem(string userId, string itemName, int count)
    {
        if (inventory.ContainsKey(itemName))
        {
            inventory[itemName] += count;
        }
        else
        {
            inventory.Add(itemName, count);
        }

        var query = new Where();
        query.Equal("userId", userId);
        query.Equal("itemName", itemName);

        var result = Backend.GameData.Get("Inventory", query);

        if (result.IsSuccess())
        {
            var rows = result.GetReturnValuetoJSON()["rows"];
            if (rows.Count > 0)
            {
                var inDate = rows[0]["inDate"].ToString();
                Param param = new Param();
                param.Add("itemCount", inventory[itemName]);
                Backend.GameData.Update("Inventory", inDate, param);
            }
            else
            {
                UserInventory newItem = new UserInventory();
                newItem.SetUserInventory(userId, itemName, count);
                Backend.GameData.Insert("Inventory", newItem.ToParam());
            }
        }
    }

    // 아이템 사용
    public void UseItem(string itemName, int count = 1)
    {
        if (inventory.ContainsKey(itemName) && inventory[itemName] >= count)
        {
            inventory[itemName] -= count;
            if (inventory[itemName] <= 0)
            {
                inventory.Remove(itemName);
            }
            Debug.Log($"{itemName} 사용! 남은 개수: {inventory.GetValueOrDefault(itemName, 0)}");
        }
        else
        {
            Debug.LogError($"{itemName}의 개수가 부족합니다.");
        }
    }

    // 인벤토리 출력 (디버깅용)
    public void PrintInventory()
    {
        Debug.Log("===== 인벤토리 목록 =====");
        foreach (var item in inventory)
        {
            Debug.Log($"아이템: {item.Key}, 개수: {item.Value}");
        }
    }
}*/