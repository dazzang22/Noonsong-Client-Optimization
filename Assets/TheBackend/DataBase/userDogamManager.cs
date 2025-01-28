using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using BackEnd;

public class userDogamManager
{
    private static userDogamManager _instance = null;

    public static userDogamManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new userDogamManager();
            }

            return _instance;
        }
    }
    public static userDogam userinventory;

    private string gameDataRowInDate = string.Empty;
    public void DataInsert(int userid, int noonsongid, string location)
    {
        if(userinventory == null)
        {
            userinventory = new userDogam();
        }

        //if(userinventory.noonsongId)
        Debug.Log("데이터를 초기화합니다.");
        userinventory.userId=userid;
        userinventory.noonsongId=noonsongid;
        userinventory.count=1;
        userinventory.location=location;

        Debug.Log("뒤끝 업데이트 목록에 해당 데이터들을 추가합니다.");
        Param param = new Param();
        param.Add("userId", userinventory.userId);
        param.Add("noonsongId", userinventory.noonsongId);
        param.Add("count", userinventory.count);
        param.Add("location", userinventory.location);

        Debug.Log("게임 정보 데이터 삽입을 요청합니다.");
        var bro = Backend.GameData.Insert("UserInventory", param);

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
    public void DataGet()
    {
        Debug.Log("게임 정보 조회 함수를 호출합니다.");

        var bro = Backend.GameData.GetMyData("USER_DATA", new Where());

        if (bro.IsSuccess())
        {
            Debug.Log("게임 정보 조회에 성공했습니다. : " + bro);


            LitJson.JsonData gameDataJson = bro.FlattenRows(); // Json으로 리턴된 데이터를 받아옵니다.  

            // 받아온 데이터의 갯수가 0이라면 데이터가 존재하지 않는 것입니다.  
            if (gameDataJson.Count <= 0)
            {
                Debug.LogWarning("데이터가 존재하지 않습니다.");
            }
            else
            {
                gameDataRowInDate = gameDataJson[0]["inDate"].ToString(); //불러온 게임 정보의 고유값입니다.  

                userinventory = new userDogam();

                userinventory.userId = int.Parse(gameDataJson[0]["userId"].ToString());
                userinventory.noonsongId = int.Parse(gameDataJson[0]["noonsongId"].ToString());
                userinventory.count = int.Parse(gameDataJson[0]["count"].ToString());
                userinventory.location = gameDataJson[0]["location"].ToString();

                Debug.Log(userinventory.ToString());
            }
        }
        else
        {
            Debug.LogError("게임 정보 조회에 실패했습니다. : " + bro);
        }
        
    }
    public void CountUp()
    {
        if(userinventory.count>0)
        {
            Debug.Log("눈송이 만남 횟수를 1 증가시킵니다.");
            userinventory.count+=1;
        }
    }

    public void DataUpdate()
    {
        if(userinventory == null)
        {
            Debug.LogError("서버에서 다운받거나 새로 삽입한 데이터가 존재하지 않습니다. Insert 혹은 Get을 통해 데이터를 생성해주세요.");
            return;
        }
        Param param = new Param();
        param.Add("userId", userinventory.userId);
        param.Add("noonsongId", userinventory.noonsongId);
        param.Add("count", userinventory.count);
        param.Add("location", userinventory.location);
        
        BackendReturnObject bro = null;

        if (string.IsNullOrEmpty(gameDataRowInDate))
        {
            Debug.Log("내 제일 최신 게임 정보 데이터 수정을 요청합니다.");

            bro = Backend.GameData.Update("USER_DATA", new Where(), param);
        }
        else
        {
            Debug.Log($"{gameDataRowInDate}의 게임 정보 데이터 수정을 요청합니다.");

            bro = Backend.GameData.UpdateV2("USER_DATA", gameDataRowInDate, Backend.UserInDate, param);
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