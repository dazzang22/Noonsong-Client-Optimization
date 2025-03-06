using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks; // [변경] async 기능을 이용하기 위해서는 해당 namepsace가 필요합니다.  

// 뒤끝 SDK namespace 추가
using BackEnd;

public class BackendSavePoint : MonoBehaviour
{
    private static BackendSavePoint _instance = null;

    public static BackendSavePoint Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new BackendSavePoint();
            }

            return _instance;
        }
    }

    public int LoadGameData()
    {
        Debug.Log("세이브포인트 로드");
        //PlayerPrefs.GetInt("TutorialStep", currentStep);
        int cur=UserDataManager.Instance.getSave();
        Debug.Log($"{cur}");
        return     cur;
    }


    public void SaveGameData(int point)
    {
        //PlayerPrefs.SetInt("TutorialStep", point);
        //PlayerPrefs.Save();
        Param param = new Param();
        param= UserDataManager.Instance.ChangeSave(point);
        UserDataManager.Instance.UpdateUserData(param);

        Debug.Log($"세이브포인트 저장: {point}");

    }


}