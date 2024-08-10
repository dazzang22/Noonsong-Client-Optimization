using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    //로그인 화면 Root 
    public GameObject LoginView;

    public InputField inputField_ID;
    public InputField inputField_PW;
    public Button Button_Login;

    //Test를 위해 임의로 사용자 아이디랑 비번을 추가했음
    private string user = "NoonSong";
    private string password = "1906";

    /// <summary>
    /// 우측 하단의 하늘색 로그인 버튼 클릭시 실행
    /// </summary>
    public void LoginButtonClick()
    {
        if (inputField_ID.text == user && inputField_PW.text == password)
        {
            Debug.Log("로그인 성공!");
            //로그인 성공시 Main Scene 로드
            SceneManager.LoadScene("MainScene"); // replace "MainScene" 으로 이동
        }
        else
        {
            Debug.Log("로그인 실패");
        }
    }
}