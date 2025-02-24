using System.Collections.Generic;
using System.Text;
using UnityEngine;

// 뒤끝 SDK namespace 추가
using BackEnd;

public class MapChartManager 
{
    private static MapChartManager _instance = null;

    public static MapChartManager Instance {
        get {
            if(_instance == null) {
                _instance = new MapChartManager();
            }

            return _instance;
        }
    }

    //지도 이름으로 지도 id 찾기
    public int getMapId(string location)
    {
        var bro = Backend.Chart.GetChartContents("162815");
        foreach(LitJson.JsonData gameData in bro.FlattenRows())
        {
            if(location == gameData["Name"].ToString()){
                return int.Parse(gameData["MapId"].ToString());
            }
        }
        return -1;

    }
    //지도 id로 지도이름 찾기
    public string getMapName(int location)
    {
        var bro = Backend.Chart.GetChartContents("162815");
        foreach(LitJson.JsonData gameData in bro.FlattenRows())
        {
            if(location == int.Parse(gameData["MapId"].ToString())){
                return gameData["Name"].ToString();
            }
        }
        return null;

    }
}