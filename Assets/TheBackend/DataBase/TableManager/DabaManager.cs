using System.Collections.Generic;
using System.Text;
using UnityEngine;

// 뒤끝 SDK namespace 추가
using BackEnd;

public class GameDataManager : MonoBehaviour
{
    private static GameDataManager _instance = null;

    public static GameDataManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new GameDataManager();
            }

            return _instance;
        }
    }
    
    //전체 테이블 리스트 조회회
    public void GetTableList()
    {
        var bro = Backend.GameData.GetTableList();

        if(!bro.IsSuccess())
        {
            Debug.LogError(bro.ToString());
            return;
        }

        List<TableItem> tableList = new List<TableItem>();
        LitJson.JsonData tableListJson = bro.GetReturnValuetoJSON()["tables"];

        for(int i = 0; i < tableListJson.Count; i++)
        {
            TableItem tableItem = new TableItem();

            tableItem.tableName = tableListJson[i]["tableName"].ToString();
            tableItem.tableExplaination = tableListJson[i]["tableExplaination"].ToString();
            tableItem.isChecked = tableListJson[i]["isChecked"].ToString() == "true" ? true : false;
            tableItem.hasSchema = tableListJson[i]["hasSchema"].ToString() == "true" ? true : false;

            tableList.Add(tableItem);
            Debug.Log(tableItem.ToString());
        }
        Debug.Log("테이블 리스트 가져오기 성공");
    }
}