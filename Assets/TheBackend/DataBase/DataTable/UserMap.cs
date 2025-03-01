using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using BackEnd;

public class UserMap 
{
    public string userId;
    public List<int> validateMap = new List<int>();

    //public bool
    public UserMap()
    {

    }

    public UserMap(LitJson.JsonData json)
    {
        this.userId=json["userId"].ToString();
        for(int i = 0; i < json["validateMap"].Count; i++)
        {
            this.validateMap.Add(int.Parse(json["validateMap"][i].ToString()));
        }
        //lastUpdate = DateTime.Parse(json["lastUpdate"].ToString());
    }
    public void setUserValidateMap(string userId, List<int> validateMap=null)
    {
        this.userId=userId;
        if(validateMap != null)
        {
            for(int i = 0; i < validateMap.Count; i++)
            {
                this.validateMap.Add(validateMap[i]);
            }
        }
    }

    //validate 된 구역 추가
    public void addValidateMap(int validateMap)
    {
        this.validateMap.Add(validateMap);
    }

    //validate 된 maplist 반환
    public List<int> getValidateMap()
    {
        return validateMap;
    }

    public override string ToString()
    {
        StringBuilder result = new StringBuilder();
        result.AppendLine($"userId: {userId}");
        foreach(var map in validateMap)
        {
            result.AppendLine($"validateMap: {map}");

        }
        return result.ToString();
    }

    public Param ToParam()
    {
        Param param = new Param();

        param.Add("userId",userId);
        param.Add("validateMap",validateMap);

        return param;
    }
}

  