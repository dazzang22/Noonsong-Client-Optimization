using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BackEnd;
using System.Collections;

public class SignUpManager : MonoBehaviour
{
    //동의 관련
    public Toggle toggleAgree;
    public GameObject privatePolicyPopup;
    public TMP_Text textAgreeWarning;

    //회원가입 UI 관련
    public GameObject signUpPopup;
    public GameObject signUpCompletionPopup;

    // 입력 필드
    public TMP_InputField inputFieldID;
    public TMP_Text textIdResult;
    public TMP_InputField inputFieldPW;
    public TMP_Text textPWResult;
    public TMP_InputField inputFieldPWConfirm;
    public TMP_Text textPWConfirmResult;
    public TMP_InputField inputFieldEmail;
    public TMP_Text textEmailResult;
    public TMP_InputField inputFieldNickname;
    public TMP_Text textNicknameResult;
    public Button btnSignUp;

    private bool isIdValid = false;
    private bool isPasswordValid = false;
    private bool isPasswordsMatch = false;
    private bool isEmailValid = false;
    private bool isNicknameValid = false;

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
        inputFieldEmail.onValueChanged.AddListener(OnEmailFieldEndEdit);
        inputFieldNickname.onValueChanged.AddListener(OnNicknameFieldEndEdit);

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

        void OnEmailFieldEndEdit(string email)
    {
        isEmailValid = ValidateEmail(email);
        UpdateEmailResultText(email);
        CheckAllConditions();
    }

    bool ValidateEmail(string email)
    {
        return Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
    }

    void UpdateEmailResultText(string email)
    {
        if (!isEmailValid)
        {
            textEmailResult.text = "올바른 이메일 형식이 아닙니다.";
            textEmailResult.color = Color.red;
        }
        else
        {
            textEmailResult.text = "사용 가능한 이메일입니다.";
            textEmailResult.color = Color.green;
        }
    }

    void OnNicknameFieldEndEdit(string nickname)
    {
        isNicknameValid = nickname.Length >= 2 && nickname.Length <= 10;
        UpdateNicknameResultText();
        CheckAllConditions();
    }

    void UpdateNicknameResultText()
    {
        if (!isNicknameValid)
        {
            textNicknameResult.text = "닉네임은 2~10자여야 합니다.";
            textNicknameResult.color = Color.red;
        }
        else
        {
            textNicknameResult.text = "사용 가능한 닉네임입니다.";
            textNicknameResult.color = Color.green;
        }
    }

    //아이디, PW1, PW2가 모두 유효한지 확인 후 계정생성 버튼 활성화
    void CheckAllConditions()
    {
        if (isIdValid && isPasswordValid && isPasswordsMatch && isEmailValid && isNicknameValid)
        {
            btnSignUp.interactable = true;
        }
        else
        {
            btnSignUp.interactable = false;
        }
    }

    //회원가입 시행 후 팝업 띄움.
    public void OnSignUpButtonClicked()
    {
        /*if (toggleAgree) // 동의 체크 여부 확인
        {
            textAgreeWarning.text = "회원가입을 진행하려면 개인정보 제공에 동의해야 합니다.";
            textAgreeWarning.color = Color.red;
            textAgreeWarning.gameObject.SetActive(true);
            return;
        }*/

        string id = inputFieldID.text;
        string password = inputFieldPW.text;
        string email = inputFieldEmail.text;
        string nickname = inputFieldNickname.text;

        Debug.Log("회원가입 요청: ID=" + id + ", Email=" + email + ", Nickname=" + nickname);

        var bro = Backend.BMember.CustomSignUp(id, password);
        
        if (bro.IsSuccess())
        {
            Debug.Log("회원가입 성공! 자동 로그인 진행...");

            // 회원가입 후 자동 로그인 (이메일과 닉네임은 로그인 후 등록)
            BackendLogin.Instance.CustomLogin(id, password, () =>
            {
                BackendLogin.Instance.UpdateNickname(nickname);
                BackendLogin.Instance.UpdateEmail(email);
            });
            // UI 변경 (회원가입 완료 창 띄우기)
            privatePolicyPopup.SetActive(false);
                //signUpPopup.SetActive(false);
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
                textIdResult.text = "회원가입에 실패했습니다. 다시 시도해주세요." + bro;
                textIdResult.color = Color.red;
            }
        }

        
    }
    public void testfunc(){
        var bro = Backend.BMember.UpdateCustomEmail("help@thebackend.io");
        if (bro.IsSuccess())
        {
            Debug.Log("이메일 등록 성공");
        }
        else
        {
            Debug.LogError("이메일 등록 실패" + bro);
        }
    }
    public void testfunc2()
    {
        var bro = Backend.BMember.UpdateNickname("nickname");
        if (bro.IsSuccess())
        {
            Debug.Log("이메일 등록 성공");
        }
        else
        {
            Debug.LogError("이메일 등록 실패" + bro);
        }
    }
}
