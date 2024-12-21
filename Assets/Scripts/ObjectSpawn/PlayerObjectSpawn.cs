using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Unity.Utilities;
using System.Collections.Generic;

public class PlayerObjectSpawn : MonoBehaviour
{
    [SerializeField]
    float spawnRadius = 20f; // 사용자의 위치에서 스폰할 반경

    [SerializeField]
    float spawnScale = 10f;

    [SerializeField]
    NoonsongManager noonsongManager;

    [SerializeField]
    FriendsManager friendsManager;

    [SerializeField]
    NoonsongEntryManager noonsongEntryManager;

    [SerializeField]
    GameObject[] generalNoonsong;

    [SerializeField]
    AbstractMap map;

    [SerializeField]
    ARAnchorManager anchorManager;

    private List<SpawnedObject> _spawnedObjects;

    public Transform xrOrigin; // XR Origin 참조
    [SerializeField]
    private Camera arCamera; // AR 카메라 참조

    [SerializeField]
    float changeInterval = 20f; // 오브젝트가 재스폰되는 시간(초)
    private float timer;

    public List<SpawnedObject> SpawnedObjects => _spawnedObjects;

    void Start()
    {
        _spawnedObjects = new List<SpawnedObject>();
        // 첫 번째 오브젝트를 스폰
        SpawnObjectNearUser();
    }

    void Update()
    {
        var activationController = GetComponentInParent<ScriptActivationController>();
        if (activationController != null && activationController.IsActive())
        {
            if (noonsongManager.Is3DViewActive() || friendsManager.Is3DViewActive())
            {
                ClearSpawnedObjects();
                return;
            }

            timer += Time.deltaTime;

            if (timer >= changeInterval)
            {
                ClearSpawnedObjects();
                SpawnObjectNearUser();
                timer = 0f;
            }

            foreach (var obj in _spawnedObjects)
            {
                if (IsObjectInView(obj))
                {
                    LookAtCamera(obj);
                }
            }
        }
    }

    void SpawnObjectNearUser()
    {
        Vector3 userPosition = xrOrigin.position;
        Vector3 randomOffset = GetRandomOffset();
        Vector3 spawnPosition = userPosition + randomOffset;
        spawnPosition.y = -5; // Y축 고정

        Debug.Log($"Attempting to spawn object at {spawnPosition}");

        var spawnedObject = GetRandomPrefab();
        GameObject prefab = spawnedObject.GameObject;


        if (prefab == null)
        {
            Debug.LogError("Prefab is null! Cannot spawn object.");
            return;
        }

        GameObject instance = Instantiate(prefab, spawnPosition, Quaternion.identity);
        Debug.Log($"Prefab {prefab.name} instantiated successfully.");

        instance.transform.localScale = new Vector3(spawnScale, spawnScale, spawnScale);

        // ARAnchor 추가
        ARAnchor anchor = instance.AddComponent<ARAnchor>();

        if (anchor == null)
        {
            Debug.LogError("Failed to attach ARAnchor to the instance.");
        }

        // Anchor를 통해 안정적으로 위치 고정
        instance.transform.parent = anchor.transform;

        _spawnedObjects.Add(new SpawnedObject(instance, spawnedObject.NoonsongEntry));
        Debug.Log($"Object added to _spawnedObjects list. Total count: {_spawnedObjects.Count}");
    }


    Vector3 GetRandomOffset()
    {
        // 랜덤 반경과 방향으로 오프셋 생성
        float angle = Random.Range(0, Mathf.PI * 2);
        float distance = Random.Range(10f, spawnRadius); // 최소 5m ~ 최대 spawnRadius
        float offsetX = Mathf.Cos(angle) * distance;
        float offsetZ = Mathf.Sin(angle) * distance;

        return new Vector3(offsetX, 0, offsetZ);
    }

    SpawnedObject GetRandomPrefab()
    {
        float probability = Random.Range(0f, 1f); // Generate a random float between 0 and 1
        if (probability < 0.6f) // 60% probability for majorNoonsong
        {
            List<NoonsongEntry> filteredEntries = GetFilteredNoonsongEntries();

            if (filteredEntries.Count > 0)
            {
                int randomIndex = Random.Range(0, filteredEntries.Count);
                return new SpawnedObject(filteredEntries[randomIndex].prefab, filteredEntries[randomIndex]);
            }
            else
            {
                int randomIndex = Random.Range(0, generalNoonsong.Length);
                return new SpawnedObject(generalNoonsong[randomIndex], null);
            }

            //여기서부터 3줄 행사용
            //NoonsongEntry[] entries = noonsongEntryManager.GetNoonsongEntries();
            //int randomIndex = Random.Range(0, entries.Length);
            //return new SpawnedObject(entries[randomIndex].prefab, entries[randomIndex]);
        }
        else // 40% probability for generalNoonsong
        {
            int randomIndex = Random.Range(0, generalNoonsong.Length);
            return new SpawnedObject(generalNoonsong[randomIndex], null);
        }
    }

    List<NoonsongEntry> GetFilteredNoonsongEntries()
    {
        var activationController = GetComponentInParent<ScriptActivationController>();
        string buildingName = activationController != null ? activationController.gameObject.name : null;

        if (!string.IsNullOrEmpty(buildingName))
        {
            return GetNoonsongEntriesByBuildingName(buildingName);
        }

        return new List<NoonsongEntry>();
    }

    List<NoonsongEntry> GetNoonsongEntriesByBuildingName(string buildingName)
    {
        List<NoonsongEntry> filteredEntries = new List<NoonsongEntry>();

        NoonsongEntry[] entries = noonsongEntryManager.GetNoonsongEntries();

        foreach (var entry in entries)
        {
            if (entry.buildingName == buildingName)
            {
                filteredEntries.Add(entry);
            }
        }

        return filteredEntries;
    }

    bool IsObjectInView(SpawnedObject obj)
    {
        // 오브젝트의 GameObject를 확인
        Vector3 viewportPoint = arCamera.WorldToViewportPoint(obj.GameObject.transform.position);

        // 뷰포트 좌표가 0~1 사이이고 Z축(깊이)이 0보다 크면 카메라에 잡힌 것으로 판단
        return viewportPoint.x >= 0 && viewportPoint.x <= 1 &&
               viewportPoint.y >= 0 && viewportPoint.y <= 1 &&
               viewportPoint.z > 0;
    }
    void LookAtCamera(SpawnedObject obj)
    {
        // 오브젝트의 GameObject를 카메라 방향으로 회전
        Vector3 directionToCamera = arCamera.transform.position - obj.GameObject.transform.position;
        directionToCamera.y = 0; // 수평 회전을 제한
        obj.GameObject.transform.rotation = Quaternion.LookRotation(directionToCamera);
    }
    void ClearSpawnedObjects()
    {
        // 기존 스폰된 오브젝트 제거
        foreach (var obj in _spawnedObjects)
        {
            Destroy(obj.GameObject); // GameObject 속성을 명시적으로 전달
        }
        _spawnedObjects.Clear();
    }
}
