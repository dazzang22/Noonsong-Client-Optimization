using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using BackEnd;

public class UserStatistics
{
    public string userId;
    public int num=0;
    public List<int> friendsList = new List<int>();

    //public bool
    public UserStatistics()
    {

    }

    public UserStatistics(LitJson.JsonData json)
    {
        this.userId=json["userId"].ToString();
        this.num=int.Parse(json["num"].ToString());

        for(int i = 0; i < json["friendsList"].Count; i++)
        {
            this.friendsList.Add(int.Parse(json["friendsList"][i].ToString()));
        }
        //lastUpdate = DateTime.Parse(json["lastUpdate"].ToString());
    }
    public void setUserStatistics(string userId,int num, List<int> friendsList=null)
    {
        this.userId=userId;
        this.num=num;
        if(friendsList != null)
        {
            for(int i = 0; i < friendsList.Count; i++)
            {
                this.friendsList.Add(friendsList[i]);
            }
        }
    }
    //validate 된 구역 업데이트
    public void updatefriendsList(int num, List<int> friendsList=null)
    {
        this.num= num;
        if(friendsList != null)
        {
            this.friendsList.Clear();
            for(int i = 0; i < friendsList.Count; i++)
            {
                this.friendsList.Add(friendsList[i]);
            }
        }
    }

    //validate 된 구역 추가
    public void addfriendsList(int friendsList)
    {
        this.friendsList.Add(friendsList);
    }

    //validate 된 maplist 반환
    public List<int> getUserfriendsList()
    {
        return this.friendsList;
    }

    public override string ToString()
    {
        StringBuilder result = new StringBuilder();
        result.AppendLine($"userId: {userId}");
        result.AppendLine($"num: {num}");
        foreach(var fl in friendsList)
        {
            result.AppendLine($"friendsList: {fl}");

        }
        return result.ToString();
    }

    public Param ToParam()
    {
        Param param = new Param();

        param.Add("userId",userId);
        param.Add("num",num);
        param.Add("friendsList",friendsList);

        return param;
    }
}

  