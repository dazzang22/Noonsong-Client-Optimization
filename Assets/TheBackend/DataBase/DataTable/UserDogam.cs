using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using BackEnd;

public class UserDogam 
{
    public string userId;
    public int noonsongId=0;
    public int count=0;
    public string location=string.Empty;

    public int favor =0;

    public UserDogam()
    {

    }

    public UserDogam(LitJson.JsonData json)
    {
        this.userId=json["userId"].ToString();
        this.noonsongId= int.Parse(json["noonsongId"].ToString());
        this.count=int.Parse(json["count"].ToString());
        this.location=json["location"].ToString();
    }
    public void setUserDogam(string userId, int noonsongId, int count, string location)
    {
        this.userId=userId;
        this.noonsongId=noonsongId;
        this.count=count;
        this.location=location;
    }

    public void setCountUp()
    {
        this.count+=1;
    }


    public override string ToString()
    {
        StringBuilder result = new StringBuilder();
        result.AppendLine($"userId: {userId}");
        result.AppendLine($"noonsongId: {noonsongId}");
        result.AppendLine($"count: {count}");
        result.AppendLine($"location: {location}");

        return result.ToString();
    }

    public Param ToParam()
    {
        Param param = new Param();

        param.Add("userId",userId);
        param.Add("noonsongId",noonsongId);
        param.Add("count",count);
        param.Add("location",location);

        return param;
    }
}

  