using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using BackEnd;

public class UserDogamManager
{
    private static UserDogamManager _instance = null;

    public static UserDogamManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new UserDogamManager();
            }

            return _instance;
        }
    }
    public static UserDogam userinventory;

    private string gameDataRowInDate = string.Empty;

    //유저의 행들의 집합 찾기 & 등록된 눈송이 리스트업
    public List<UserDogam> getUserDogamList(string userid)
    {
        Debug.Log($"{userid}의 도감 행들을 찾습니다.");
        var bro= Backend.GameData.GetMyData("UserDogam",new Where());

        List<UserDogam> userdogamList = new List<UserDogam>();

        if(bro.IsSuccess())
        {
            Debug.Log("게임 정보 조회에 성공했습니다.: "+bro);

            LitJson.JsonData userdogamjson = bro.FlattenRows();

            if(userdogamjson.Count<=0)
            {
                Debug.LogWarning("데이터가 존재하지 않습니다.");
                return null;
            }
            else
            {
                foreach(LitJson.JsonData udjson in userdogamjson)
                {
                    UserDogam us=new UserDogam(udjson);
                    Debug.Log(us.ToString());
                    userdogamList.Add(us);
                }
            }
        }
        else
        {
            Debug.LogError("게임 정보 조회에 실패했습니다.: "+bro);
            return null;
        }
        return userdogamList;
    }

    //UserDogam 테이블에 도감 등록하기
    public void noonsongInsert(string userid, int noonsongid, string location)
    {
        //도감에 해당 눈송이가 등록되어 있으면 count +1 증가.
        //1. 유저 행들의 집합 찾기
        //2. 거기서 해당 눈송 id가 있는지 확인
        //3. 없으면 등록 진행
        //4. 있으면 count 함수 진행

        Debug.Log("유저도감 테이블에서 유저의 행을 찾아 등록 여부를 확인합니다.");
        List<UserDogam> userdogamlist = new List<UserDogam>();
        userdogamlist=getUserDogamList(userid);
        Param param = new Param();
        if(userdogamlist!=null)
        {
            foreach(UserDogam ud in userdogamlist)
            {
                if(ud.noonsongId == noonsongid)
                {
                    Debug.Log($"{noonsongid}에 countup을 합니다.");
                    userinventory = ud;
                    param = CountUp();
                    DataUpdate(param);
                    return;
                }
            }
        }

        Debug.Log("도감에 등록되어 있지 않으므로 등록을 진행합니다.");
        if(userinventory == null)
        {
            userinventory = new UserDogam();
        }

        Debug.Log("데이터를 초기화합니다.");
        userinventory.setUserDogam(userid,noonsongid,1,location);
        param = userinventory.ToParam();

        Debug.Log("게임 정보 데이터 삽입을 요청합니다.");
        var bro = Backend.GameData.Insert("UserDogam", param);

        if (bro.IsSuccess())
        {
            Debug.Log("게임 정보 데이터 삽입에 성공했습니다. : " + bro);

            //삽입한 게임 정보의 고유값입니다.  
            gameDataRowInDate = bro.GetInDate();
        }
        else
        {
            Debug.LogError("게임 정보 데이터 삽입에 실패했습니다. : " + bro);
        }


    }

    //UserDogam 테이블에서 유저가 만난 눈송이 데이터의 count를 1 증가시킴.
    public Param CountUp()
    {
        if(userinventory.count>0)
        {
            Debug.Log("눈송이 만남 횟수를 1 증가시킵니다.");
            userinventory.setCountUp();
            Debug.Log(userinventory.count);
        }
        return userinventory.ToParam();
    }

    //UserDogam 테이블에서 해당 유저 데이터 업데이트
    public void DataUpdate(Param param)
    {
        if(param == null)
        {
            Debug.LogError("Param이 존재하지 않습니다.");
            return;
        }
        if(userinventory == null)
        {
            Debug.LogError("서버에서 다운받거나 새로 삽입한 데이터가 존재하지 않습니다. Insert 혹은 Get을 통해 데이터를 생성해주세요.");
            return;
        }
        
        //Param param = new Param();
        //param = userinventory.ToParam();
        Debug.Log(userinventory.ToString());
        
        BackendReturnObject bro = null;

        if (string.IsNullOrEmpty(gameDataRowInDate))
        {
            Debug.Log("내 제일 최신 게임 정보 데이터 수정을 요청합니다.");

            bro = Backend.GameData.Update("UserDogam", new Where(), param);
        }
        else
        {
            Debug.Log($"{gameDataRowInDate}의 게임 정보 데이터 수정을 요청합니다.");

            bro = Backend.GameData.UpdateV2("UserDogam", gameDataRowInDate, Backend.UserInDate, param);
        }

        if (bro.IsSuccess())
        {
            Debug.Log("게임 정보 데이터 수정에 성공했습니다. : " + bro);
        }
        else
        {
            Debug.LogError("게임 정보 데이터 수정에 실패했습니다. : " + bro);
        }

    }
}