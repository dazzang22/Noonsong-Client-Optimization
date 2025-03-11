using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MusicManager : MonoBehaviour
{
    [Serializable]
    public class CircularArea
    {
        public string name;
        public Vector2 center; // 중심 좌표
        public float radius;   // 반경
        public AudioClip morningClip;
        public AudioClip afternoonClip;
        public AudioClip eveningClip;
        public AudioClip nightClip;
    }

    [Serializable]
    public class PolygonalArea
    {
        public string name;
        public List<Vector2> vertices; // 다각형 꼭지점
        public AudioClip morningClip;
        public AudioClip afternoonClip;
        public AudioClip eveningClip;
        public AudioClip nightClip;
    }

    public CircularArea area3; // 구역 3 (원형)
    public CircularArea area4; // 구역 4 (원형)
    public PolygonalArea area1; // 구역 1 (다각형)
    public PolygonalArea area2; // 구역 2 (다각형)

    private AudioSource audioSource;
    private string currentArea = null; // 현재 활성화된 구역 이름
    private const float gpsUpdateInterval = 1f; // GPS 업데이트 주기 (초)

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        StartCoroutine(StartLocationService());
    }

    private IEnumerator StartLocationService()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("Location service is disabled.");
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait <= 0 || Input.location.status != LocationServiceStatus.Running)
        {
            Debug.LogError("Unable to start location service.");
            yield break;
        }

        Debug.Log("Location service started.");
        StartCoroutine(UpdateLocation());
    }

    private IEnumerator UpdateLocation()
    {
        while (true)
        {
            LocationInfo location = Input.location.lastData;
            Vector2 currentPosition = new Vector2(location.latitude, location.longitude);

            UpdateMusicForArea(currentPosition);

            yield return new WaitForSeconds(gpsUpdateInterval);
        }
    }

    private void UpdateMusicForArea(Vector2 currentPosition)
    {
        string timeOfDay = GetTimeOfDay();
        string detectedArea = null;

        // 구역 3 확인
        if (IsWithinCircularArea(currentPosition, area3))
        {
            detectedArea = area3.name;
            PlayMusicForTimeOfDay(area3, timeOfDay);
        }
        // 구역 4 확인
        else if (IsWithinCircularArea(currentPosition, area4))
        {
            detectedArea = area4.name;
            PlayMusicForTimeOfDay(area4, timeOfDay);
        }
        // 구역 1 확인
        else if (IsWithinPolygonalArea(currentPosition, area1))
        {
            detectedArea = area1.name;
            PlayMusicForTimeOfDay(area1, timeOfDay);
        }
        // 구역 2 확인
        else if (IsWithinPolygonalArea(currentPosition, area2))
        {
            detectedArea = area2.name;
            PlayMusicForTimeOfDay(area2, timeOfDay);
        }

        // 구역 변경 시 음악 정지
        if (detectedArea == null && currentArea != null)
        {
            StopMusic();
        }

        currentArea = detectedArea;
    }

    private bool IsWithinCircularArea(Vector2 position, CircularArea area)
    {
        float distance = Vector2.Distance(position, area.center);
        return distance <= area.radius;
    }

    private bool IsWithinPolygonalArea(Vector2 position, PolygonalArea area)
    {
        int intersectCount = 0;
        for (int i = 0; i < area.vertices.Count; i++)
        {
            Vector2 p1 = area.vertices[i];
            Vector2 p2 = area.vertices[(i + 1) % area.vertices.Count];

            if (RayIntersectsSegment(position, p1, p2))
            {
                intersectCount++;
            }
        }
        return intersectCount % 2 == 1; // 홀수 교차점이면 내부
    }

    private bool RayIntersectsSegment(Vector2 point, Vector2 p1, Vector2 p2)
    {
        if (p1.y > p2.y)
        {
            Vector2 temp = p1;
            p1 = p2;
            p2 = temp;
        }

        if (point.y == p1.y || point.y == p2.y)
        {
            point.y += 0.0001f; // 수평선 교차 방지
        }

        if (point.y < p1.y || point.y > p2.y || point.x > Mathf.Max(p1.x, p2.x))
        {
            return false;
        }

        if (point.x < Mathf.Min(p1.x, p2.x))
        {
            return true;
        }

        float slope = (p2.x - p1.x) / (p2.y - p1.y);
        float xIntersection = p1.x + (point.y - p1.y) * slope;

        return point.x <= xIntersection;
    }

    private void PlayMusicForTimeOfDay(dynamic area, string timeOfDay)
    {
        if (audioSource.isPlaying && currentArea == area.name) return;

        AudioClip clipToPlay = null;
        switch (timeOfDay)
        {
            case "Morning":
                clipToPlay = area.morningClip;
                break;
            case "Afternoon":
                clipToPlay = area.afternoonClip;
                break;
            case "Evening":
                clipToPlay = area.eveningClip;
                break;
            case "Night":
                clipToPlay = area.nightClip;
                break;
        }

        if (clipToPlay != null)
        {
            audioSource.Stop();
            audioSource.clip = clipToPlay;
            audioSource.Play();
        }
    }

    private void StopMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
    }

    private string GetTimeOfDay()
    {
        TimeSpan currentTime = DateTime.Now.TimeOfDay;
        if (currentTime >= new TimeSpan(6, 0, 0) && currentTime < new TimeSpan(12, 0, 0))
            return "Morning";
        if (currentTime >= new TimeSpan(12, 0, 0) && currentTime < new TimeSpan(18, 0, 0))
            return "Afternoon";
        if (currentTime >= new TimeSpan(18, 0, 0) && currentTime < new TimeSpan(24, 0, 0))
            return "Evening";
        return "Night";
    }
}