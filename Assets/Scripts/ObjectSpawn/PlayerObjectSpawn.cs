using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Unity.Utilities;
using System.Collections.Generic;

public class PlayerObjectSpawn : MonoBehaviour
{
    [SerializeField] float spawnRadius = 10f; // 사용자의 위치에서 스폰할 반경

    [SerializeField] float spawnScale = 10f;

    [SerializeField] NoonsongManager noonsongManager;

    [SerializeField] FriendsManager friendsManager;

    [SerializeField] NoonsongEntryManager noonsongEntryManager;

    [SerializeField] GameObject[] generalNoonsong;

    [SerializeField] AbstractMap map;

    [SerializeField] ARAnchorManager anchorManager;

    [SerializeField] private Camera arCamera; // AR 카메라 참조

    [SerializeField] float changeInterval = 20f; // 오브젝트가 재스폰되는 시간(초)

    public Transform xrOrigin; // XR Origin 참조
    private float timer = 0f;

    [SerializeField] private EncounterUI encounterUI;

    private Vector3 lastPosition;
    private Vector3 movementDirection = Vector3.forward;

    [SerializeField] private bool testModeSpawnInFrontOfCamera = true;
    [SerializeField] private float testSpawnDistance = 3f;

    void Start()
    {
        lastPosition = xrOrigin.position;

        PlayerObjectSpawnManager.Instance.RegisterSpawnController(this);


    }

    void Update()
    {
        var activationController = GetComponentInParent<ScriptActivationController>();

        bool isInActiveZone = testModeSpawnInFrontOfCamera ||
                              (activationController != null && activationController.IsActive());

        bool isUICanvasOn =
            (noonsongManager != null && noonsongManager.Is3DViewActive()) ||
            (friendsManager != null && friendsManager.Is3DViewActive());

        if (!isInActiveZone || isUICanvasOn)
        {
            if (!testModeSpawnInFrontOfCamera &&
                activationController != null &&
                PlayerObjectSpawnManager.Instance != null)
            {
                PlayerObjectSpawnManager.Instance.RemoveSpawnedObjectsForBuilding(activationController.gameObject.name);
            }

            timer = 0f;
            return;
        }

        UpdateMovementDirection();

        if (timer == 0f)
        {
            if (PlayerObjectSpawnManager.Instance == null || PlayerObjectSpawnManager.Instance.CanSpawn())
            {
                SpawnObjectNearUser();
            }
        }

        timer += Time.deltaTime;

        if (timer >= changeInterval)
        {
            if (PlayerObjectSpawnManager.Instance != null)
            {
                PlayerObjectSpawnManager.Instance.RemoveSpawnedObjects();
            }

            if (encounterUI != null)
            {
                encounterUI.CloseEncounter();
            }

            if (PlayerObjectSpawnManager.Instance == null || PlayerObjectSpawnManager.Instance.CanSpawn())
            {
                SpawnObjectNearUser();
            }

            timer = 0f;
        }
    }


    void SpawnObjectNearUser()
    {
        if (!testModeSpawnInFrontOfCamera)
        {
            if (PlayerObjectSpawnManager.Instance != null && !PlayerObjectSpawnManager.Instance.CanSpawn())
                return;
        }

        Vector3 spawnPosition;

        if (testModeSpawnInFrontOfCamera)
        {
            spawnPosition = arCamera.transform.position + arCamera.transform.forward * testSpawnDistance;
            spawnPosition.y = arCamera.transform.position.y;
        }
        else
        {
            Vector3 userPosition = xrOrigin.position;
            Vector3 cameraForward = new Vector3(arCamera.transform.forward.x, 0, arCamera.transform.forward.z).normalized;
            spawnPosition = GetSpawnPositionInFront(userPosition, cameraForward);
            spawnPosition.y = -5;
        }

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

        float yRotation = xrOrigin != null ? xrOrigin.eulerAngles.y + 180f : arCamera.transform.eulerAngles.y + 180f;
        instance.transform.rotation = Quaternion.Euler(0, yRotation, 0);

        ARAnchor anchor = instance.AddComponent<ARAnchor>();
        anchor.transform.position = spawnPosition;
        instance.transform.parent = anchor.transform;

        var activationController = GetComponentInParent<ScriptActivationController>();
        string buildingName = activationController != null ? activationController.gameObject.name : "TestBuilding";

        if (PlayerObjectSpawnManager.Instance != null)
        {
            PlayerObjectSpawnManager.Instance.AddSpawnedObject(
                buildingName,
                new SpawnedObject(instance, spawnedObject.NoonsongEntry)
            );
        }
    }

