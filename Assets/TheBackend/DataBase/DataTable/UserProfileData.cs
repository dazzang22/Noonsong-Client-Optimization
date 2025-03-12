using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;

using BackEnd;

public class UserProfileData
{
    public string inDate; //데이터가 차트에 들어온 날짜
    public string owner_inDate=Backend.UserInDate; //유저 첫 등록 날짜 
    public DateTime updatedAt; // 수정 날짜
    public string client_date;
    //아래에서부터 UserProfile 테이블에 추가되는 데이터.
    public string userId;
    public string password;
    public string nickname=Backend.UserNickName;
    public string sookmyungMail;
    public string mail;
    public int saveTutorial;
    public int sendingTime=0; // 파견하기 잔여시간간

    public UserProfileData()
    {

    }
    
    public UserProfileData(LitJson.JsonData json)
    {
        Debug.Log(json.ToJson());
        this.owner_inDate=json["inDate"].ToString();
        this.updatedAt = DateTime.Parse(json["inDate"].ToString());
        this.sookmyungMail=null;
        this.nickname="none";
    }
    public void setUser(LitJson.JsonData json)
    {
        this.userId=json["userId"].ToString();
        this.password=json["password"].ToString();
        this.owner_inDate=json["owner_inDate"].ToString();
        this.nickname =json["nickname"].ToString();
        this.updatedAt = DateTime.Parse(json["updatedAt"].ToString());
        this.sookmyungMail=json["sookmyungMail"].ToString();
        this.sookmyungMail=json["mail"].ToString();
        this.saveTutorial=int.Parse(json["saveTutorial"].ToString());
        this.sendingTime=int.Parse(json["sendingTime"].ToString());
        
    }

    public void setUserProfile(string id,string pw,string nick, string email)
    {
        this.userId=id;
        this.password=pw;
        this.nickname=nick;
        this.mail=email;
        this.saveTutorial=0;
        this.sendingTime=0;
    }
    public void setNickname(string newnick)
    {
        this.nickname=newnick;
    }
    public void setSMail(string smail)
    {
        this.sookmyungMail= smail;
    }
    public void setMail(string mail)
    {
        this.mail= mail;
    }
    public void setPW(string pw)
    {
        this.password=pw;
    }
    public void setSave(int point)
    {
        this.saveTutorial=point;
    }
    public void setSendingTime(int time)
    {
        this.sendingTime=time;
    }


    public string getuserId()
    {
        return this.userId;
    }

    public int getSave()
    {
        return this.saveTutorial;
    }

    public string getUserNickname()
    {
        return this.nickname;
    }
    public int getSendingTime()
    {
        return this.sendingTime;
    }
 

    public override string ToString()
    {
        StringBuilder result = new StringBuilder();
        result.AppendLine($"userId: {userId}");
        result.AppendLine($"password: {password}");
        result.AppendLine($"nickname: {nickname}");
        result.AppendLine($"owner_inDate: {owner_inDate}");
        result.AppendLine($"sookmyungMail: {sookmyungMail}");
        result.AppendLine($"mail: {mail}");
        result.AppendLine($"saveTutorial: {saveTutorial}");
        result.AppendLine($"sendingTime: {sendingTime}");


        return result.ToString();
    }

    public Param ToParam()
    {
        Param param = new Param();

        param.Add("userId",userId);
        param.Add("nickname", nickname);
        param.Add("password", password);
        param.Add("sookmyungMail", sookmyungMail);
        param.Add("mail",mail);
        param.Add("saveTutorial", saveTutorial);
        param.Add("sendingTime", sendingTime);


        return param;
    }
}

  