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
    private int currentStep=0;
    //private GPSManager gpsManager;

    void Start()
    {
        //currentStep=gpsManager.currentIndex;
       
    }

    public void LoadGameData()
    {
        Debug.Log("세이브포인트 로드");
        PlayerPrefs.GetInt("TutorialStep", currentStep);
        Debug.Log($"저장된 튜토리얼 단계: {currentStep}"); 
    }


    public void SaveGameData(int point)
    {
        Debug.Log("세이브포인트 저장");
        PlayerPrefs.SetInt("TutorialStep", point);
        PlayerPrefs.Save();
        UserDataManager.Instance.ChangeSave(point);
        Debug.Log($"세이브포인트 저장: {point}");

    }


}