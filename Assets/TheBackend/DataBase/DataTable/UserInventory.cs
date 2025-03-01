using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using BackEnd;

public class UserInventory
{
    public string userId;
    public string itemName;
    public int itemCount;

    public UserInventory()
    {
    }

    public UserInventory(LitJson.JsonData json)
    {
        this.userId = json["userId"].ToString();
        this.itemName = json["itemName"].ToString();
        this.itemCount = int.Parse(json["itemCount"].ToString());
    }

    public void SetUserInventory(string userId, string itemName, int itemCount)
    {
        this.userId = userId;
        this.itemName = itemName;
        this.itemCount = itemCount;
    }

    public void IncrementItemCount(int amount = 1)
    {
        this.itemCount += amount;
    }

    public void DecrementItemCount(int amount = 1)
    {
        this.itemCount = Mathf.Max(0, this.itemCount - amount);
    }

    public override string ToString()
    {
        StringBuilder result = new StringBuilder();
        result.AppendLine($"userId: {userId}");
        result.AppendLine($"itemName: {itemName}");
        result.AppendLine($"itemCount: {itemCount}");
        return result.ToString();
    }

    public Param ToParam()
    {
        Param param = new Param();
        param.Add("userId", userId);
        param.Add("itemName", itemName);
        param.Add("itemCount", itemCount);
        return param;
    }
}