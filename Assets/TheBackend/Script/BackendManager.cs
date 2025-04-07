using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks; // [변경] async 기능을 이용하기 위해서는 해당 namepsace가 필요합니다.  

// 뒤끝 SDK namespace 추가
using BackEnd;

public class BackendManager : MonoBehaviour
{
    private UserDataManager userDataManager;
    void Start()
    {
        var bro = Backend.Initialize(); // 뒤끝 초기화

        // 뒤끝 초기화에 대한 응답값
        if (bro.IsSuccess())
        {
            Debug.Log("초기화 성공 : " + bro); // 성공일 경우 statusCode 204 Success
            string googlehash = Backend.Utils.GetGoogleHash();
            Debug.Log("구글 해시 키 : " + googlehash);
        }
        else
        {
            Debug.LogError("초기화 실패 : " + bro); // 실패일 경우 statusCode 400대 에러 발생
        } 
       

        //Test();

        //BackendChart.Instance.InitializeShopInfo(() =>
        //{
            //Debug.Log("차트 데이터 로드 완료 후 TuriShopManager 호출");
            //TuriShopManager.Instance.DisplayShopItems(); //테스트
        //});
    }

    // =======================================================
    // [추가] 동기 함수를 비동기에서 호출하게 해주는 함수(유니티 UI 접근 불가)
    // =======================================================
// <<<<<<< Updated upstream
//     async void Test()
//     {
        /*await Task.Run(() => {
            
            BackendLogin.Instance.CustomSignUp("user1", "1234"); // [추가] 뒤끝 회원가입 함수
            Debug.Log("테스트를 종료합니다.");
            
        });*/
        //BackendLogin.Instance.CustomLogin("user1", "1234"); // [추가] 뒤끝 로그인
        //BackendLogin.Instance.UpdateNickname("Noonsong"); // [추가] 닉네임 변겅
        //Debug.Log("테스트를 종료합니다.");

    // async void Test()
    // {
    //     await Task.Run(() => {
    //         BackendLogin.Instance.CustomSignUp("user2","1234");
    //         BackendLogin.Instance.CustomLogin("user2","1234");
    //         Debug.Log("차트를 가져옵니다.");

            
    //         BackendChart.Instance.ChartGet("157725");
    //         Debug.Log("테스트를 종료합니다.");
    //     });
    // }


    void Test(){
        //BackendLogin.Instance.CustomSignUp("User3","1234");

        //BackendLogin.Instance.CustomLogin("werwer","werwer");
        BackendLogin.Instance.CustomLogin("werwer","WERwer1111");
        //재화 평균
        GameStatistics.Instance.AverageSnowCount();
        //유저별 눈송이 수집율
        
        //눈송이별 수집율
        GameStatistics.Instance.UpdateNoonStatics();

        //Debug.Log("테스트를 종료합니다.");

    }
}