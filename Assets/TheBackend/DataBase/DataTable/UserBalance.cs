using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using BackEnd;

public class UserBalance 
{
    public string userId;
    public int snowCount=0;

    public UserBalance()
    {

    }

    public UserBalance(LitJson.JsonData json)
    {
        this.userId=json["userId"].ToString();
        this.snowCount=int.Parse(json["snowCount"].ToString());
    }

    public void setuserBalance(string userid, int snowcount=0)
    {
        this.userId=userid;
        this.snowCount=snowcount;
    }

    public void setBalance(int snowcount)
    {
        this.snowCount=snowcount;
    }

    public int getBalance()
    {
        return this.snowCount;
    }

    public override string ToString()
    {
        StringBuilder result = new StringBuilder();
        result.AppendLine($"userId: {userId}");
        result.AppendLine($"snowCount: {snowCount}");


        return result.ToString();
    }

    public Param ToParam()
    {
        Param param = new Param();

        param.Add("userId",userId);
        param.Add("snowCount",snowCount);

        return param;
    }
}

  