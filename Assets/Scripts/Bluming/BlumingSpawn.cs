using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BlumingSpawn : MonoBehaviour
{
    public float targetLatitude;
    public float targetLongitude;
    public float proximityRange = 0.01f;
    public string nextScene;

    private bool locationServiceInitialized = false;

    [SerializeField]
    private MapManager mapManager;

    void Start()
    {
        StartCoroutine(StartLocationService());
    }

    IEnumerator StartLocationService()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("GPS가 비활성화되어 있습니다!");
            yield break;
        }

        Input.location.Start();
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait <= 0 || Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("GPS를 초기화하는 데 실패했습니다.");
            yield break;
        }

        Debug.Log("GPS가 성공적으로 초기화되었습니다.");
        locationServiceInitialized = true;
    }

    void Update()
    {
        if (!locationServiceInitialized) return;

        float currentLatitude = Input.location.lastData.latitude;
        float currentLongitude = Input.location.lastData.longitude;

        Debug.Log($"현재 위치: {currentLatitude}, {currentLongitude}");

        if (IsWithinProximity(currentLatitude, currentLongitude, targetLatitude, targetLongitude) && mapManager.AreAllRegionsUnlocked())
        {
            Debug.Log("목표 위치에 도달하고 모든 구역이 해금되었습니다. 씬을 전환합니다.");
            SceneManager.LoadScene(nextScene);
        }
    }

    bool IsWithinProximity(float currentLat, float currentLon, float targetLat, float targetLon)
    {
        float latDifference = Mathf.Abs(currentLat - targetLat);
        float lonDifference = Mathf.Abs(currentLon - targetLon);
        return latDifference <= proximityRange && lonDifference <= proximityRange;
    }
}
