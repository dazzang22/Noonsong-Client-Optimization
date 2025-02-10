using System.Collections.Generic;
using System.Text;
using UnityEngine;

// 뒤끝 SDK namespace 추가
using BackEnd;

public class UserMapManager {
    private static UserMapManager _instance = null;

    public static UserMapManager Instance {
        get {
            if(_instance == null) {
                _instance = new UserMapManager();
            }

            return _instance;
        }
    }
    public static UserMap userMap;
    private string gameDataRowInDate = string.Empty;

    //테이블에 유저 등록
    public void insertUserMap(string userid, List<int> maps = null)
    {
        //유저 정보 불러오기
        var bro= Backend.BMember.GetUserInfo();
        if(!bro.IsSuccess())
        {
            Debug.LogError("에러가 발생했습니다 : " + bro.ToString());
            return;
        }
        LitJson.JsonData userInfoJson = bro.GetReturnValuetoJSON()["row"];

        //테이블에 유저 있는지 확인
        Where where = new Where();
        where.Equal("userId", userid);
        var bro1 = Backend.GameData.Get("UserMap", where, 10);
        if(bro1.IsSuccess()&& bro1.FlattenRows().Count > 0)
        {
            Debug.Log("테이블에 유저 이미 존재함.");
            return;
        }
        
        //UserMap에 새롭게 유저 생성
        UserMap user= new UserMap();
        user.setUserValidateMap(userid,maps);
        Param param=user.ToParam();

        Debug.Log("게임 정보 데이터 삽입을 요청합니다.");       
        var bro2 = Backend.GameData.Insert("UserMap", param);

        if (bro2.IsSuccess())
        {
            Debug.Log("게임 정보 데이터 삽입에 성공했습니다. : " + bro);
            gameDataRowInDate=bro.GetInDate();
        }
        else
        {
            Debug.LogError("게임 정보 데이터 삽입에 실패했습니다. : " + bro);
        }
    }

    //유저 테이블 리스트 조회
    public UserMap getUserMapJson()
    {
        Where where = new Where();
        where.Equal("owner_inDate", Backend.UserInDate);
        var bro = Backend.GameData.Get("UserMap", where, 10);
        if(!bro.IsSuccess())
        {
            Debug.LogError(bro.ToString());
            return null;
        }

        LitJson.JsonData gameDataListJson = bro.FlattenRows()[0];
        Debug.Log(gameDataListJson.ToJson());

        if(gameDataListJson.Count <= 0)
        {
            Debug.Log("데이터가 존재하지 않습니다");
            return null;
        }

        gameDataRowInDate = gameDataListJson["inDate"].ToString();
        UserMap gameDataItem = new UserMap(gameDataListJson);

        Debug.Log(gameDataItem.ToString());
        return gameDataItem;

    }

    //유저 활성화 map 리스트 조회
    public List<string> getUserMapList()
    {
        UserMap userMap= getUserMapJson();
        List<int> userMapid= new List<int>(userMap.getValidateMap());
        List<string> usermap= new List<string>();
        for(int i=0;i<userMapid.Count;i++)
        {
            usermap.Add(MapChartManager.Instance.getMapName(userMapid[i]));
            Debug.Log($"{userMapid[i]} : {usermap[i]}");
        }
        return usermap;   
    }

    //유저 validateMap 업데이트 (추가) : 구역이름으로 1개씩 전달하면 숫자로 저장됨.
    public Param addUserValidate(string name)
    {
        int mapid= MapChartManager.Instance.getMapId(name);
        UserMap usermap= getUserMapJson();
        usermap.addValidateMap(mapid);
        return usermap.ToParam();
    }

    //뒤끝 UserMap에 테이블 업데이트
    public void UpdateUserMap(Param param)
    {   
        if(param == null)
        {
            Debug.LogError("Param이 존재하지 않습니다.");
            return;
        }
        BackendReturnObject bro=null;
        if(string.IsNullOrEmpty(gameDataRowInDate))
        {
            Debug.Log("내 제일 최신 게임 정보 데이터 수정을 요청합니다.");
            bro = Backend.GameData.Update("UserMap",new Where(),param);
        }
        else
        {
            Debug.Log($"{gameDataRowInDate}의 게임 정보 데이터 수정을 요청합니다.");
            bro = Backend.GameData.UpdateV2("UserMap", gameDataRowInDate ,Backend.UserInDate, param);

        }
        if(bro.IsSuccess())
        {
            Debug.Log("테이블 업데이트에 성공했습니다.");
        }
        else
        {
            Debug.LogError(bro.ToString());
        }
    }


}