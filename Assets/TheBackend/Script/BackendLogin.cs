using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 뒤끝 SDK namespace 추가
using BackEnd;
using UnityEngine.SceneManagement;

public class BackendLogin
{
    private static BackendLogin _instance = null;

    public int login_static=0;
    public int signup_static=0;
    public string su_error="";

    public static BackendLogin Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BackendLogin();
            }

            return _instance;
        }
    }

    public void CustomSignUp(string id, string pw,string nick,string email)
    {
        // Step 2. 회원가입 구현하기 로직

        Debug.Log("회원가입을 요청합니다.");

        var bro = Backend.BMember.CustomSignUp(id,pw);
        UserDataManager.Instance.InsertUserData(id,pw,nick,email);
        //지도  
        UserMapManager.Instance.insertUserMap(id);
        //재화
        UserBalanceManager.Instance.InsertUserBalance(id,0);

        if (bro.IsSuccess())
        {
            Debug.Log("회원가입에 성공했습니다. : " + bro);
            signup_static=1;
            su_error=null;
        }
        else
        {
            Debug.LogError("회원가입에 실패했습니다. : " + bro);
            signup_static=0;
            su_error=bro.GetErrorCode();
        }
    }

    public void CustomLogin(string id, string pw)
    {
        // Step 3. 로그인 구현하기 로직

        Debug.Log("로그인을 요청합니다.");

        var bro = Backend.BMember.CustomLogin(id, pw);


        if (bro.IsSuccess())
        {
            Debug.Log("로그인이 성공했습니다. : " + bro);
            SceneManager.LoadScene("Merge-TutorialScene");
            login_static=1;
        }
        else
        {
            Debug.LogError("로그인이 실패했습니다. : " + bro);
            login_static=0;
        }
    }

    public void UpdateNickname(string nickname)
    {
        // Step 4. 닉네임 변경 구현하기 로직

        Debug.Log("닉네임 변경을 요청합니다.");

        var bro = Backend.BMember.UpdateNickname(nickname);

        if (bro.IsSuccess())
        {
            Debug.Log("닉네임 변경에 성공했습니다 : " + bro);
            Param newparam=UserDataManager.Instance.ChangeNickname(nickname);
            UserDataManager.Instance.UpdateUserData(newparam);
        }
        else
        {
            Debug.LogError("닉네임 변경에 실패했습니다 : " + bro);
        }
    }

    public void UpdateEmail(string email)
    {       
        //Backend.BMember.LoginWithTheBackendToken();

        var bro = Backend.BMember.UpdateCustomEmail(email);
            
            if (bro.IsSuccess())
            {
                Debug.Log("이메일 등록 성공");
                Param newparam=UserDataManager.Instance.ChangeEmail(email);
                UserDataManager.Instance.UpdateUserData(newparam);
                Logout(); // 이메일 등록 후 로그아웃
            }
            else
            {
                Debug.LogError("이메일 등록 실패");
                Logout(); // 실패해도 로그아웃 실행
            }
                
    }

    //비밀번호 수정 기능 수정해야함
    public void UpdatePassword(string id,string email)
    {
        var bro = Backend.BMember.ResetPassword(id,email);
            
        if (bro.IsSuccess())
        {
            Debug.Log("이메일로 초기화 비번 전송 성공");
            //Backend.BMember.UpdatePassword()
            Logout(); // 이메일 등록 후 로그아웃
        }
        else
        {
            Debug.LogError("이메일 등록 실패");
            Logout(); // 실패해도 로그아웃 실행
        }
    }

    

    public void Logout()
    {
        Debug.Log("로그아웃 진행...");
        Backend.BMember.Logout();
        Debug.Log("로그아웃 완료.");
    }
}