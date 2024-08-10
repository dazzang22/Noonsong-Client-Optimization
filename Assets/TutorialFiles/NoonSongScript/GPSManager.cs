using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class GPSManager : MonoBehaviour
{
    //public Text textUI; // UI에 현재 위치를 표시하기 위한 텍스트 UI 요소
    //public GameObject[] popUps; // 팝업 UI 요소를 관리하기 위한 배열
    public bool[] isVisited; // 각 위치의 방문 여부를 저장하는 배열
    public double[] lats; // 각 위치의 위도를 저장하는 배열
    public double[] longs; // 각 위치의 경도를 저장하는 배열
    //public Animator popup_anim; // 팝업 애니메이션을 관리하는 애니메이터
    public TalkDialogue talkDialogue; // TalkDialog 클래스 인스턴스, 다이얼로그 호출에 사용

    private int currentIndex = 0; // 현재 방문해야 할 위치의 인덱스

    IEnumerator Start()
    {
        // 위치 권한이 있는지 확인하고 요청
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
    }

    void Update()
    {
        // 위치 서비스가 실행 중인 경우
        if (Input.location.status == LocationServiceStatus.Running)
        {
            double myLat = Input.location.lastData.latitude;
            double myLong = Input.location.lastData.longitude;

            // 현재 인덱스 위치에 대한 거리 계산
            if (currentIndex < lats.Length && !isVisited[currentIndex])
            {
                double remainDistance = distance(myLat, myLong, lats[currentIndex], longs[currentIndex]);

                // 지정된 거리 내에 도착하면
                if (remainDistance <= 100f) // 1m
                {
                    isVisited[currentIndex] = true; // 방문 여부를 true로 설정
                    TriggerDialog(currentIndex); // 해당 위치의 다이얼로그 호출
                    currentIndex++; // 다음 위치로 인덱스 증가
                }
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
        switch (index)
        {
            case 0:
                talkDialogue.FirstDialog();
                break;
            case 1:
                if (talkDialogue.IsDialogTriggered(0)) // 이전 다이얼로그가 호출되었는지 확인
                    talkDialogue.SecondDialog();
                break;
            case 2:
                if (talkDialogue.IsDialogTriggered(1)) // 이전 다이얼로그가 호출되었는지 확인
                    talkDialogue.ThirdDialog();
                break;
            case 3:
                if (talkDialogue.IsDialogTriggered(2)) // 이전 다이얼로그가 호출되었는지 확인
                    talkDialogue.ForthDialog();
                break;
        }
    }
}