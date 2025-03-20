using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using System;

public class GameStatistics
{
    private static GameStatistics _instance;

    public static GameStatistics Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameStatistics();
            }
            return _instance;
        }
    }

    private GameStatistics() { }

    public void AverageSnowCount()
    {
        var bro = Backend.GameData.Get("UserBalance", new Where(), 100);

        if (!bro.IsSuccess())
        {
            Debug.LogError($"[GameStatistics] 유저 재화 데이터 가져오기 실패: {bro}");
            return;
        }

        int totalSnowCount = 0;
        int userCount = bro.FlattenRows().Count;

        foreach (LitJson.JsonData data in bro.FlattenRows())
        {
            totalSnowCount += int.Parse(data["snowCount"].ToString());
        }

        float averageSnowCount = (float)totalSnowCount / userCount;
        Debug.Log($"[GameStatistics] 전체 유저 평균 재화량: {averageSnowCount}");
    }

    //전체 유저 불러오기 & 유저 도감 지표 업데이트트
    public void setUserStatic()
    {
        string[] select = { "userId" };
        var bro = Backend.GameData.Get("UserProfile",new Where(),select, 100);

        if (!bro.IsSuccess())
        {
            Debug.LogError($"[GameStatistics] 유저 프로필 데이터 가져오기 실패: {bro}");
            return;
        }
         var rows = bro.FlattenRows();
    if (rows == null)
    {
        Debug.LogWarning("[Backend] 유저 데이터가 존재하지 않습니다.");
        return;
    }
         int userCount = rows.Count;
        
        List<string> users = new List<string>();

        foreach (LitJson.JsonData data in bro.FlattenRows())
        {
            string userID =  data["userId"].ToString();            
            users.Add(userID);
        }
        Debug.Log($"[GameStatistics] 전체 유저 수: {userCount}");

        Dictionary<string,List<int>> udlist = new Dictionary<string,List<int>>(getUDList());
        foreach(string user in users)
        {
            foreach(var ud in udlist)
            {
                if(user == ud.Key)
                {
                    Debug.Log($"{ud.Value.Count}");
                    UserStatisticsManager.Instance.insertUserStatistics(user,ud.Value.Count, ud.Value);
                }
            }
        }

    }

    //특정 유저의 친구 리스트 가져오기
    public Dictionary<string,List<int>> getUDList()
    {
        string[] select = { "userId","noonsongId" };
        var bro = Backend.GameData.Get("UserDogam", new Where(), select, 100);
        //var bro = Backend.GameData.GetV2("UserDogam", userIndate, owner_inDate,select);
        if (!bro.IsSuccess())
        {
            Debug.LogError($"[GameStatistics] 유저 도감 데이터 가져오기 실패: {bro}");
            return null;
        }
        LitJson.JsonData userdogamjson = bro.FlattenRows();
        Dictionary<string,List<int>> dogamList = new Dictionary<string,List<int>>();

        if(userdogamjson.Count<=0)
        {
            Debug.LogWarning("데이터가 존재하지 않습니다.");
            return null;
        }
        else
        {
            foreach(LitJson.JsonData udjson in userdogamjson)
            {
                int n= int.Parse(udjson["noonsongId"].ToString());
                string uid=udjson["userId"].ToString();
                Debug.Log($"{udjson["userId"].ToString()}:{n}");
                if(dogamList.ContainsKey(uid))
                {
                    dogamList[uid].Add(n);
                }
                else
                {
                    dogamList[uid]=new List<int>{n};
                }
                // if(us.getFriend())
                // {
                //     //addFriendList(us);
                //     //userfriendList.Add(us);
                // }
            }
        }
        return dogamList;
    }

    //눈송이별 지표
    public void UpdateNoonStatics()
    {
        // for(int i = 8 ; i<60 ;i++)
        // {
        //     string noons = DogamChartManager.Instance.getNoonsongName(i);
        //     NoonsongStatisticsManager.Instance.reset(noons);
        // }
        Dictionary<string,List<int>> udlist = new Dictionary<string,List<int>>(getUDList());
        Dictionary<int,int> noonlist = new Dictionary<int,int>();
        foreach(var e in udlist)
        {
            foreach(int n in e.Value)
            {
                if(noonlist.ContainsKey(n))
                {
                    noonlist[n]+=1;
                }
                else
                {
                    noonlist.Add(n,1);
                }
            }
        }
        foreach(var n in noonlist)
        {
            string noonName =  DogamChartManager.Instance.getNoonsongName(n.Key);
            if(n.Value!=NoonsongStatisticsManager.Instance.getNoons(noonName))
            {
                NoonsongStatisticsManager.Instance.insertNoonsongStatistic(noonName,n.Value);
            }
        }
    }
}
