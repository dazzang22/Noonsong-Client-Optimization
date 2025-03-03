using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using BackEnd;

public class UserBalanceManager
{
    private static UserBalanceManager _instance = null;

    public static UserBalanceManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new UserBalanceManager();
            }

            return _instance;
        }
    }
    public static UserBalance userbalance;

    private string gameDataRowInDate = string.Empty;

    //유저 밸런스 등록하기
    public void InsertUserBalance(string userid,int balance)
    {
        //유저 정보 불러오기
        var bro= Backend.BMember.GetUserInfo();
        if(!bro.IsSuccess())
        {
            Debug.LogError("에러가 발생했습니다 : " + bro.ToString());
            return;
        }
        LitJson.JsonData userInfoJson = bro.GetReturnValuetoJSON()["row"];
        //Debug.Log(userInfoJson.ToString());

        //테이블에 유저 있는지 확인
        Where where = new Where();
        where.Equal("userId", userid);
        var bro1 = Backend.GameData.Get("UserBalance", where, 10);
        if(bro1.IsSuccess()&& bro1.FlattenRows().Count > 0)
        {
            Debug.Log("테이블에 유저 이미 존재함.");
            return;
        }
        
        //UserBalance에 새롭게 유저 생성
        UserBalance user= new UserBalance();
        user.setuserBalance(userid,balance);
        Param param=user.ToParam();

        Debug.Log("게임 정보 데이터 삽입을 요청합니다.");       
        var bro2 = Backend.GameData.Insert("UserBalance", param);

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

    //유저 밸런스 조회 가져오기 -> return UserBalance
    public UserBalance getUserBalanceJson()
    {
        Where where = new Where();
        where.Equal("owner_inDate", Backend.UserInDate);
        var bro = Backend.GameData.Get("UserBalance", where, 10);

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
        UserBalance gameDataItem = new UserBalance(gameDataListJson);

        Debug.Log(gameDataItem.ToString());
        return gameDataItem;

    }

    //유저 눈의 결정 개수 return
    public int getUserBalance()
    {
        UserBalance userBalance = getUserBalanceJson();
        return userBalance.getBalance();
    }

    //눈의 결정 추가
    public Param addsnow(int snowcount)
    {
        UserBalance userBalance = getUserBalanceJson();
        userBalance.setBalance(userBalance.getBalance()+snowcount);
        Param param= userBalance.ToParam();
        return param;
    }

    //눈의 결정 소비
    public Param paysnow(int snowcount)
    {
        UserBalance userBalance = getUserBalanceJson();
        int now= getUserBalance();
        if(now<snowcount)
        {
            Debug.LogError("눈의 결정이 부족합니다.");
            return null;
        }
        userBalance.setBalance(now-snowcount);
        Param param= userBalance.ToParam();
        return param;
    }

    //접속 중에 5분마다마다 자동으로 눈의 결정 3개씩 증가

    //파견에 의한 보상 수령하기( 파견 소요 시간이 지나야지 가능 )
    public void getReward(int snows)
    {

    }

    //뒤끝 콘솔 UserBalance 테이블에 업데이트 반영영
    public void updateBalance(Param param)
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
            bro = Backend.GameData.Update("UserBalance",new Where(),param);
        }
        else
        {
            Debug.Log($"{gameDataRowInDate}의 게임 정보 데이터 수정을 요청합니다.");
            bro = Backend.GameData.UpdateV2("UserBalance", gameDataRowInDate ,Backend.UserInDate, param);

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