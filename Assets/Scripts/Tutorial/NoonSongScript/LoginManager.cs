using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using BackEnd;
using TMPro;

public class LoginManager : MonoBehaviour
{
    public GameObject LoginView;
    public InputField inputField_ID;
    public InputField inputField_PW;
    public TMP_Text resultText;

    ButtonManager buttonManager;

    public Button loginButton;
    private string inputID = "";
    private string inputPW = "";

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

        var bro = Backend.BMember.CustomLogin(inputID, inputPW);

        if (bro.IsSuccess())
        {
            SceneManager.LoadScene("Merge-TutorialScene");
        }
        else
        {
            string googlehash = Backend.Utils.GetGoogleHash();

            Debug.Log("구글 해시 키 : " + googlehash);
            //resultText.text = $"로그인 실패: {bro.GetMessage()}";
            resultText.text = $"로그인 실패: {bro.GetMessage()},{googlehash}";

            resultText.color = Color.red;

        }
    }
}
