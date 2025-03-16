using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using Doublsb.Dialog;
using TMPro;

public class GPSManager : MonoBehaviour
{
    public bool[] isVisited; // 각 위치의 방문 여부를 저장하는 배열
    public double[] lats; // 각 위치의 위도를 저장하는 배열
    public double[] longs; // 각 위치의 경도를 저장하는 배열
    //public Animator popup_anim; // 팝업 애니메이션을 관리하는 애니메이터
    public TalkDialogue talkDialogue; // TalkDialog 클래스 인스턴스, 다이얼로그 호출에 사용
    public Text printerText;

    private int currentIndex = 0; // 현재 방문해야 할 위치의 인덱스

    public GameObject savepointPopupUI; //savapoint 할때 팝업
    private string newText; //savepoint에서 시작할 때 안내 text
    void Awake()
    {

        currentIndex = BackendSavePoint.Instance.LoadGameData();
        Debug.Log($"시작 인덱스: {currentIndex}");

        if(currentIndex !=0 && currentIndex !=4)
        {
            for(int i=0; i<currentIndex; i++)
            {
                talkDialogue.dialogTriggered[i]=true;
            }   
        }
        if(currentIndex==4)
        {
            talkDialogue.CompleteTutorial();
        }
        if(currentIndex > 0 && currentIndex < 4)
        {  
            StartCoroutine(InitializeSavePoint());
            for(int j=0; j<currentIndex; j++)
            {
                talkDialogue.dialogTriggered[j]=true;
            }
        }
        
    }
    private IEnumerator InitializeSavePoint()
    {
        yield return StartCoroutine(ShowSavePointPopup());
        while (savepointPopupUI != null)
        {
            yield return null; // 프레임마다 확인
        }

    }

        void ClosePrefab()
    {
        if (savepointPopupUI != null)
        {
            Destroy(savepointPopupUI);
            savepointPopupUI = null;
        }
    }

    //세이브포인트 팝업
    private IEnumerator ShowSavePointPopup()
    {
        Time.timeScale = 0f; // 시간 정지
        //savapoint popup tuto 로드
        GameObject prefab = Resources.Load<GameObject>("Canvas)SavePoint PopupTutorial");
        if(prefab != null)
        {
            Debug.Log("로드");
            savepointPopupUI= Instantiate(prefab);
            savepointPopupUI.transform.SetAsLastSibling();
            TMP_Text[] textComponents = savepointPopupUI.GetComponentsInChildren<TMP_Text>();
            Button closeButton = savepointPopupUI.GetComponentInChildren<Button>();

            if (textComponents.Length >= 2)
            {
                if(currentIndex==1)
                newText="사라진 눈송이를 찾아 1캠퍼스 정문으로 이동해보자!";
                if(currentIndex==2)
                    newText="사라진 눈결이를 찾아 2캠퍼스 정문으로 이동해보자!";
                if(currentIndex==3)
                    newText="사라진 눈송이를 찾아 프라임관으로 이동해보자!"; 
                // 두 번째 TMP_Text의 텍스트 변경
                textComponents[1].text = newText;
            }
            savepointPopupUI.SetActive(true);
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(ClosePrefab);
            }
        }

        while (savepointPopupUI !=null) // 마우스 클릭을 기다림
        {
            yield return null; // 한 프레임을 대기
        }

