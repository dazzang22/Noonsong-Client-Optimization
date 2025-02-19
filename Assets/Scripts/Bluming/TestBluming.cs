using Mapbox.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBluming : MonoBehaviour
{
    public GameObject mapPrefab;
    public float spawnDistance = 2.0f;
    public float yOffset = -7.0f;
    public Vector3 spawnScale = new Vector3(3, 3, 3);

    public Camera arCamera;

    public string alwaysActiveGroupName = "ActiveGroup";
    public string initiallyInactiveGroupName = "HiddenGroup";

    private GameObject spawnedMap;

    void Start()
    {
        arCamera = Camera.main;

        if (arCamera == null)
        {
            Debug.LogError("AR 카메라를 찾을 수 없습니다. ARSession 설정을 확인하세요.");
            return;
        }

        SpawnMapAtCurrentLocation();
    }

    void SpawnMapAtCurrentLocation()
    {
        Vector3 spawnPosition = arCamera.transform.position + arCamera.transform.forward * spawnDistance;
        spawnPosition.y += yOffset;

        spawnedMap = Instantiate(mapPrefab, spawnPosition, Quaternion.identity);
        spawnedMap.transform.localScale = spawnScale;

        Debug.Log("맵이 현재 위치 앞에 스폰되었습니다!");

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
}