    private void UpdateMovementDirection()
    {
        Vector3 currentPosition = xrOrigin.position;
        Vector3 displacement = currentPosition - lastPosition;

        if (displacement.magnitude > 0.01f) // 최소 이동 거리 확인 (노이즈 방지)
        {
            movementDirection = displacement.normalized; // 이동 방향 업데이트
        }
        else
        {
            movementDirection = xrOrigin.forward; // 정지 시 플레이어가 바라보는 방향 사용
        }

        lastPosition = currentPosition;
    }

    Vector3 GetSpawnPositionInFront(Vector3 userPosition, Vector3 direction)
    {
        float angle = Random.Range(-30f, 30f); // 플레이어의 시야각 내에서 랜덤 위치
        Vector3 spawnOffset = Quaternion.Euler(0, angle, 0) * direction * Random.Range(6f, spawnRadius);

        return userPosition + spawnOffset;
    }
    //랜덤 위치로 스폰
    // Vector3 GetRandomOffset()
    // {
    //     // 랜덤 반경과 방향으로 오프셋 생성
    //     float angle = Random.Range(0, Mathf.PI * 2);
    //     float distance = Random.Range(5f, spawnRadius); // 최소 5m ~ 최대 spawnRadius
    //     float offsetX = Mathf.Cos(angle) * distance;
    //     float offsetZ = Mathf.Sin(angle) * distance;

    //     return new Vector3(offsetX, 0, offsetZ);
    // }

    SpawnedObject GetRandomPrefab()
    {
        float probability = Random.Range(0f, 1f); // Generate a random float between 0 and 1
        if (probability < 0.8f) // 80% probability for majorNoonsong
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
        else // 20% probability for generalNoonsong
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

        if (testModeSpawnInFrontOfCamera)
        {
            // 테스트 모드에서는 모든 엔트리를 반환
            return new List<NoonsongEntry>(entries);
        }

        foreach (var entry in entries)
        {
            if (entry.buildingName == buildingName)
            {
                filteredEntries.Add(entry);
            }
        }

        return filteredEntries;
    }
    public bool AreAllEntriesDiscoveredForBuilding(string buildingName)
    {
        if (!string.IsNullOrEmpty(buildingName))
        {
            List<NoonsongEntry> entries = GetNoonsongEntriesByBuildingName(buildingName);

            foreach (var entry in entries)
            {
                if (!entry.isFriend)
                {
                    return false; // 발견되지 않은 항목이 있으면 false 반환
                }
            }

            return true; // 모든 항목이 발견된 경우 true 반환
        }

        return false; // 건물 이름이 없으면 false 반환
    }


    // bool IsObjectInView(SpawnedObject obj)
    // {
    //     // 오브젝트의 GameObject를 확인
    //     Vector3 viewportPoint = arCamera.WorldToViewportPoint(obj.GameObject.transform.position);

    //     // 뷰포트 좌표가 0~1 사이이고 Z축(깊이)이 0보다 크면 카메라에 잡힌 것으로 판단
    //     return viewportPoint.x >= 0 && viewportPoint.x <= 1 &&
    //            viewportPoint.y >= 0 && viewportPoint.y <= 1 &&
    //            viewportPoint.z > 0;
    // }
    // void LookAtCamera(SpawnedObject obj)
    // {
    //     // 오브젝트의 GameObject를 카메라 방향으로 회전
    //     Vector3 directionToCamera = arCamera.transform.position - obj.GameObject.transform.position;
    //     directionToCamera.y = 0; // 수평 회전을 제한
    //     obj.GameObject.transform.rotation = Quaternion.LookRotation(directionToCamera);
    // }
    // void ClearSpawnedObjects()
    // {
    //     // 기존 스폰된 오브젝트 제거
    //     foreach (var obj in _spawnedObjects)
    //     {
    //         Destroy(obj.GameObject); // GameObject 속성을 명시적으로 전달
    //     }
    //     _spawnedObjects.Clear();
    //     PlayerObjectSpawnManager.Instance.OnObjectDestroyed(); // 삭제되었음을 매니저에 알림

    // }
}