        Time.timeScale = 1f; // 시간 재개
    }
    IEnumerator Start()
    {
        //세이브 포인트 로드
        currentIndex = BackendSavePoint.Instance.LoadGameData();
        Debug.Log($"시작 인덱스: {currentIndex}");

        if(currentIndex<0 && currentIndex>4)
        {
            yield return null;

        }
        while (savepointPopupUI !=null) // 마우스 클릭을 기다림
        {
            yield return null; // 한 프레임을 대기
        }
        yield break;

        //위치 권한이 있는지 확인하고 요청
        while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            yield return null;
            Permission.RequestUserPermission(Permission.FineLocation);
        }

        // 사용자가 위치 서비스를 활성화했는지 확인
        if (!Input.location.isEnabledByUser)
            yield break;

        // 위치 서비스 시작
        Input.location.Start(10, 1);

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // 위치 서비스 초기화 실패 시 종료
        if (maxWait < 1)
        {
            print("Timed out");
            yield break;
        }

        // 위치 서비스 실패 시 종료
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");
            yield break;
        }
        else
        {
            // 위치 정보가 성공적으로 받아지면 초기 위치를 출력
            print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude);
        }

        // yield return new WaitForSeconds(5f);
        // StartCoroutine(DialogTrigger());
    }


    void LateUpdate() {
        
          int nextIndex = currentIndex + 1; // 다음 위치의 인덱스를 설정

        // 위치 서비스가 실행 중인 경우
        if (Input.location.status == LocationServiceStatus.Running)
        {
            double myLat = Input.location.lastData.latitude;
            double myLong = Input.location.lastData.longitude;
            Debug.Log($"{lats.Length}");

            // 현재 인덱스 위치에 대한 거리 계산
            if (currentIndex < lats.Length && !isVisited[currentIndex])
            {
                double remainDistance = distance(myLat, myLong, lats[currentIndex], longs[currentIndex]);

                // 지정된 거리 내에 도착하면
                //if (remainDistance <= 5f) // 5m 이내
                if (remainDistance <= 15f) // 10m 이내
                {
                    if (talkDialogue.IsDialogTriggered(currentIndex))
                    {
                        BackendSavePoint.Instance.SaveGameData(currentIndex);                        
                        Debug.Log($"currentIndex:{currentIndex}");
                        if(currentIndex<3)
                            isVisited[currentIndex] = true; // 방문 여부를 true로 설정
                        // printerText.text = "";
                        TriggerDialog(nextIndex); // 해당 위치의 다이얼로그 호출
                        currentIndex++; // 다음 위치로 인덱스 증가
                    }
                }
                // else
                // {
                //     if (talkDialogue.IsDialogTriggered(0))
                //     {
                //         Debug.Log("firstDialog 완료");
                //         printerText.text = "1캠퍼스 정문으로 가세요";
                //     }
                //     else if(talkDialogue.IsDialogTriggered(1))
                //     {
                //         printerText.text = "2캠퍼스 정문으로 가세요";
                //     }
                //     else if(talkDialogue.IsDialogTriggered(2))
                //     {
                //         printerText.text = "프라임관으로 가세요";
                //     }
                // }
            }
        }

    }


    private double distance(double lat1, double lon1, double lat2, double lon2)
    {
        // 두 좌표 간의 거리 계산 (Haversine 공식을 사용)
        double theta = lon1 - lon2;
        double dist = Math.Sin(Deg2Rad(lat1)) * Math.Sin(Deg2Rad(lat2)) + Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) * Math.Cos(Deg2Rad(theta));
        dist = Math.Acos(dist);
        dist = Rad2Deg(dist);
        dist = dist * 60 * 1.1515;
        dist = dist * 1609.344; // 미터로 변환
        return dist;
    }

    private double Deg2Rad(double deg)
    {
        return (deg * Mathf.PI / 180.0f);
    }

    private double Rad2Deg(double rad)
    {
        return (rad * 180.0f / Mathf.PI);
    }

    // 인덱스에 따라 해당 다이얼로그를 트리거하는 메서드
    private void TriggerDialog(int index)
    {
        BackendSavePoint.Instance.SaveGameData(index);
        switch (index)
        {
            case 0:
                talkDialogue.FirstDialog();
                break;
            case 1:
                if (talkDialogue.IsDialogTriggered(0)) // 이전 다이얼로그가 호출되었는지 확인
                {
                    talkDialogue.SecondDialog();
                }
                    
                break;
            case 2:
                if (talkDialogue.IsDialogTriggered(1)) // 이전 다이얼로그가 호출되었는지 확인
                {
                    talkDialogue.ThirdDialog();
                }    
                break;
            case 3:
                if (talkDialogue.IsDialogTriggered(2)) // 이전 다이얼로그가 호출되었는지 확인
                {
                    talkDialogue.FourthDialog();
                }    
                break;
        }
    }
}