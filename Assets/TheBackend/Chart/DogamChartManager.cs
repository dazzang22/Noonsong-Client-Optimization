using System.Collections.Generic;
using System.Text;
using UnityEngine;

// 뒤끝 SDK namespace 추가
using BackEnd;

public class DogamChartManager {
    private static DogamChartManager _instance = null;

    public static DogamChartManager Instance {
        get {
            if(_instance == null) {
                _instance = new DogamChartManager();
            }

            return _instance;
        }
    }

    //눈송이 이름으로 눈송이 차트에서 눈송이이ID 찾기.
    public int getSnowflakeId(string noonsoong)
    {
        var bro = Backend.Chart.GetChartContents("157725");
        foreach(LitJson.JsonData gameData in bro.FlattenRows())
        {
            if(noonsoong == gameData["SnowflakeName"].ToString()){
                return int.Parse(gameData["SnowflakeId"].ToString());
            }
        }
        return -1;

    }

    //대학 이름으로 대학 차트에서 대학id 가져오기.
    public int getCollegeId(string college)
    {
        var collegechart = Backend.Chart.GetChartContents("157731"); 

        int collegeid=0;
        foreach(LitJson.JsonData gameData in collegechart.FlattenRows())
        {
            if(college == gameData["CollegeName"].ToString()){
                return collegeid = int.Parse(gameData["CollegeId"].ToString());
            }
        }
        return -1;
    }
}