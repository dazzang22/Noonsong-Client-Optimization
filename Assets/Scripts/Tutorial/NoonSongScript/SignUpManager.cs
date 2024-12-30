using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BackEnd;

public class SignUpManager : MonoBehaviour
{
    public Toggle agreeToggle;
    public Button agreeButton;

    public InputField idInputField;
    public TMP_Text idResultText;
    public InputField passwordInputField;
    public TMP_Text passwordResultText;
    public InputField passwordConfirmInputField;
    public TMP_Text passwordConfirmResultText;
    public Button signUpButton;
    public GameObject privatePolicyPopup;
    public GameObject signUpPopup;
    public GameObject signUpCompletionPopup;

    private bool isIdValid = false;
    private bool isPasswordValid = false;
    private bool isPasswordsMatch = false;

    void Start()
    {
        SetupPrivacyAgreementToggle();
        SetupSignUpPage();
    }

    //개인정보수집처리 팝업 관련 초기화
    void SetupPrivacyAgreementToggle()
    {
        agreeButton.interactable = false;
        agreeToggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    //동의 체크박스 선택시 확인버튼 활성화
    void OnToggleValueChanged(bool isOn)
    {
        agreeButton.interactable = isOn;
    }

    //회원가입페이지 관련 초기화. 버튼 비활성화하고 입력 필드에 리스너 설정
    void SetupSignUpPage()
    {
        signUpButton.interactable = false;
        idInputField.onValueChanged.AddListener(OnIDFieldEndEdit);
        passwordInputField.onValueChanged.AddListener(OnPasswordFieldEndEdit);
        passwordConfirmInputField.onValueChanged.AddListener(OnPasswordConfirmFieldEndEdit);
        signUpButton.onClick.AddListener(OnSignUpButtonClicked);
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
        return Regex.IsMatch(id, @"^[a-z0-9]{6,20}$");
    }

    //아이디 유효 검사에 따라 텍스트 업데이트
    void UpdateIdResultText(string id)
    {
        if (!isIdValid)
        {
            idResultText.text = "아이디는 6자 이상 20자 이하로 영문 소문자와 숫자만 사용 가능합니다.";
            idResultText.color = Color.red;
        }
        else
        {
            idResultText.text = "사용 가능한 아이디입니다.";
            idResultText.color = Color.green;
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
        return Regex.IsMatch(password, @"^(?=.*\d).{6,}$");
    }

    //PW 유효 검사에 따라 텍스트 업데이트
    void UpdatePasswordResultText(string password)
    {
        if (!isPasswordValid)
        {
            passwordResultText.text = "비밀번호는 6자 이상, 숫자를 포함해야 합니다.";
            passwordResultText.color = Color.red;
        }
        else
        {
            passwordResultText.text = "사용 가능한 비밀번호입니다.";
            passwordResultText.color = Color.green;
        }
    }

    //PW2필드에서 입력 끝났을 때 PW1과 일치하는지 비교하고 확인문구 업데이트
    void OnPasswordConfirmFieldEndEdit(string confirmPassword)
    {
        isPasswordsMatch = passwordInputField.text == confirmPassword;
        UpdateConfirmPasswordResultText();
        CheckAllConditions();
    }

    //PW2 유효 검사에 따라 텍스트 업데이트
    void UpdateConfirmPasswordResultText()
    {
        if (!isPasswordsMatch)
        {
            passwordConfirmResultText.text = "비밀번호가 일치하지 않습니다.";
            passwordConfirmResultText.color = Color.red;
        }
        else
        {
            passwordConfirmResultText.text = "비밀번호가 일치합니다.";
            passwordConfirmResultText.color = Color.green;
        }
    }

    //아이디, PW1, PW2가 모두 유효한지 확인 후 계정생성 버튼 활성화
    void CheckAllConditions()
    {
        if (isIdValid && isPasswordValid && isPasswordsMatch)
        {
            signUpButton.interactable = true;
        }
        else
        {
            signUpButton.interactable = false;
        }
    }

    //회원가입 시행 후 팝업 띄움.
    public void OnSignUpButtonClicked()
    {
        string id = idInputField.text;
        string password = passwordInputField.text;

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
                idResultText.text = "이미 존재하는 아이디입니다.";
                idResultText.color = Color.red;
            }
            else
            {
                idResultText.text = "회원가입에 실패했습니다. 다시 시도해주세요.";
                idResultText.color = Color.red;
            }
        }
    }




}
