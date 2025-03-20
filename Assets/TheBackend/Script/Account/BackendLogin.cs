using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 뒤끝 SDK namespace 추가
using BackEnd;
using UnityEngine.SceneManagement;

public class BackendLogin
{
    private static BackendLogin _instance = null;

    public int login_static=0;
    public int signup_static=0;
    public int updateNickname_static=0;
    public int updateEmail_static=0;
    public string su_error="";
    //SignUpManager signUpManager = GameObject.Find("SignupManager").GetComponent<SignUpManager>();

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
        Debug.Log("유저 테이블 로드.");
        UserDataManager.Instance.InsertUserData(id,pw,nick,email);
        //지도
        Debug.Log("지도 테이블 로드.");  
        UserMapManager.Instance.insertUserMap(id);
        //재화
        Debug.Log("재화 테이블 로드.");
        UserBalanceManager.Instance.InsertUserBalance(id,15);
        //BackendSavePoint.Instance.SaveGameData(0);


        if (bro.IsSuccess())
        {
            Debug.Log("회원가입에 성공했습니다. : " + bro);
            BackendSavePoint.Instance.SaveGameData(0);
            signup_static =1;
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
       //serDogamManager.Instance.First();

        if (bro.IsSuccess())
        {
            Debug.Log("로그인이 성공했습니다. : " + bro);
            //SceneManager.LoadScene("Merge-TutorialScene");
            UserDogamManager.Instance.First();
            login_static =1;
        }
        else
        {
            Debug.LogError("로그인이 실패했습니다. : " + bro);
            login_static=0;
        }
    }

    public void UpdateNickname(string nickname)
    {
    //     // Step 4. 닉네임 변경 구현하기 로직

    //     Debug.Log("닉네임 변경을 요청합니다.");
    //     var bro = Backend.BMember.CheckNicknameDuplication(nickname);
    //     if (bro.IsSuccess())
    //     {
    //         Debug.Log("닉네임 중복체크 성공");
    //         bro = Backend.BMember.UpdateNickname(nickname);
    //         if (bro.IsSuccess())
    //         {
    //             Debug.Log("닉네임 변경에 성공했습니다 : " + bro);
    //             Param newparam = UserDataManager.Instance.ChangeNickname(nickname);
    //             updateNickname_static=1;
    //             UserDataManager.Instance.UpdateUserData(newparam);
    //         }
    //         else
    //         {
    //             Debug.LogError("닉네임 변경에 실패했습니다 : " + bro);
    //             signUpManager.textNicknameResult.text = "닉네임 변경 실패" + bro;
    //             signUpManager.textNicknameResult.color = Color.red;
    //         }
    //     }
    //     else if (bro.GetStatusCode() == "409")
    //     {
    //         Debug.LogError("중복된 닉네임");
    //         signUpManager.textNicknameResult.text = "중복된 닉네임 입니다.";
    //         signUpManager.textNicknameResult.color = Color.red;
    //     }
    //     else
    //     {
    //         Debug.LogError("닉네임이 형식이 맞지 않음");
    //         signUpManager.textNicknameResult.text="닉네임에 공백이 존재합니다.";
    //         signUpManager.textNicknameResult.color = Color.red;
    //         return;
    //     }

        
    }

    public void UpdateEmail(string email)
    {       
        //Backend.BMember.LoginWithTheBackendToken();

        var bro = Backend.BMember.UpdateCustomEmail(email);
            
            if (bro.IsSuccess())
            {
                Debug.Log("이메일 등록 성공");
                UserDataManager.Instance.changeEmail(email);
                updateEmail_static=1;
            }
            else
            {
                Debug.LogError("이메일 등록 실패" + bro);
                
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
            Debug.LogError("이메일 등록 실패" + bro);
            Logout(); // 실패해도 로그아웃 실행
        }
    }

    

    public void Logout()
    {
        Debug.Log("로그아웃 진행...");
        Backend.BMember.Logout();
        Debug.Log("로그아웃 완료.");
    }

    /*public int GetServerHour()
    {
        BackendReturnObject serverTime = Backend.Utils.GetServerTime();

        string time = serverTime.GetReturnValuetoJSON()["utcTime"].ToString();
        DateTime parsedDate = DateTime.Parse(time);
        int hour = parsedDate.Hour;

        //Debug.Log($"현재 시각: {hour}시");
        return hour;
    }*/

}