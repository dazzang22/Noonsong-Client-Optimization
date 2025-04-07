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

    public Button letsGoButton;
    public Button loginButton;

    private string inputID = "";
    private string inputPW = "";

    public AudioSource audioSource; 
    public AudioClip successSound;


    private void Start()
    {
        inputField_ID.onValueChanged.AddListener(OnIDFieldEndEdit);
        inputField_PW.onValueChanged.AddListener(OnPasswordFieldEndEdit);

        loginButton.onClick.AddListener(TryLogin);
        letsGoButton.onClick.AddListener(TryAutoLogin);

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

        if (BackendLogin.Instance.login_static==0)
        {
            resultText.text = " 아이디 또는 비밀번호를 다시 확인하세요.";
            resultText.color = Color.red;
        }
        else
        {
            audioSource.PlayOneShot(successSound);
            GoScene();

        }
    }

    private void TryAutoLogin()
    {
        BackendReturnObject bro = Backend.BMember.LoginWithTheBackendToken();
        if (bro.IsSuccess())
        {
            Debug.Log("자동 로그인 성공");
            audioSource.PlayOneShot(successSound);
            GoScene();
        }
        else
        {
            Debug.Log($"자동 로그인 실패: {bro.GetMessage()}");
            LoginView.SetActive(true);
        }
    }

    public void GoScene()
    {
        if (PlayerPrefs.GetInt("TutorialCompleted", 0) == 1)
        {
            // 튜토리얼이 완료되었으므로 main Scene으로 바로 이동
            SceneManager.LoadScene("MainScene(Release)");
        }
        else
        {
            SceneManager.LoadScene("Merge-TutorialScene");
        }
        
    }
}
