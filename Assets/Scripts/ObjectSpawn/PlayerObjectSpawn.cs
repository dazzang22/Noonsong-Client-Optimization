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
    NoonsongEntryManager noonsongEntryManager;

    [SerializeField]
    GameObject[] generalNoonsong;

    [SerializeField]
    AbstractMap map;

    [SerializeField]
    ARAnchorManager anchorManager;

    private List<SpawnedObject> _spawnedObjects;

    public Transform xrOrigin; // XR Origin 참조
    public Camera arCamera; // AR 카메라 참조

    [SerializeField]
    float changeInterval = 10f; // 오브젝트가 재스폰되는 시간(초)
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
        // Increment timer
        timer += Time.deltaTime;

        // Check if it's time to change the marker position and prefab
        if (timer >= changeInterval)
        {
            // 기존 오브젝트 제거
            ClearSpawnedObjects();

            // 새 위치에 오브젝트 스폰
            SpawnObjectNearUser();
            
            timer = 0f; // Reset timer
        }

        // 스폰된 오브젝트가 카메라를 바라보도록 설정
        foreach (var obj in _spawnedObjects)
        {
            if (IsObjectInView(obj))
            {
                LookAtCamera(obj);
            }
        }
    }

    void SpawnObjectNearUser()
    {
        // XR Origin의 위치를 기반으로 오브젝트를 스폰
        Vector3 userPosition = xrOrigin.position;
        Vector3 randomOffset = GetRandomOffset();

        Vector3 spawnPosition = userPosition + randomOffset;
        spawnPosition.y = -5; // Y축 고정

        // 랜덤한 프리팹 선택
        var spawnedObject = GetRandomPrefab();
        GameObject prefab = spawnedObject.GameObject;
        NoonsongEntry entry = spawnedObject.NoonsongEntry;
        GameObject instance = Instantiate(prefab, spawnPosition, Quaternion.identity);
        instance.transform.localScale = new Vector3(spawnScale, spawnScale, spawnScale);

        // ARAnchor 추가
        ARAnchor anchor = instance.AddComponent<ARAnchor>();

        // Anchor를 통해 안정적으로 위치 고정
        instance.transform.parent = anchor.transform;

        _spawnedObjects.Add(new SpawnedObject(instance, entry));
        Debug.Log($"Object spawned near user at: {spawnPosition}");
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
        }
        else // 40% probability for generalNoonsong
        {
            int randomIndex = Random.Range(0, generalNoonsong.Length);
            return new SpawnedObject(generalNoonsong[randomIndex], null);
        }
    }

    List<NoonsongEntry> GetFilteredNoonsongEntries()
    {
        string buildingName = GetBuildingNameFromScript(ScriptActivationController.activatedScriptName);

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

    string GetBuildingNameFromScript(string activatedScriptName)
    {
        switch (activatedScriptName)
        {
            case "Sunheon Bldg":
                return "�����";
            case "Suryeon Bldg":
                return "���ñ���ȸ��";
            case "Haengpa Faculty Bldg":
                return "���ı���ȸ��";
            case "Veritas Bldg":
                return "������";
            case "Myungshin Bldg":
                return "���Ű�";
            case "Wisdom Bldg":
                return "������";
            case "Saehim Bldg":
                return "������";
            case "Acministration Bldg":
                return "������";
            case "Peace Bldg":
                return "��ȭ��";
            case "Student Union Bldg":
                return "�л�ȸ��";
            case "Arena Theater Bldg":
                return "��������";

            default:
                return null;
        }
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
