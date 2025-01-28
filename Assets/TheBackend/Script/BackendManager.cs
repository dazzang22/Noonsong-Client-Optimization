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
        }
        else
        {
            Debug.LogError("초기화 실패 : " + bro); // 실패일 경우 statusCode 400대 에러 발생
        }

        Test();
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
    
        BackendLogin.Instance.CustomLogin("User2","1234");
        //Debug.Log(Backend.UserInDate);
        //BackendLogin.Instance.UpdateNickname("User3Change");

        UserDataManager.Instance.getAllUsersList();
  
        Debug.Log("테스트를 종료합니다.");
    }
}