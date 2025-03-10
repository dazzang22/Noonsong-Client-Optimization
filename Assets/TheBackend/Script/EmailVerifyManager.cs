using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Mail;
using System;
using System.Security.Cryptography;
using System.Text;
using TMPro;

public class EmailVerifyManager : MonoBehaviour
{
    public GameObject emailPanel;
    public GameObject codePanel;
    public GameObject successPanel;

    public TMP_InputField emailInput;
    public TMP_InputField codeInput;
    public TMP_Text emailErrorText;
    public TMP_Text codeErrorText;

    public Button requestCodeButton;
    public Button verifyCodeButton;

    private string verificationCode;
    private string userEmail;

    void Start()
    {
        emailPanel.SetActive(true);
        codePanel.SetActive(false);
        successPanel.SetActive(false);

        requestCodeButton.onClick.AddListener(OnRequestVerification);
        verifyCodeButton.onClick.AddListener(OnVerifyCode);
    }

    public void OnRequestVerification()
    {
        userEmail = emailInput.text;

        if (!IsValidEmail(userEmail))
        {
            emailErrorText.text = "올바른 숙명여자대학교 이메일을 입력해주세요.(@sookmyung.ac.kr 또는 @sm.ac.kr)";
            return;
        }

        verificationCode = GenerateVerificationCode();
        SendVerificationEmail(userEmail, verificationCode);

        emailPanel.SetActive(false);
        codePanel.SetActive(true);
    }

    private void SendVerificationEmail(string recipientEmail, string code)
    {
        string senderEmail = "friendsnoonsong@gmail.com"; //프눈계정 이메일
        string senderPassword = "tkousxipdrjdrotq"; //설정한 앱 비밀번호
        
        try
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(senderEmail);
            mail.To.Add(recipientEmail);
            mail.Subject = "[프렌즈 눈송!] 학교 이메일 인증 코드";

            string emailBody = $@"
        <html>
        <head>
            <style>
                body {{ font-family: Arial, sans-serif; text-align: center; }}
                .container {{ width: 100%; max-width: 500px; margin: auto; border: 1px solid #ddd; padding: 20px; border-radius: 10px; background-color: #f9f9f9; }}
                .title {{ font-size: 18px; font-weight: bold; margin-bottom: 20px; }}
                .code-box {{ background-color: #fff; padding: 15px; border-radius: 5px; font-size: 24px; font-weight: bold; color: #2A3C89; }}
                .footer {{ font-size: 12px; color: #666; margin-top: 20px; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='title'>이메일 인증코드를 입력해주세요.</div>
                <div class='code-box'>{code}</div>
                <div class='footer'>
                    프렌즈 눈송! 학교 인증 이메일 본인 확인을 위한 메일입니다.<br>
                    게임에 접속 후, 위의 코드를 입력하여 인증을 완료해 주세요
                </div>
            </div>
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
        }
    }

    public void OnVerifyCode()
    {
        if (codeInput.text == verificationCode)
        {
            Debug.Log("인증 성공");
            //인증 성공 시 처리
            codePanel.SetActive(false);
            successPanel.SetActive(true);
            emailErrorText.text = "";
            //DB 연결
            UserDataManager.Instance.SetSEmail(userEmail);
        }
        else
        {
            codeErrorText.text = "잘못된 인증 코드입니다";
        }
    }

    private bool IsValidEmail(string email)
    {
        if (!email.Contains("@") || !email.Contains("."))
        {
            return false;
        }

        if (email.EndsWith("@sookmyung.ac.kr") || email.EndsWith("@sm.ac.kr"))
        {
            return true;
        }
        return false;
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
        emailErrorText.text = "";
        codeErrorText.text = "";
        emailPanel.SetActive(true);
        codePanel.SetActive(false);
        successPanel.SetActive(false);
    }
}
