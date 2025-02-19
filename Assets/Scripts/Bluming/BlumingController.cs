using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class BlumingController : MonoBehaviour
{
    public GameObject mapPrefab;
    public string alwaysActiveGroupName = "ActiveGroup";
    public string initiallyInactiveGroupName = "HiddenGroup";

    public float latitude;
    public float longitude;
    public float altitudeOffset = -10.0f;
    public Vector3 spawnScale = new Vector3(3, 3, 3);

    private Camera arCamera;
    private GameObject spawnedMap;

    void Start()
    {
        arCamera = Camera.main;

        if (arCamera == null)
        {
            Debug.LogError("AR 카메라를 찾을 수 없습니다. 씬에 AR 카메라가 있는지 확인하세요.");
            return;
        }

        StartCoroutine(SpawnMapAtGPSLocation());
    }

    IEnumerator SpawnMapAtGPSLocation()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("GPS가 활성화되지 않았습니다.");
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            Debug.LogError("GPS 정보를 가져올 수 없습니다.");
            yield break;
        }


        LocationInfo userLocation = Input.location.lastData;
        Vector3 spawnPosition = ConvertGPSLocationToWorldPosition(userLocation.latitude, userLocation.longitude);
        spawnPosition.y += altitudeOffset;

        spawnedMap = Instantiate(mapPrefab, spawnPosition, Quaternion.identity);
        spawnedMap.transform.localScale = spawnScale;

        Transform activeGroup = spawnedMap.transform.Find(alwaysActiveGroupName);
        if (activeGroup != null)
        {
            activeGroup.gameObject.SetActive(true);
            Debug.Log($"{alwaysActiveGroupName} 그룹 활성화됨.");
        }
        else
        {
            Debug.LogWarning($"{alwaysActiveGroupName} 그룹을 찾을 수 없음.");
        }

        Transform inactiveGroup = spawnedMap.transform.Find(initiallyInactiveGroupName);
        if (inactiveGroup != null)
        {
            inactiveGroup.gameObject.SetActive(false);
            Debug.Log($"{initiallyInactiveGroupName} 그룹 비활성화됨.");
        }
        else
        {
            Debug.LogWarning($"{initiallyInactiveGroupName} 그룹을 찾을 수 없음.");
        }
    }

    public void ActivateHiddenObjects()
    {
        if (spawnedMap == null)
        {
            Debug.LogWarning("맵이 아직 스폰되지 않았습니다.");
            return;
        }

        Transform inactiveGroup = spawnedMap.transform.Find(initiallyInactiveGroupName);
        if (inactiveGroup != null)
        {
            inactiveGroup.gameObject.SetActive(true);
            Debug.Log($"{initiallyInactiveGroupName} 그룹이 활성화됨!");
        }
        else
        {
            Debug.LogWarning($"{initiallyInactiveGroupName} 그룹을 찾을 수 없음.");
        }
    }



    Vector3 ConvertGPSLocationToWorldPosition(float targetLatitude, float targetLongitude)
    {
        float scaleFactor = 1000f;
        float x = (targetLongitude - longitude) * scaleFactor;
        float z = (targetLatitude - latitude) * scaleFactor;
        return new Vector3(x, 0, z);
    }
}