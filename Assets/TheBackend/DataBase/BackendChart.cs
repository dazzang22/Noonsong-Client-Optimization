using System.Collections.Generic;
using System.Text;
using UnityEngine;

// 뒤끝 SDK namespace 추가
using BackEnd;

public class BackendChart {
    private static BackendChart _instance = null;

    public static BackendChart Instance {
        get {
            if(_instance == null) {
                _instance = new BackendChart();
            }

            return _instance;
        }
    }

    public void ChartGet(string chartId) {
        // Step 3. 차트 정보 가져오기 내용 추가
        Debug.Log($"{chartId}의 차트 불러오기를 요청합니다.");
        var bro = Backend.Chart.GetChartContents(chartId);

        if(bro.IsSuccess() == false) {
            Debug.LogError($"{chartId}의 차트를 불러오는 중, 에러가 발생했습니다. : " + bro);
            return;
        }

        Debug.Log("차트 불러오기에 성공했습니다. : " + bro);
        foreach(LitJson.JsonData gameData in bro.FlattenRows()) {
            StringBuilder content = new StringBuilder();
            content.AppendLine("itemID : " + int.Parse(gameData["SnowflakeId"].ToString()));
            content.AppendLine("itemName : " + gameData["SnowflakeName"].ToString());
            content.AppendLine("itemType : " + gameData["SnowflakeType"].ToString());
            content.AppendLine("itemInfo : " + gameData["SnowflakeInfo"].ToString());

            Debug.Log(content.ToString());
        }
    }
    public int FindSnowflakeId(string noonsoong)
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

    public void getCollegeNoonsong(string college)
    {
        var collegechart = Backend.Chart.GetChartContents("157731"); 
        var noongsongchart = Backend.Chart.GetChartContents("157725"); 

        int collegeid=0;
        foreach(LitJson.JsonData gameData in collegechart.FlattenRows())
        {
            if(college == gameData["CollegeName"].ToString()){
                collegeid = int.Parse(gameData["CollegeId"].ToString());
                break;
            }
        }
        //var collegenoonsong = Backend.Chart


    }
}