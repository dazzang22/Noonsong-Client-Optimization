using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BackEnd;


public class FindPasswordManager : LoginBase
{
    [SerializeField]
    private Image imageID;
    [SerializeField]
    private TMP_InputField inputFieldID;
    [SerializeField]
    private Image imageEmail;
    [SerializeField]
    private TMP_InputField inputFieldEmail;

    [SerializeField]
    private Button buttonFindPW;

    public void OnclickFindPW(){
        ResetUI(imageID, imageEmail);

        if (IsFieldDataEmpty(imageID, inputFieldID.text, "아이디"))
        {
            return;
        }
        if (IsFieldDataEmpty(imageEmail, inputFieldEmail.text, "메일 주소"))
        {
            return;
        }
        
        //메일 형식 검사 
        if ( !inputFieldEmail.text.Contains("@") )
        {
            GuideForIncorrectlyEnteredData(imageEmail, "메일 형식이 올바르지 않습니다.");
            return;
        }
        buttonFindPW.interactable = false;
        SetMessage("메일 발송중입니다...");

        //비밀번호 찾기 요청
        FindCustomPW();


    }

    private void FindCustomPW()
    {
        Backend.BMember.ResetPassword(inputFieldID.text, inputFieldEmail.text, callback =>
        {//비밀번호 찾기 버튼 상호작용 활성화
            buttonFindPW.interactable = true;
            if (callback.IsSuccess())
            {
                SetMessage($"{inputFieldEmail.text} 주소로 메일을 발송하였습니다.");
            } 
            //메일 발송 실패 
            else
            {
                string message = string.Empty;

                switch(int.Parse(callback.GetStatusCode()))
                {
                
                    case 404:
                        message = "해당 이메일을 사용하는 사용자가 없습니다.";
                        break;
                    case 429:
                        message = "24시간 이내에 5회 이상 아이디/비밀번호 찾기를 시도했습니다.";
                        break;
                    default:
                        message = callback.GetMessage();
                        break;
                }
                if (message.Contains("이메일"))
                {
                    GuideForIncorrectlyEnteredData(imageEmail, message);
                }
                else
                {
                    SetMessage(message);
            }
        }
        });
    }

}
