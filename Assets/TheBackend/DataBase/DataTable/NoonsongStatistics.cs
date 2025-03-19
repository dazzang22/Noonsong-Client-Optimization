using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using BackEnd;

public class NoonsongStatistics 
{
    public string noonsongName;
    public int num;

    //public bool
    public NoonsongStatistics()
    {

    }

    public NoonsongStatistics(LitJson.JsonData json)
    {
        this.noonsongName=json["noonsongName"].ToString();
        this.num=int.Parse(json["num"].ToString());
        
    }
    public void setNoonsongStatistics(string noonsongName, int num)
    {
        this.noonsongName=noonsongName;
        this.num=num;
    }

    public void updateNoonNum(int num)
    {
        this.num= num;
    }
    
    public int getnoonNum()
    {
        return this.num;
    }


    public override string ToString()
    {
        StringBuilder result = new StringBuilder();
        result.AppendLine($"noonsongName: {noonsongName}");
        result.AppendLine($"num: {num}");
        return result.ToString();
    }

    public Param ToParam()
    {
        Param param = new Param();

        param.Add("noonsongName",noonsongName);
        param.Add("num",num);

        return param;
    }
}

  