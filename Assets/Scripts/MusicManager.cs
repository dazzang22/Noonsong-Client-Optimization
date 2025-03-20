using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using BackEnd;

public class MusicManager : MonoBehaviour
{
    public static event Action OnEnterArea4;
    public static event Action OnExitArea4;

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
    private string currentArea = null;
    private DateTime serverTime;
    private bool isServerTimeSynced = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.loop = true;
        audioSource.volume = 1f;

        StartCoroutine(SyncServerTime());
    }

    private void OnEnable()
    {
        ScriptActivationController.OnLocationUpdated += UpdateMusicForArea;
    }

    private void OnDisable()
    {
        ScriptActivationController.OnLocationUpdated -= UpdateMusicForArea;
    }

    private void UpdateMusicForArea(Vector2d userLocation)
    {
        Vector2 currentPosition = new Vector2((float)userLocation.x, (float)userLocation.y);

        string timeOfDay = GetCurrentTimeOfDay();
        string detectedArea = null;

        if (IsWithinCircularArea(currentPosition, area3))
        {
            detectedArea = area3.name;
            PlayMusicForTimeOfDay(area3, timeOfDay);
        }
        else if (IsWithinCircularArea(currentPosition, area4))
        {
            detectedArea = area4.name;
            PlayMusicForTimeOfDay(area4, timeOfDay);
        }
        else if (IsWithinPolygonalArea(currentPosition, area1))
        {
            detectedArea = area1.name;
            PlayMusicForTimeOfDay(area1, timeOfDay);
        }
        else if (IsWithinPolygonalArea(currentPosition, area2))
        {
            detectedArea = area2.name;
            PlayMusicForTimeOfDay(area2, timeOfDay);
        }

        HandleAreaTransition(detectedArea);
    }

    private void HandleAreaTransition(string detectedArea)
    {
        if (detectedArea == area4.name && currentArea != area4.name)
        {
            OnEnterArea4?.Invoke();
        }
        else if (currentArea == area4.name && detectedArea != area4.name)
        {
            OnExitArea4?.Invoke();
        }

        if (detectedArea != currentArea)
        {
            if (detectedArea == null && currentArea != null)
            {
                Debug.Log($"Exiting Area: {currentArea}");
                StopMusic();
            }
            else if (detectedArea != null)
            {
                Debug.Log($"Entering Area: {detectedArea}");
            }

            currentArea = detectedArea;
            Debug.Log($"Current Area updated to: {currentArea}");
        }
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
        return intersectCount % 2 == 1;
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
            point.y += 0.0001f;
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

    private void PlayMusicForTimeOfDay(CircularArea area, string timeOfDay)
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
        else
        {
            Debug.LogWarning($"No clip assigned for {timeOfDay} in area {area.name}");
        }
    }

    private void PlayMusicForTimeOfDay(PolygonalArea area, string timeOfDay)
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
        else
        {
            Debug.LogWarning($"No clip assigned for {timeOfDay} in area {area.name}");
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

    private string GetCurrentTimeOfDay()
    {
        int hour = serverTime.Hour;

        if (hour >= 6 && hour < 12)
            return "Morning";
        else if (hour >= 12 && hour < 18)
            return "Afternoon";
        else if (hour >= 18 && hour < 24)
            return "Evening";
        else
            return "Night";
    }

    private IEnumerator SyncServerTime()
    {
        while (true)
        {
            SendQueue.Enqueue(Backend.Utils.GetServerTime, bro =>
            {
                if (bro.IsSuccess())
                {
                    string time = bro.GetReturnValuetoJSON()["utcTime"].ToString();
                    serverTime = DateTime.Parse(time);
                    isServerTimeSynced = true;
                    Debug.Log($"Server time synced: {serverTime}");
                }
                else
                {
                    Debug.LogError($"Failed to retrieve server time: {bro}");
                }
            });

            yield return new WaitForSeconds(60f); // 1분마다 서버 시간 동기화
        }
    }
}
