using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks; // [변경] async 기능을 이용하기 위해서는 해당 namepsace가 필요합니다.  

// 뒤끝 SDK namespace 추가
using BackEnd;

public class BackendSavePoint : MonoBehaviour
{
    private int currentStep=0;
    void Start()
    {
        LoadGameData();
        
       
    }

    public void LoadGameData()
    {
        currentStep = PlayerPrefs.GetInt("TutorialStep", 0);
        Debug.Log($"저장된 튜토리얼 단계: {currentStep}"); 
    }


    public void SaveGameData()
    {
        //currentStep = PlayerPrefs.SetInt("TutorialStep", 0);
        PlayerPrefs.Save();

    }


}