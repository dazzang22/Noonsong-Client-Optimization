using System.Collections.Generic;
using System.Text;
using UnityEngine;

// 뒤끝 SDK namespace 추가
using BackEnd;

public class NoonsongStatisticsManager {
    private static NoonsongStatisticsManager _instance = null;

    public static NoonsongStatisticsManager Instance {
        get {
            if(_instance == null) {
                _instance = new NoonsongStatisticsManager();
            }

            return _instance;
        }
    }
    private string gameDataRowInDate = string.Empty;

    //초기화
    public void reset(string noon)
    {
        //UserStatistics에 새롭게 유저 생성
        NoonsongStatistics noons= new NoonsongStatistics();
        noons.setNoonsongStatistics(noon,0);
        Param param=noons.ToParam();

        Debug.Log("게임 정보 데이터 삽입을 요청합니다.");       
        var bro2 = Backend.GameData.Insert("NoonsongStatistics", param);

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
    //테이블에 눈송 지표 등록
    public void insertNoonsongStatistic(string noon,int num)
    {
        //테이블에 유저 있는지 확인
        Where where = new Where();
        where.Equal("noonsongName", noon);
        var bro1 = Backend.GameData.Get("NoonsongStatistics", where, 100);
        if(bro1.IsSuccess()&& bro1.FlattenRows().Count > 0)
        {
            Debug.Log("테이블에 이미 존재함.");
            updateNoonsongStatistics(noon,num);
            return;
        }
        
        //UserStatistics에 새롭게 유저 생성
        NoonsongStatistics noons= new NoonsongStatistics();
        noons.setNoonsongStatistics(noon,num);
        Param param=noons.ToParam();

        Debug.Log("게임 정보 데이터 삽입을 요청합니다.");       
        var bro2 = Backend.GameData.Insert("NoonsongStatistics", param);

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

    //눈송 지표 테이블 리스트 조회
    public NoonsongStatistics getNoonsongStatistic(string noon)
    {
        Where where = new Where();
        where.Equal("noonsongName", noon);
        var bro = Backend.GameData.Get("NoonsongStatistics", where, 10);
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
        Debug.Log($"{gameDataRowInDate}");
        NoonsongStatistics gameDataItem = new NoonsongStatistics(gameDataListJson);

        Debug.Log(gameDataItem.ToString());
        return gameDataItem;

    }
    

    //특정 눈송이 지표 확인
    public int getNoons(string noon)
    {
        NoonsongStatistics noonStatistics= getNoonsongStatistic(noon);
        int noonnum = noonStatistics.getnoonNum();
        return noonnum;   
    }

    //유저 validateMap 업데이트 (추가) : 구역이름으로 1개씩 전달하면 숫자로 저장됨.
    public void updateNoonsongStatistics(string noon,int num)
    {
        NoonsongStatistics noonStatistics= getNoonsongStatistic(noon);
        noonStatistics.updateNoonNum(num);
        Param param = new Param();
        param= noonStatistics.ToParam();
        UpdateNoonsongStatistic(param);
    }

    //뒤끝 UserMap에 테이블 업데이트
    public void UpdateNoonsongStatistic(Param param)
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
            bro = Backend.GameData.Update("NoonsongStatistics",new Where(),param);
        }
        else
        {
            Debug.Log($"{gameDataRowInDate}의 게임 정보 데이터 수정을 요청합니다.");
            bro = Backend.GameData.UpdateV2("NoonsongStatistics", gameDataRowInDate ,Backend.UserInDate, param);

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