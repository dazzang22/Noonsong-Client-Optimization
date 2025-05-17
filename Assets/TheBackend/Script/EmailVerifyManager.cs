using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Mail;
using System;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using System.Collections;

public class EmailVerifyManager : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField codeInput;

    public TMP_Text emailMessageText;
    public TMP_Text codeMessageText;

    public Button requestCodeButton;
    public Button verifyCodeButton;

    private string verificationCode;
    private string userEmail;

    void Start()
    {
        emailInput.onValueChanged.AddListener(OnEmailFieldChanged);
        requestCodeButton.onClick.AddListener(OnRequestVerification);
        verifyCodeButton.onClick.AddListener(OnVerifyCode);
    }

    private void OnEmailFieldChanged(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            emailMessageText.text = "";
            return;
        }

        if (!IsValidEmail(email))
        {
            SetEmailMessage("올바른 숙명여자대학교 이메일을 입력해주세요.(@sookmyung.ac.kr 또는 @sm.ac.kr)", Color.red);
        }
        else
        {
            SetEmailMessage("올바른 이메일 형식입니다.", Color.green);
        }
    }

    public void OnRequestVerification()
    {
        userEmail = emailInput.text;

        if (!IsValidEmail(userEmail))
        {
            SetEmailMessage("올바른 숙명여자대학교 이메일을 입력해주세요.(@sookmyung.ac.kr 또는 @sm.ac.kr)", Color.red);
            return;
        }

        StartCoroutine(RequestVerificationCoroutine());
    }

    private IEnumerator RequestVerificationCoroutine()
    {
        SetEmailMessage("메일 발송 중...", Color.black, true);

        yield return new WaitForSeconds(0.5f); 

        verificationCode = GenerateVerificationCode();
        SendVerificationEmail(userEmail, verificationCode);

        SetEmailMessage("입력하신 메일로 인증코드 발송하였습니다.", Color.green, true);
    }

    private void SendVerificationEmail(string recipientEmail, string code)
    {
        string senderEmail = "friendsnoonsong@gmail.com"; //프눈 계정 이메일
        string senderPassword = "tkousxipdrjdrotq"; //설정되어있는 앱 비밀번호
        
        try
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(senderEmail);
            mail.To.Add(recipientEmail);
            mail.Subject = "안녕하세요. 프렌즈! 눈송에서 인증번호를 알려드립니다.";

            string emailBody = $@"
        <head>
    
</head>
<body>
        <strong>안녕하세요. 프렌즈! 눈송입니다.</strong>
            <h1>인증코드는 {code} 입니다. 감사합니다.</h1>
        </p>
        
</body>
</html>";

            mail.Body = emailBody;
            mail.IsBodyHtml = true;

            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
            smtpServer.Port = 587;
            smtpServer.Credentials = new NetworkCredential(senderEmail, senderPassword);
            smtpServer.EnableSsl = true;

            smtpServer.Send(mail);
            Debug.Log("이메일 전송 성공 (HTML 형식)");
        }
        catch (Exception ex)
        {
            Debug.LogError("이메일 전송 실패: " + ex.Message);
            SetEmailMessage("이메일 전송 실패. 다시 시도해주세요.", Color.red, false);
        }
    }

    public void OnVerifyCode()
    {
        if (codeInput.text == verificationCode)
        {
            Debug.Log("인증 성공");
            //인증 성공 시 처리
            codeMessageText.text = "인증에 성공하였습니다.";
            codeMessageText.color = Color.green;
        }
        else
        {
            codeMessageText.text = "잘못된 인증 코드입니다";
            codeMessageText.color = Color.red;
        }
    }

    private bool IsValidEmail(string email)
    {
        return email.EndsWith("@sookmyung.ac.kr") || email.EndsWith("@sm.ac.kr");
        
    }

    private string GenerateVerificationCode()
    {
        StringBuilder code = new StringBuilder();
        using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
        {
            byte[] randomNumber = new byte[6];
            rng.GetBytes(randomNumber);

            foreach (byte b in randomNumber)
            {
                code.Append((b % 10).ToString());
            }
        }
        return code.ToString();
    }

    public void ClearInputText()
    {
        emailInput.text = "";
        codeInput.text = "";
        emailMessageText.text = "";
        codeMessageText.text = "";
        emailInput.interactable = true;
    }

    public bool isEmailVerified()
    {
        return codeMessageText.text == "인증에 성공하였습니다.";
    }

    public string GetUserEmail()
    {
        return userEmail;
    }

    private void SetEmailMessage(string message, Color color, bool disableInput = false)
    {
        emailMessageText.text = message;
        emailMessageText.color = color;
        emailInput.interactable = !disableInput;
    }
}
