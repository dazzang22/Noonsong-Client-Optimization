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
    public static UserProfileData userdata;
    private string gameDataRowInDate=string.Empty;

//회원가입 이후 최초 유저 등록
    public void InsertUserData(string id,string pw)
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
        user.setIdPw(id,pw);
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

    //전체 회원 조회 (private table은 자기 자신 데이터만 조회할 수 있어서.. 일단 중지..)
    public void getAllUsersList()
    {
        // List<string> userIdList = new List<string>();
        // var bro = Backend.GameData.Get("UserProfile",new Where());
        
        // if(!bro.IsSuccess())
        // {
        //     Debug.Log("데이터 읽기 중 문제 발생: "+ bro);
        // }

        // LitJson.JsonData users=bro.FlattenRows();

        // if(users==null || users.Count <= 0)
        // {
        //     Debug.Log("불러오기는 가능하지만, 데이터가 존재하지 않음.");
        // }
        
        // for(int i=0;i<users.Count;i++)
        // {
        //     userIdList.Add(users[i]["userId"].ToString());
        //     Debug.Log(i+userIdList[i]);
        // }
        // Debug.Log(string.Join(", ",userIdList));

    }

    //회원가입 한 날짜 (indate) 으로 특정 회원 조회 -> 회원 엔티티 반환.
    public UserProfileData findUser(string indate)
    {
        Where where = new Where();
        where.Equal("owner_inDate", indate);
        var bro = Backend.GameData.Get("UserProfile", where, 10);

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
        UserProfileData gameDataItem = new UserProfileData();
        gameDataItem.setUser(gameDataListJson);

        Debug.Log(gameDataItem.ToString());
        return gameDataItem;
    }

    public string getUserID()
    {
        UserProfileData userProfileData=findUser(Backend.UserInDate);
        string userid= userProfileData.getuserId();
        return userid;
    }

    //비밀번호 수정 -> 변경값을 전달
    public Param ChangePassword(string newPw)
    {
        //비밀번호 수정 코드 작성
        //UserProfileData에서 Param 생성 해서 return하기
        return null;
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