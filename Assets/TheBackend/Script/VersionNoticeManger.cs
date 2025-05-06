using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using BackEnd;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using LitJson;
using System;



public class VersionNoticeManger  
{   
    //public GameObject noticePopupUI; //공지 팝업
    private string title="공지";
    public string newText; // 안내 text

    public bool ispopup=false;
    public bool isversion=false;

    private static VersionNoticeManger _instance = null;

    public static VersionNoticeManger Instance { get; } = new VersionNoticeManger();

    public void SetMaintenanceNotice(string message,bool ispopup,bool isversion)
    {
        title = "공지";
        newText = message;
        this.ispopup = ispopup;
        this.isversion=isversion;
    }

    //버전 관리
    public void VersionCheck()
    {
        var bro = Backend.Utils.GetLatestVersion();

        if(!bro.IsSuccess())
        {
            Debug.LogError("버전 정보를 조회하는데 실패하였습니다.\n"+bro);
            ispopup=false;
            return;
        }
        
        string version = bro.GetReturnValuetoJSON()["version"].ToString();
        Debug.Log($"{version},{Application.version}");

        // 최신버전이 현재 기기에 저장된 버전이라면
        if(version  == Application.version) {
            ispopup=false;
            isversion=false;
            return;
        }
        Version  client = new Version(Application.version);
        Version server = new Version(version);
        if(server>client)
        {
            //현재 앱의 버전과 버전관리에서 설정한 버전이 맞지 않을 경우
            string forceUpdate = bro.GetReturnValuetoJSON()["type"].ToString();
            if(forceUpdate == "1") {
                SetMaintenanceNotice("현재 사용 중인 앱 버전은 더 이상 지원되지 않습니다.\n스토어에서 업데이트 후 이용해 주세요.",false,true);
                //newText = "현재 사용 중인 앱 버전은 더 이상 지원되지 않습니다.\n스토어에서 업데이트 후 이용해 주세요.";
                Debug.Log("업데이트를 하시겠습니까? y/n");
                //ispopup=true;
            }
            else if(forceUpdate == "2") {
                Debug.Log("최신 버전이 존재합니다. 업데이트를 진행해 주세요.");
                SetMaintenanceNotice("최신버전이 존재합니다. \n업데이트를 진행해주세요.",false,true);
                //ispopup=true;
            }
        }
    }
    //공지 관리
    public void  NoticeCheck()
    {
        string tempNotice = Backend.Notice.GetTempNotice();

        if(string.IsNullOrEmpty(tempNotice)) {
            ispopup=false;
            return;
        }

        LitJson.JsonData data = JsonMapper.ToObject(tempNotice);
        if(bool.Parse(data["isUse"].ToString())) {
            Debug.Log(data["contents"].ToString());
            SetMaintenanceNotice(data["contents"].ToString(),true,false);
            Debug.Log(newText);
            //ispopup=true;
        }
    }
    

}