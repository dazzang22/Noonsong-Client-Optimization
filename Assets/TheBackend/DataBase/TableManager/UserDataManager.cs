using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using BackEnd;
using LitJson;

//테이블 이름: UserProfile
//테이블 개별 데이터(행): UserProfileData
public class UserDataManager
{
    private static UserDataManager _instance = null;

    public static UserDataManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new UserDataManager();
            }

            return _instance;
        }
    }
    private string gameDataRowInDate=string.Empty;

//회원가입 이후 최초 유저 등록
    public void InsertUserData(string id,string pw,string nickname, string email)
	{

        var bro1 = Backend.BMember.GetUserInfo();

        if (!bro1.IsSuccess())
        {
            Debug.LogError("에러가 발생했습니다 : " + bro1.ToString());
            return;
        }

        JsonData userInfoJson = bro1.GetReturnValuetoJSON()["row"];
        Debug.Log(userInfoJson.ToString());
        UserProfileData user= new UserProfileData(userInfoJson);
        user.setUserProfile(id,pw,nickname,email);
        Param param=user.ToParam();

        Debug.Log("게임 정보 데이터 삽입을 요청합니다.");       
        var bro = Backend.GameData.Insert("UserProfile", param);

        if (bro.IsSuccess())
        {
            Debug.Log("게임 정보 데이터 삽입에 성공했습니다. : " + bro);
            gameDataRowInDate=bro.GetInDate();
        }
        else
        {
            Debug.LogError("게임 정보 데이터 삽입에 실패했습니다. : " + bro);
        }


	}

    
    //회원가입 한 날짜 (indate) 으로 특정 회원 조회 -> 회원 엔티티 반환.
    public UserProfileData findUser(string indate)
    {
        Where where = new Where();
        where.Equal("owner_inDate", indate);
        var bro = Backend.GameData.Get("UserProfile", where, 10);

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

        if(gameDataListJson.Count <= 0)
        {
            Debug.Log("데이터가 존재하지 않습니다");
            return null;
        }

        gameDataRowInDate = gameDataListJson["inDate"].ToString();
        UserProfileData gameDataItem = new UserProfileData();
        gameDataItem.setUser(gameDataListJson);

        return gameDataItem;
    }

    //id 로드
    public string getUserID()
    {
        UserProfileData userProfileData=findUser(Backend.UserInDate);
        
        string userid= userProfileData.getuserId();
        return userid;
    }
    //nickname 로드
    public string getUserNickname()
    {
        UserProfileData userProfileData=findUser(Backend.UserInDate);
        string usernick= userProfileData.getUserNickname();
        return usernick;
    }

    //세이브 포인트 로드
    public int getSave()
    {
        UserProfileData userProfileData=findUser(Backend.UserInDate);
        int cursave=userProfileData.getSave();
        Debug.Log($"{cursave}");
        return cursave;

    }
    //파견 잔여시간간
    public int getSendingTime()
    {
        UserProfileData userProfileData=findUser(Backend.UserInDate);
        int time = userProfileData.getSendingTime();
        Debug.Log($"파견 잔여시간: {time}");
        return time;
    }

    //튜토리얼 세이브 포인트 저장
    public Param ChangeSave(int point)
    {
        UserProfileData userProfileData=findUser(Backend.UserInDate);
        userProfileData.setSave(point);
        Param param=userProfileData.ToParam();
        return param;
    }

    //비밀번호 수정 -> 변경값을 전달
    public void ChangePassword(string newPw)
    {
        UserProfileData userProfileData=findUser(Backend.UserInDate);
        userProfileData.setPW(newPw);
        Param param=userProfileData.ToParam();
        UpdateUserData(param);
    }

    //닉네임 변경   -> 변경 값을 전달
    public Param ChangeNickname(string newnick)
    {
        UserProfileData userProfileData=findUser(Backend.UserInDate);
        userProfileData.setNickname(newnick);
        Param param=userProfileData.ToParam();
        Debug.Log(param.GetJson().ToString());
        return param;
    }

    //숙명 이메일 저장
    public void SetSEmail(string email)
    {
        UserProfileData userProfileData=findUser(Backend.UserInDate);
        userProfileData.setSMail(email);
        Param param=userProfileData.ToParam();
        UpdateUserData(param);
    }
    public void changeEmail(string email)
    {
        UserProfileData userProfileData=findUser(Backend.UserInDate);
        userProfileData.setMail(email);
        Param param=userProfileData.ToParam();
        UpdateUserData(param);
    }
    //파견하기 시간 count
    public void setSendingTime(int time)
    {
        UserProfileData userProfileData=findUser(Backend.UserInDate);
        userProfileData.setSendingTime(time);
        Param param=userProfileData.ToParam();
        UpdateUserData(param);
    }
    

    //유저 탈퇴
    public void DeleteUser()
    {
        Where where = new Where();
        where.Equal("userId", getUserID());
        Backend.GameData.Delete("UserProfile", where);
    }
    

    //회원정보 수정 반영 (업데이트) -> 수정 함수로 변경 결과 (Param)를 받아서 테이블 업데이트
    public void UpdateUserData(Param param)
    {   
        if(param==null)
        {
            Debug.LogError("Param이 존재하지 않습니다.");
            return;
        }
        BackendReturnObject bro=null;
        if(string.IsNullOrEmpty(gameDataRowInDate))
        {
            Debug.Log("내 제일 최신 게임 정보 데이터 수정을 요청합니다.");
            bro = Backend.GameData.Update("UserProfile",new Where(),param);
        }
        else
        {
            Debug.Log($"{gameDataRowInDate}의 게임 정보 데이터 수정을 요청합니다.");
            bro = Backend.GameData.UpdateV2("UserProfile", gameDataRowInDate ,Backend.UserInDate, param);

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