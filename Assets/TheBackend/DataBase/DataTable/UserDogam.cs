using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


using BackEnd;

public class UserDogam 
{
    public string userId;
    public int noonsongId=0;
    public bool friend=false; //친구 유무
    public int count=0; // 결정 개수
    public int favor =0; //호감도
    public bool max=false; //베프 유무
    public string updatedAt;

    public UserDogam()
    {

    }

    public UserDogam(LitJson.JsonData json)
    {
        this.userId=json["userId"].ToString();
        this.noonsongId= int.Parse(json["noonsongId"].ToString());
        this.count=int.Parse(json["count"].ToString());  
        this.favor=int.Parse(json["favor"].ToString()); 
        this.friend=bool.Parse(json["friend"].ToString()); 
        this.max=bool.Parse(json["max"].ToString()); 
        this.updatedAt = json["updatedAt"].ToString();   
   
  
    
    }
    public void setUserDogam(string userId, int noonsongId, int count,int favor, bool friend, bool max)
    {
        this.userId=userId;
        this.noonsongId=noonsongId;
        this.favor=favor;
        this.count=count;
        this.friend=friend;
        this.max=max;
    }

    public int getFavor()
    {
        return this.favor;
    }

    public int getCount()
    {
        return this.count;
    }

    public bool getFriend()
    {
        return this.friend;
    }

    public void setFavorUp(int love)
    {
        this.favor+=love;
    }
    public void setFavorMax(int love)
    {
        this.favor=love;
        this.max=true;
    }

    public void setCountUp(int count)
    {
        this.count+=count;
    }

    public void setFriend()
    {
        this.friend=true;
        //this.count=0;
    }


    public override string ToString()
    {
        StringBuilder result = new StringBuilder();
        result.AppendLine($"userId: {userId}");
        result.AppendLine($"noonsongId: {noonsongId}");
        result.AppendLine($"count: {count}");
        result.AppendLine($"favor: {favor}");
        result.AppendLine($"friend: {friend}");
        result.AppendLine($"max: {max}");

        return result.ToString();
    }

    public Param ToParam()
    {
        Param param = new Param();

        param.Add("userId",userId);
        param.Add("noonsongId",noonsongId);
        param.Add("count",count);
        param.Add("favor",favor);
        param.Add("friend",friend);
        param.Add("max",max);
        //param.Add("updatedAt",updatedAt);


        return param;
    }
}

  