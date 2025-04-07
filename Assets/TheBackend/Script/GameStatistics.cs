using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using System;
using System.Linq;

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
    public List<string> getUserList()
    {
        string[] select = { "userId" };
        var bro = Backend.GameData.Get("UserProfile",new Where(),select, 100);

        if (!bro.IsSuccess())
        {
            Debug.LogError($"[GameStatistics] 유저 프로필 데이터 가져오기 실패: {bro}");
            return null;
        }
         var rows = bro.FlattenRows();
        if (rows == null)
        {
            Debug.LogWarning("[Backend] 유저 데이터가 존재하지 않습니다.");
            return null;
        }
        int userCount = rows.Count;
        
        List<string> users = new List<string>();
        Debug.Log($"[GameStatistics] 전체 유저 수: {userCount}");
        int i=1;
        foreach (LitJson.JsonData data in bro.FlattenRows())
        {
            string userID =  data["userId"].ToString();            
            users.Add(userID);
            Debug.Log($"{i++}:{userID}");
        }
        return users;
    

    }

    //특정 유저의 친구 리스트 가져오기
    public Dictionary<string,List<Dictionary<string,int>>> getUDList()
    {
        Dictionary<string,List<Dictionary<string,int>>> dogamList = new Dictionary<string,List<Dictionary<string,int>>>();

        List<string> users = new List<string>(getUserList());
        //유저마다
        Debug.Log("[GameStatistics] 전체 유저별 눈송이 지표");

        foreach(string user in users)
        {
            Where where = new Where();
            where.Equal("userId",user);
            string[] select = { "userId","noonsongId","favor" };
            var bro = Backend.GameData.Get("UserDogam", where, select, 100);
            if (!bro.IsSuccess())
            {
                Debug.LogError($"[GameStatistics] 유저 도감 데이터 가져오기 실패: {bro}");
                return null;
            }
            LitJson.JsonData userdogamjson = bro.FlattenRows();
            Debug.Log($"유저 {user} : {userdogamjson.Count} 개 수집");
            //유저 별 도감 리스트
            dogamList[user]= new List<Dictionary<string, int>>();
            Dictionary<string, int> udlist = new Dictionary<string, int>();
            if(userdogamjson.Count<=0)
            {
                //Debug.Log("데이터가 존재하지 않습니다.");
                dogamList[user].Add(udlist);
                continue;
            }
            foreach(LitJson.JsonData udjson in userdogamjson)
            {
                int n= int.Parse(udjson["noonsongId"].ToString());
                int favor= int.Parse(udjson["favor"].ToString());
                string name= DogamChartManager.Instance.getNoonsongName(n);
                if(!dogamList.ContainsKey(user))
                {
                    dogamList[user]= new List<Dictionary<string, int>>();
                }
                udlist[name]=favor;
            }
            dogamList[user].Add(udlist);
            int i =1;
            foreach (var ud in udlist)
            {
                Debug.Log($"{i++}: {ud.Key} : {ud.Value}");
            }

        }

            
        return dogamList;
    }

    //눈송이별 지표
    public void UpdateNoonStatics()
    {
        Dictionary<string,List<Dictionary<string,int>>> udlist = new Dictionary<string,List<Dictionary<string,int>>>(getUDList());
        Dictionary<string,int> noonlist = new Dictionary<string,int>();
        for(int i = 8 ; i<60 ;i++)
        {
            string noons = DogamChartManager.Instance.getNoonsongName(i);
            noonlist[noons]=0;
        }
        foreach(var list in udlist)
        {
            foreach(var dict in list.Value)
            {
                int i =0;
                foreach(var n in dict)
                {
                    if(noonlist.ContainsKey(n.Key))
                    {
                        noonlist[n.Key]+=1;
                    }
                    else
                    {
                        noonlist.Add(n.Key,1);
                    }
                }
            }
        }
        Debug.Log($"[GameStatistics] 전체 눈송이 지표 : 전체 {noonlist.Count(kv => kv.Value >= 1)}개 수집됨");
        foreach(var n in noonlist)
        {
            Debug.Log($"{n.Key} : {n.Value} 개 수집됨");
        }
    }
}
