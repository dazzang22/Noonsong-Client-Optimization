using System.Collections.Generic;
using System.Text;
using UnityEngine;

// 뒤끝 SDK namespace 추가
using BackEnd;

public class UserStatisticsManager {
    private static UserStatisticsManager _instance = null;

    public static UserStatisticsManager Instance {
        get {
            if(_instance == null) {
                _instance = new UserStatisticsManager();
            }

            return _instance;
        }
    }
    private string gameDataRowInDate = string.Empty;

    //테이블에 리스트 등록
    public void insertUserStatistics(string userid,int num, List<int> friendsList = null)
    {
        //테이블에 유저 있는지 확인
        Where where = new Where();
        where.Equal("userId", userid);
        var bro1 = Backend.GameData.Get("UserStatistics", where, 10);
        if(bro1.IsSuccess()&& bro1.FlattenRows().Count > 0)
        {
            Debug.Log("테이블에 유저 이미 존재함.");
            updateUserFriendsList(userid,num,friendsList);
            return;
        }
        
        //UserStatistics에 새롭게 유저 생성
        UserStatistics user= new UserStatistics();
        user.setUserStatistics(userid,num, friendsList);
        Param param=user.ToParam();

        Debug.Log("게임 정보 데이터 삽입을 요청합니다.");       
        var bro2 = Backend.GameData.Insert("UserStatistics", param);

        if (bro2.IsSuccess())
        {
            Debug.Log("게임 정보 데이터 삽입에 성공했습니다. : " + bro2);
            gameDataRowInDate=bro2.GetInDate();
        }
        else
        {
            Debug.LogError("게임 정보 데이터 삽입에 실패했습니다. : " + bro2);
        }
    }

    //유저 테이블 리스트 조회
    public UserStatistics getUserStatisticsJson(string userId)
    {
        Where where = new Where();
        where.Equal("userId", userId);
        var bro = Backend.GameData.Get("UserStatistics", where, 10);
        if(!bro.IsSuccess())
        {
            if(bro.IsBadAccessTokenError()) 
            {
                Debug.Log("액세스토큰이 만료되었을 경우");
                
                var bro2 = Backend.BMember.RefreshTheBackendToken();
                if(bro2.GetMessage() == "bad refreshToken") 
                {
                    Debug.Log("로그인 정보가 만료되었습니다.");
                }
            }
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
        UserStatistics gameDataItem = new UserStatistics(gameDataListJson);

        Debug.Log(gameDataItem.ToString());
        return gameDataItem;

    }

    //특정 유저 도감 친구 리스트 조회
    public List<string> getUserfList(string userId)
    {
        UserStatistics userStatistics= getUserStatisticsJson(userId);
        List<int> userStatisticsId= new List<int>(userStatistics.getUserfriendsList());
        List<string> userFriends= new List<string>();
        for(int i=0;i<userStatisticsId.Count;i++)
        {
            userFriends.Add(DogamChartManager.Instance.getNoonsongCollege(userStatisticsId[i]));
            Debug.Log($"{userStatisticsId[i]} : {userFriends[i]}");
        }
        return userFriends;   
    }

    //유저 validateMap 업데이트 (추가) : 구역이름으로 1개씩 전달하면 숫자로 저장됨.
    public void updateUserFriendsList(string userId,int num, List<int> newFriendsList)
    {
        UserStatistics userStatistics= getUserStatisticsJson(userId);
        userStatistics.updatefriendsList(num, newFriendsList);
        Param param = new Param();
        param= userStatistics.ToParam();
        UpdateUserStatistics(param);
    }

    //뒤끝 UserMap에 테이블 업데이트
    public void UpdateUserStatistics(Param param)
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
            bro = Backend.GameData.Update("UserStatistics",new Where(),param);
        }
        else
        {
            Debug.Log($"{gameDataRowInDate}의 게임 정보 데이터 수정을 요청합니다.");
            bro = Backend.GameData.UpdateV2("UserStatistics", gameDataRowInDate ,Backend.UserInDate, param);

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