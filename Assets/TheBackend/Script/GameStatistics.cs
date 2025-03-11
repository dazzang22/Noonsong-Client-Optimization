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
}
