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
    public TMP_InputField inputFieldNickname;
    public TMP_Text textNicknameResult;

    public Button btnSignUp;

    private bool isIdValid = false;
    private bool isPasswordValid = false;
    private bool isPasswordsMatch = false;
    private bool isNicknameValid = false;

    public AudioSource audioSource;
    public AudioClip successSound;

    public EmailVerifyManager emailVerifyManager;


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
        inputFieldNickname.onValueChanged.AddListener(OnNicknameFieldEndEdit);

        toggleAgree.onValueChanged.AddListener(delegate{ CheckAllConditions(); });
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
        textIdResult.text = isIdValid
            ? "사용 가능한 아이디입니다."
            : "아이디는 5자 이상 15자 이하로 영문 소문자와 숫자만 사용 가능합니다.";
        textIdResult.color = isIdValid ? Color.green : Color.red;
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
        textPWResult.text = isPasswordValid
            ? "사용 가능한 비밀번호입니다."
            : "비밀번호는 8자 이상 12자 이하로, 대문자, 소문자, 숫자를 1개씩 포함해야 합니다.";
        textPWResult.color = isPasswordValid ? Color.green : Color.red;
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
        textPWConfirmResult.text = isPasswordsMatch
            ? "비밀번호가 일치합니다."
            : "비밀번호가 일치하지 않습니다.";
        textPWConfirmResult.color = isPasswordsMatch ? Color.green : Color.red;
    }

    void OnNicknameFieldEndEdit(string nickname)
    {
        isNicknameValid = nickname.Length >= 2 && nickname.Length <= 10;
        UpdateNicknameResultText();
        CheckAllConditions();
    }

    void UpdateNicknameResultText()
    {
        textNicknameResult.text = isNicknameValid
            ? "사용 가능한 닉네임입니다."
            : "닉네임은 2~10자여야 합니다.";
        textNicknameResult.color = isNicknameValid ? Color.green : Color.red;
    }

    //아이디, PW1, PW2가 모두 유효한지 확인 후 계정생성 버튼 활성화
    void CheckAllConditions()
    {
        btnSignUp.interactable =
            isIdValid && isPasswordValid && isPasswordsMatch &&
            isNicknameValid && toggleAgree.isOn && emailVerifyManager.isEmailVerified();
    }

    //회원가입 시행 후 팝업 띄움.
    public void OnSignUpButtonClicked()
    {
        if (!toggleAgree.isOn)
        {
            textAgreeWarning.text = "회원가입을 진행하려면 개인정보 제공에 동의해야 합니다.";
            textAgreeWarning.color = Color.red;
            textAgreeWarning.gameObject.SetActive(true);
            return;
        }

        string id = inputFieldID.text;
        string password = inputFieldPW.text;
        string email = emailVerifyManager.GetUserEmail();
        string nickname = inputFieldNickname.text;

        // 닉네임 중복 확인 후 진행
        /*var nicknameCheck = Backend.BMember.CheckNicknameDuplication(nickname);
        if (!nicknameCheck.IsSuccess())
        {
            if (nicknameCheck.GetErrorCode() == "DuplicatedParameterException")
            {
                textNicknameResult.text = "이미 사용 중인 닉네임입니다.";
                textNicknameResult.color = Color.red;
                return;
            }
            else
            {
                textNicknameResult.text = "닉네임 확인 중 오류 발생. 다시 시도해주세요.: " + nicknameCheck.GetMessage();
                textNicknameResult.color = Color.red;
                return;
            }
        }*/

        Debug.Log("회원가입 요청: ID=" + id + ", Email=" + email + ", Nickname=" + nickname);

        BackendLogin.Instance.CustomSignUp(id, password,nickname,email);

        if (BackendLogin.Instance.signup_static != 1)
        {
            textIdResult.text = BackendLogin.Instance.su_error == "DuplicatedParameterException"
                ? "이미 존재하는 아이디입니다."
                : "회원가입에 실패했습니다. 다시 시도해주세요.";
            textIdResult.color = Color.red;
            return;
        }

        Debug.Log("회원가입 성공! 자동 로그인 진행...");
        BackendLogin.Instance.CustomLogin(id, password);

        if (BackendLogin.Instance.login_static != 1)
        {
            HandleFailure("자동 로그인 실패");
            return;
        }

        Debug.Log("자동 로그인 성공");
        CheckNicknameAndProceed(nickname, email);
    }

    private void CheckNicknameAndProceed(string nickname, string email)
    {
        var nicknameCheck = Backend.BMember.CheckNicknameDuplication(nickname);

        if (!nicknameCheck.IsSuccess())
        {
            string errorMessage = nicknameCheck.GetErrorCode() == "DuplicatedParameterException"
                ? "이미 사용 중인 닉네임입니다."
                : $"닉네임 확인 중 오류 발생. 다시 시도해주세요.: {nicknameCheck.GetMessage()}";

            textNicknameResult.text = errorMessage;
            textNicknameResult.color = Color.red;
            BackendLogin.Instance.Logout();
            return;
        }

        Debug.Log("닉네임 중복 확인 성공");
        HandleSignUpSuccess(nickname, email);
    }

    private void HandleSignUpSuccess(string nickname, string email)
    {
        BackendLogin.Instance.UpdateNickname(nickname);
        if (BackendLogin.Instance.updateNickname_static != 1)
        {
            HandleFailure("닉네임 업데이트 실패");
            return;
        }

        Debug.Log("닉네임 업데이트 성공");
        BackendLogin.Instance.UpdateEmail(email);
        if (BackendLogin.Instance.updateEmail_static != 1)
        {
            HandleFailure("이메일 업데이트 실패");
            return;
        }

        Debug.Log("이메일 업데이트 성공");
        SignupSuccess();
    }

    private void HandleFailure(string message)
    {
        Debug.LogError(message);
        BackendLogin.Instance.Logout();
    }

    public void SignupSuccess(){
        //BackendLogin.Instance.CustomLogin(id, password);
        audioSource.PlayOneShot(successSound);
        privatePolicyPopup.SetActive(false);
        signUpPopup.SetActive(false);
        signUpCompletionPopup.SetActive(true);

        //DB 연결
        UserDataManager.Instance.SetSEmail(emailVerifyManager.GetUserEmail());
    }
}
