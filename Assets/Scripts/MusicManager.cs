using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MusicManager : MonoBehaviour
{
    public AudioClip area1MorningClip;
    public AudioClip area1AfternoonClip;
    public AudioClip area1EveningClip;
    public AudioClip area1NightClip;

    public AudioClip area2MorningClip;
    public AudioClip area2AfternoonClip;
    public AudioClip area2EveningClip;
    public AudioClip area2NightClip;

    public Vector2 area1Center = new Vector2(37.546033f, 126.964657f); // 1Ä·
    public float area1Radius = 0.01f; // ¹Ý°æ 0.01µµ (¾à 1km)

    public Vector2 area2Center = new Vector2(37.544392f, 126.965341f); // 2Ä·
    public float area2Radius = 0.01f;

    private bool isInArea1 = false;
    private bool isInArea2 = false;

    private AudioSource audioSource;

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
            float latitude = location.latitude;
            float longitude = location.longitude;

            TimeSpan currentTime = DateTime.Now.TimeOfDay;
            string timeOfDay = GetTimeOfDay(currentTime);

            bool inArea1 = IsWithinArea(new Vector2(latitude, longitude), area1Center, area1Radius);
            if (inArea1 && !isInArea1)
            {
                PlayMusicForTimeOfDay(area1MorningClip, area1AfternoonClip, area1EveningClip, area1NightClip, timeOfDay);
            }
            else if (!inArea1 && isInArea1)
            {
                StopMusic();
            }
            isInArea1 = inArea1;

            bool inArea2 = IsWithinArea(new Vector2(latitude, longitude), area2Center, area2Radius);
            if (inArea2 && !isInArea2)
            {
                PlayMusicForTimeOfDay(area2MorningClip, area2AfternoonClip, area2EveningClip, area2NightClip, timeOfDay);
            }
            else if (!inArea2 && isInArea2)
            {
                StopMusic();
            }
            isInArea2 = inArea2;

            yield return new WaitForSeconds(1);
        }
    }

    private bool IsWithinArea(Vector2 currentPosition, Vector2 areaCenter, float radius)
    {
        float distance = Vector2.Distance(currentPosition, areaCenter);
        return distance <= radius;
    }

    private void PlayMusicForTimeOfDay(AudioClip morningClip, AudioClip afternoonClip, AudioClip eveningClip, AudioClip nightClip, string timeOfDay)
    {
        if (audioSource.isPlaying) audioSource.Stop();

        if (timeOfDay == "Morning" && morningClip != null) audioSource.clip = morningClip;
        if (timeOfDay == "Afternoon" && afternoonClip != null) audioSource.clip = afternoonClip;
        if (timeOfDay == "Evening" && eveningClip != null) audioSource.clip = eveningClip;
        if (timeOfDay == "Night" && nightClip != null) audioSource.clip = nightClip;

        if (audioSource.clip != null) audioSource.Play();
    }

    private void StopMusic()
    {
        if (audioSource.isPlaying) audioSource.Stop();
    }

    private string GetTimeOfDay(TimeSpan currentTime)
    {
        if (currentTime >= new TimeSpan(6, 0, 0) && currentTime < new TimeSpan(12, 0, 0))
        {
            return "Morning"; // ¿ÀÀü
        }
        else if (currentTime >= new TimeSpan(12, 0, 0) && currentTime < new TimeSpan(18, 0, 0))
        {
            return "Afternoon"; // ¿ÀÈÄ
        }
        else if (currentTime >= new TimeSpan(18, 0, 0) && currentTime < new TimeSpan(24, 0, 0))
        {
            return "Evening"; // Àú³á
        }
        else
        {
            return "Night"; // ¹ã
        }
    }
}