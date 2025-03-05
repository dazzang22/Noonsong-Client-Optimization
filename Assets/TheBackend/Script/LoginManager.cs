using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using BackEnd;
using TMPro;

public class LoginManager : MonoBehaviour
{
    public GameObject LoginView;
    public TMP_InputField inputField_ID;
    public TMP_InputField inputField_PW;
    public TMP_Text resultText;

    ButtonManager buttonManager;

    public Button loginButton;
    private string inputID = "";
    private string inputPW = "";

    public AudioSource audioSource;  // 오디오 소스
    public AudioClip successSound;


    private void Start()
    {
        inputField_ID.onValueChanged.AddListener(OnIDFieldEndEdit);
        inputField_PW.onValueChanged.AddListener(OnPasswordFieldEndEdit);

        loginButton.onClick.AddListener(TryLogin);

        buttonManager = FindObjectOfType<ButtonManager>();
    }

    private void OnIDFieldEndEdit(string input)
    {
        // ID 입력 완료 시 동작이 필요하다면 이곳에 작성
    }

    private void OnPasswordFieldEndEdit(string input)
    {
        // 비밀번호 입력 완료 시 동작이 필요하다면 이곳에 작성
    }

    private void TryLogin()
    {
        inputID = inputField_ID.text;
        inputPW = inputField_PW.text;

        BackendLogin.Instance.CustomLogin(inputID, inputPW);
        //BackendSavePoint.Instance.LoadGameData();

        if (BackendLogin.Instance.login_static==0)
        {
            //resultText.text = $"로그인 실패: {bro.GetMessage()}";
            resultText.text = "로그인 실패";

            resultText.color = Color.red;
        }
        else
        {
            audioSource.PlayOneShot(successSound);

        }
    }
}
