using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BackEnd;

public class SignUpManager : MonoBehaviour
{
    //동의 관련
    public Toggle toggleAgree;
    public GameObject privatePolicyPopup;
    public TMP_Text textAgreeWarning;

    //회원가입 UI 관련
    public GameObject signUpPopup;
    //public TMP_Text signUpTitle;
    //public ScrollRect scrollViewSignUp;
    public GameObject signUpCompletionPopup;
    public TMP_Text asteriskId;
    public TMP_Text asteriskPW;
    public TMP_Text asteriskPWConfirm;


    //입력 필드
    public InputField inputFieldID;
    public TMP_Text textIdResult;
    public InputField inputFieldPW;
    public TMP_Text textPWResult;
    public InputField inputFieldPWConfirm;
    public TMP_Text textPWConfirmResult;
    public Button btnSignUp;

    private bool isIdValid = false;
    private bool isPasswordValid = false;
    private bool isPasswordsMatch = false;
    private bool isAgreeChecked = false;

    void Start()
    {
        SetupSignUpPage();
    }
    //회원가입페이지 관련 초기화. 버튼 비활성화하고 입력 필드에 리스너 설정
    void SetupSignUpPage()
    {
        btnSignUp.interactable = false;

        inputFieldID.onValueChanged.AddListener(OnIDFieldEndEdit);
        inputFieldPW.onValueChanged.AddListener(OnPasswordFieldEndEdit);
        inputFieldPWConfirm.onValueChanged.AddListener(OnPasswordConfirmFieldEndEdit);

        inputFieldID.onValueChanged.AddListener(OnIDFieldChanged);
        inputFieldPW.onValueChanged.AddListener(OnPasswordFieldChanged);
        inputFieldPWConfirm.onValueChanged.AddListener(OnPasswordConfirmFieldChanged);

        btnSignUp.onClick.AddListener(OnSignUpButtonClicked);
    }

    //ID필드에서 입력 끝났을 때 아이디 유효한지 확인하고, 확인 문구 업데이트
    void OnIDFieldEndEdit(string id)
    {
        isIdValid = ValidateId(id);
        UpdateIdResultText(id);
        CheckAllConditions();
    }

    //유효 아이디 조건: 길이 6자 이상 20자 이하, 영문 소문자와 숫자만
    bool ValidateId(string id)
    {
        return Regex.IsMatch(id, @"^[a-z0-9]{5,15}$");
    }

    //아이디 유효 검사에 따라 텍스트 업데이트
    void UpdateIdResultText(string id)
    {
        if (!isIdValid)
        {
            textIdResult.text = "아이디는 5자 이상 15자 이하로 영문 소문자와 숫자만 사용 가능합니다.";
            textIdResult.color = Color.red;
        }
        else
        {
            textIdResult.text = "사용 가능한 아이디입니다.";
            textIdResult.color = Color.green;
        }
    }

    //PW1필드에서 입력 끝났을 때 PW 유효한지 확인하고, 확인 문구 업데이트
    void OnPasswordFieldEndEdit(string password)
    {
        isPasswordValid = ValidatePassword(password);
        UpdatePasswordResultText(password);
        CheckAllConditions();
    }

    //유효 PW 조건: 6자 이상, 숫자 포함
    bool ValidatePassword(string password)
    {
        return Regex.IsMatch(password, @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)[A-Za-z\d]{8,12}$");

    }

    //PW 유효 검사에 따라 텍스트 업데이트
    void UpdatePasswordResultText(string password)
    {
        if (!isPasswordValid)
        {
            textPWResult.text = "비밀번호는 8자 이상 12자 이하로, 대문자, 소문자, 숫자를 1개씩 포함해야 합니다.";
            textPWResult.color = Color.red;
        }
        else
        {
            textPWResult.text = "사용 가능한 비밀번호입니다.";
            textPWResult.color = Color.green;
        }
    }

    //PW2필드에서 입력 끝났을 때 PW1과 일치하는지 비교하고 확인문구 업데이트
    void OnPasswordConfirmFieldEndEdit(string confirmPassword)
    {
        isPasswordsMatch = inputFieldPW.text == confirmPassword;
        UpdateConfirmPasswordResultText();
        CheckAllConditions();
    }

    //PW2 유효 검사에 따라 텍스트 업데이트
    void UpdateConfirmPasswordResultText()
    {
        if (!isPasswordsMatch)
        {
            textPWConfirmResult.text = "비밀번호가 일치하지 않습니다.";
            textPWConfirmResult.color = Color.red;
        }
        else
        {
            textPWConfirmResult.text = "비밀번호가 일치합니다.";
            textPWConfirmResult.color = Color.green;
        }
    }

    //아이디, PW1, PW2가 모두 유효한지 확인 후 계정생성 버튼 활성화
    void CheckAllConditions()
    {
        if (isIdValid && isPasswordValid && isPasswordsMatch)
        {
            btnSignUp.interactable = true;
        }
        else
        {
            btnSignUp.interactable = false;
        }
    }

    //필수표시 (*) 활성화   
    void OnIDFieldChanged(string text)
    {
        asteriskId.gameObject.SetActive(string.IsNullOrEmpty(text));
    }

    void OnPasswordFieldChanged(string text)
    {
        asteriskPW.gameObject.SetActive(string.IsNullOrEmpty(text));
    }

    void OnPasswordConfirmFieldChanged(string text)
    {
        asteriskPWConfirm.gameObject.SetActive(string.IsNullOrEmpty(text));
    }

    //회원가입 시행 후 팝업 띄움.
    public void OnSignUpButtonClicked()
    {
        if (!isAgreeChecked) // 동의 체크 여부 확인
        {
            textAgreeWarning.text = "회원가입을 진행하려면 개인정보 제공에 동의해야 합니다.";
            textAgreeWarning.color = Color.red;
            textAgreeWarning.gameObject.SetActive(true);
            return;
        }

        string id = inputFieldID.text;
        string password = inputFieldPW.text;

        var bro = Backend.BMember.CustomSignUp(id, password);

        if (bro.IsSuccess())
        {
            privatePolicyPopup.SetActive(false);
            signUpPopup.SetActive(false);
            signUpCompletionPopup.SetActive(true);
        }
        else
        {
            if (bro.GetErrorCode() == "DuplicatedParameterException")
            {
                textIdResult.text = "이미 존재하는 아이디입니다.";
                textIdResult.color = Color.red;
            }
            else
            {
                textIdResult.text = "회원가입에 실패했습니다. 다시 시도해주세요.";
                textIdResult.color = Color.red;
            }
        }
    }
}
