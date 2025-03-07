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
    //public static UserDogam userinventory;
    public List<UserDogam> userdogamList;
    public List<UserDogam> userfriendList;
    public string userId;

    private string gameDataRowInDate = string.Empty;

    //테이블 처음 등록 및 로드하기
    public void First()
    {
        userdogamList = new List<UserDogam>();
        userfriendList = new List<UserDogam>();
        userId=UserDataManager.Instance.getUserID();
        userdogamList=getUserDogamList();
        UserDogam userinventory = new UserDogam();
        if(userdogamList!=null)
        {
            foreach(UserDogam ud in userdogamList)
            {
                userinventory=ud;
                Debug.Log($"{userinventory.noonsongId}: {userinventory.count}");
            }
        }
    }

    //--------------------------------------------------------------------------------
    //UserDogam 테이블 조회
    //--------------------------------------------------------------------------------

    //DB에서 유저의 행들의 집합 찾기 & 등록된 눈송이 리스트업 가져오기기
    public List<UserDogam> getUserDogamList()
    {
        Debug.Log($"{userId}의 도감 행들을 찾습니다.");
        var bro= Backend.GameData.GetMyData("UserDogam",new Where());
        List<UserDogam> userdogamlist = new List<UserDogam>();

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
                    
                    userdogamlist.Add(us);
                    if(us.getFriend())
                    {
                        addFriendList(us);
                        //userfriendList.Add(us);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("게임 정보 조회에 실패했습니다.: "+bro);
            return null;
        }
        return userdogamlist;
    }

//친구 눈송이리스트에 추가
    public void addFriendList(UserDogam ad)
    {
        foreach(UserDogam ud in userfriendList)
        {
            if(ud.noonsongId == ad.noonsongId)
            {
                return;
            }
        }
        userfriendList.Add(ad);
    }
    
    //유저 친구 눈송이 개수 가져오기
    public int getFriendNum()
    {
        if(userfriendList != null)
        {
            foreach(UserDogam ud in userfriendList)
            {
                Debug.Log($"{ud.noonsongId}");
            }
            return userfriendList.Count;
        }
        else
        {
            return 0;
        }
        
    }

    //--------------------------------------------------------------------------------
    //UserDogam 테이블 추가 및 업데이트
    //--------------------------------------------------------------------------------

    //UserDogam 테이블에 도감 추가하기
    public void noonsongInsert(string noonsong, int love,string college)
    {
        UserDogam userinventory = new UserDogam();

        Debug.Log("유저도감 테이블에서 유저의 행을 찾아 등록 여부를 확인합니다.");
        int noonsongid= DogamChartManager.Instance.getSnowflakeId(noonsong);
        Param param = new Param();
        if(userdogamList!=null)
        {
            foreach(UserDogam ud in userdogamList)
            {
                if(ud.noonsongId == noonsongid)
                {
                    Debug.Log($"{noonsong}, {noonsongid}에 친밀도 업데이트 합니다.");
                    userinventory = ud;

                    userinventory.setFavorUp(love);
                    //결정 , 도감 등록, 최대 호감도 확인
                    checkMaxFavor(college,userinventory);
                    checkCount(college,userinventory);
                    checkIsDogam(college,userinventory);
                    DataUpdate(userinventory.ToParam());
                    return;
                }
            }
        }

        if(userinventory == null)
        {
            Debug.Log("도감에 등록되어 있지 않으므로 도감에 추가합니다.");
            //userinventory = new UserDogam();
        }

        //Debug.Log("데이터를 처음 등록합니다.");
        userinventory.setUserDogam(userId,noonsongid,love);
        checkMaxFavor(college,userinventory);
        checkCount(college,userinventory);
        checkIsDogam(college,userinventory);
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

    //결정 개수 증가 여부 확인
    public void checkCount(string college,UserDogam userinventory)
    {
        int crystal= DogamChartManager.Instance.crystalValue(college);
        Debug.Log($"crystal:{crystal}");
        Debug.Log($"crystal:{userinventory.getFavor()/crystal}");

        if(userinventory.getFavor()/crystal > userinventory.getCount())
        {
            int increase= userinventory.getFavor()/crystal - userinventory.getCount();
            userinventory.setCountUp(increase);
            DataUpdate(userinventory.ToParam());
            Debug.Log($"{increase}결정 개수 증가");
        }
    }

    //도감 등록 변환 확인
    public void checkIsDogam(string college,UserDogam userinventory)
    {
        int friend=DogamChartManager.Instance.friendFavor(college);
        Debug.Log($"friend:{friend}");

        if(userinventory.getFavor()>=friend && userinventory.getCount()==5)
        {
            userinventory.setFriend(); //isdogam true
            DataUpdate(userinventory.ToParam());
            addFriendList(userinventory);
            Debug.Log("도감 등록");
        }

    }

    //최대 호감도 확인
    public void checkMaxFavor(string college,UserDogam userinventory)
    {
        int favor= DogamChartManager.Instance.maxFavor(college);
        if(!userinventory.getFriend())
        {
            return;
        }
        if(userinventory.getFavor()>=favor)
        {
            userinventory.setFavorMax(favor); //친밀도 최대로 지정
            DataUpdate(userinventory.ToParam());
            Debug.Log("최대 호감도 도달");
        }
    }

    //UserDogam 테이블에서 해당 유저 데이터 테이블에 업데이트
    public void DataUpdate(Param param)
    {
        if(param == null)
        {
            Debug.LogError("Param이 존재하지 않습니다.");
            return;
        }
        
        //Debug.Log(userinventory.ToString());
        
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