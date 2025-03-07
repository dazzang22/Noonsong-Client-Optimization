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
        var bro = Backend.Chart.GetChartContents("168146");
        foreach(LitJson.JsonData gameData in bro.FlattenRows())
        {
            if(noonsoong == gameData["SnowflakeName"].ToString()){
                return int.Parse(gameData["SnowflakeId"].ToString());
            }
        }
        return -1;

    }
        //눈송이 이름으로 눈송이 차트에서 눈송이이ID 찾기.
    public string getNoonsongName(int id)
    {
        var bro = Backend.Chart.GetChartContents("168146");
        foreach(LitJson.JsonData gameData in bro.FlattenRows())
        {
            if(id == int.Parse(gameData["SnowflakeId"].ToString())){
                return gameData["SnowflakeName"].ToString();
            }
        }
        return null;

    }
    //눈송이 아이로 대학이름 찾기
    public string getNoonsongCollege(int id)
    {
        var bro = Backend.Chart.GetChartContents("168146");
        int cid=0;
        foreach(LitJson.JsonData gameData in bro.FlattenRows())
        {
            if(id == int.Parse(gameData["SnowflakeId"].ToString()))
            {
                cid= int.Parse(gameData["CollegeId"].ToString());
                return getCollegeName(cid);
            }
        }
        return null;

    }
    public int collegeID(string college)
    {
        return getCollegeId(getCollegeChart(college));
    }
    public int crystalValue(string college)
    {
        return getCrystalValue(getCollegeChart(college));
    }
    public int friendFavor(string college)
    {
        return getFriendFavor(getCollegeChart(college));
    }
    public int maxFavor(string college)
    {
        return getMaxFavor(getCollegeChart(college));
    }

    //대학 이름으로 대학 차트 json으로 가져오기.
    public LitJson.JsonData getCollegeChart(string college)
    {
        var collegechart = Backend.Chart.GetChartContents("168109"); 

        LitJson.JsonData collegeChart;
        foreach(LitJson.JsonData gameData in collegechart.FlattenRows())
        {
            if(college == gameData["CollegeName"].ToString()){
                collegeChart = gameData;
                return collegeChart;
            }
        }
        return null;
    }
    public string getCollegeName(int college)
    {
        var collegechart = Backend.Chart.GetChartContents("168109"); 

        LitJson.JsonData collegeChart;
        foreach(LitJson.JsonData gameData in collegechart.FlattenRows())
        {
            if(college == int.Parse(gameData["CollegeId"].ToString()))
            {
                return gameData["collegeName"].ToString();
            }
        }
        return null;
    }

    //대학 이름으로 대학 차트에서 대학id 가져오기.
    public int getCollegeId(LitJson.JsonData college)
    {
        return int.Parse(college["collegeId"].ToString());
    }

    //대학 차트에서 대학별 결정 수치 가져오기
    public int getCrystalValue(LitJson.JsonData college)
    {
        return int.Parse(college["crystalValue"].ToString());
    }

    //대학 차트에서 대학별 도감 등록 친밀도 수치 가져오기
    public int getFriendFavor(LitJson.JsonData college)
    {
        return int.Parse(college["friend_favorability"].ToString());
    }

    //대학 차트에서 대학별 최대 친밀도 수치 가져오기
    public int getMaxFavor(LitJson.JsonData college)
    {
        return int.Parse(college["max_favorability"].ToString());
    }

}