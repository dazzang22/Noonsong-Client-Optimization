using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public GameObject LoginView;
    public InputField inputField_ID;
    public InputField inputField_PW;

    ButtonManager buttonManager;
    private string inputID = "";
    private string inputPW = "";

    private string user = "NoonSong";
    private string password = "1906";

    private void Start()
    {
        inputField_ID.onEndEdit.AddListener(OnIDFieldEndEdit);
        inputField_PW.onEndEdit.AddListener(OnPasswordFieldEndEdit);

        buttonManager = FindObjectOfType<ButtonManager>();
    }

    private void OnIDFieldEndEdit(string input)
    {
        // ID 입력 완료 시 동작이 필요하다면 이곳에 작성
    }

    private void OnPasswordFieldEndEdit(string input)
    {
        TryLogin();
    }

    private void TryLogin()
    {
        inputID = inputField_ID.text;
        inputPW = inputField_PW.text;

        if (inputID == user && inputPW == password)
        {
            Debug.Log("로그인 성공!");
            buttonManager.GoScene(); // 씬 전환
        }
        else
        {
            Debug.Log("로그인 실패");
        }
    }
}