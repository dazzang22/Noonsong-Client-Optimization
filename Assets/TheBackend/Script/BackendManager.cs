using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks; // [변경] async 기능을 이용하기 위해서는 해당 namepsace가 필요합니다.  
using TMPro;
using UnityEngine.UI;

// 뒤끝 SDK namespace 추가
using BackEnd;

public class BackendManager : MonoBehaviour
{

    void Start()
    {
        var bro = Backend.Initialize(); // 뒤끝 초기화

        // 뒤끝 초기화에 대한 응답값
        if (bro.IsSuccess())
        {
            Debug.Log("초기화 성공 : " + bro); // 성공일 경우 statusCode 204 Success
            string googlehash = Backend.Utils.GetGoogleHash();
            Debug.Log("구글 해시 키 : " + googlehash);
        
            //점검중 관리
            Backend.ErrorHandler.OnMaintenanceError = () => {
                Debug.Log("점검 에러 발생!!!");
                VersionNoticeManger.Instance.SetMaintenanceNotice( "점검 중입니다.",true,false);
                StartCoroutine(OpenNoticeUI("점검 중입니다."));
            };
            //버전 & 공지 관리
            StartCoroutine(VersionAndNoticeFlow());
        }
        else
        {
            Debug.LogError("초기화 실패 : " + bro); // 실패일 경우 statusCode 400대 에러 발생
        } 
       

        //Test();

        //BackendChart.Instance.InitializeShopInfo(() =>
        //{
            //Debug.Log("차트 데이터 로드 완료 후 TuriShopManager 호출");
            //TuriShopManager.Instance.DisplayShopItems(); //테스트
        //});
    }
    private IEnumerator VersionAndNoticeFlow()
    {
        VersionNoticeManger.Instance.VersionCheck();
        string vt=VersionNoticeManger.Instance.newText;
        Debug.Log($"{VersionNoticeManger.Instance.newText}");
        if (VersionNoticeManger.Instance.isversion)
        {
            yield return StartCoroutine(OpenNoticeUI(vt));
        }

        VersionNoticeManger.Instance.NoticeCheck();
        string pt=VersionNoticeManger.Instance.newText;
        if (VersionNoticeManger.Instance.ispopup)
        {
            yield return StartCoroutine(OpenNoticeUI(pt));
        }
    }

private IEnumerator OpenNoticeUI(string newtext)
{
    GameObject prefab = Resources.Load<GameObject>("Canvas)SavePoint PopupTutorial");
    if (prefab == null)
    {
        Debug.LogError("공지 팝업 프리팹을 찾을 수 없습니다.");
        yield break;
    }

    GameObject popup = Instantiate(prefab);
    popup.transform.SetAsLastSibling();

    TMP_Text[] texts = popup.GetComponentsInChildren<TMP_Text>();
    if (texts.Length >= 2)
    {
        texts[0].text = "공지";
        texts[1].text = newtext;
        Debug.Log($"{newtext}");
    }

    Button close = popup.GetComponentInChildren<Button>();
    if (close != null)
        close.onClick.AddListener(() => Destroy(popup));

    yield return new WaitWhile(() => popup != null);
}


    // =======================================================
    // [추가] 동기 함수를 비동기에서 호출하게 해주는 함수(유니티 UI 접근 불가)
    // =======================================================
// <<<<<<< Updated upstream
//     async void Test()
//     {
        /*await Task.Run(() => {
            
            BackendLogin.Instance.CustomSignUp("user1", "1234"); // [추가] 뒤끝 회원가입 함수
            Debug.Log("테스트를 종료합니다.");
            
        });*/
        //BackendLogin.Instance.CustomLogin("user1", "1234"); // [추가] 뒤끝 로그인
        //BackendLogin.Instance.UpdateNickname("Noonsong"); // [추가] 닉네임 변겅
        //Debug.Log("테스트를 종료합니다.");

    // async void Test()
    // {
    //     await Task.Run(() => {
    //         BackendLogin.Instance.CustomSignUp("user2","1234");
    //         BackendLogin.Instance.CustomLogin("user2","1234");
    //         Debug.Log("차트를 가져옵니다.");

            
    //         BackendChart.Instance.ChartGet("157725");
    //         Debug.Log("테스트를 종료합니다.");
    //     });
    // }


    void Test(){
        BackendLogin.Instance.CustomLogin("emily9821","Emily9821");
        //재화 평균
        GameStatistics.Instance.AverageSnowCount();
        //유저별 & 눈송이별 수집율
        GameStatistics.Instance.UpdateNoonStatics();


        //Debug.Log("테스트를 종료합니다.");

    }
}