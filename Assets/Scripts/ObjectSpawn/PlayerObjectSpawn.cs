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

    [SerializeField] private Camera arCamera;

    [SerializeField] float changeInterval = 20f; // 오브젝트가 재스폰되는 시간(초)

    [SerializeField] private float majorNoonsongProbability = 0.8f;

    public Transform xrOrigin; // XR Origin 참조
    // 상태머신
    private SpawnLifecycle spawnLifecycle;
    private SpawnedObject ownedSpawnedObject;
    private bool wasPhotoModeActive;

    [SerializeField] private EncounterUI encounterUI;

    private Vector3 lastPosition;
    private Vector3 movementDirection = Vector3.forward;

    [SerializeField] private bool testModeSpawnInFrontOfCamera = false;
    [SerializeField] private float testSpawnDistance = 3f;

    //caching fields for performance
    private Dictionary<string, List<NoonsongEntry>> entriesByBuilding;
    private List<NoonsongEntry> allEntriesCache;
    private static readonly List<NoonsongEntry> EmptyEntries = new List<NoonsongEntry>();

    void Start()
    {
        spawnLifecycle = new SpawnLifecycle(changeInterval);
        lastPosition = xrOrigin.position;
        PlayerObjectSpawnManager.Instance.RegisterSpawnController(this);
        BuildEntryCache();
    }

    private void BuildEntryCache()
    {
        allEntriesCache = noonsongEntryManager.GetNoonsongEntries();
        entriesByBuilding = new Dictionary<string, List<NoonsongEntry>>();

        foreach (var entry in allEntriesCache)
        {
            if (entry == null || string.IsNullOrEmpty(entry.buildingName))
                continue;

            if (!entriesByBuilding.TryGetValue(entry.buildingName, out var list))
            {
                list = new List<NoonsongEntry>();
                entriesByBuilding[entry.buildingName] = list;
            }

            list.Add(entry);
        }
    }
    void Update()
    {
        var activationController = GetComponentInParent<ScriptActivationController>();

        bool isInActiveZone = testModeSpawnInFrontOfCamera ||
                              (activationController != null && activationController.IsActive());

        bool isPhotoModeActive =
            (noonsongManager != null && noonsongManager.Is3DViewActive()) ||
            (friendsManager != null && friendsManager.Is3DViewActive());

        // 사진 모드 전환: Hide 또는 Show를 한 번만 실행
        HandlePhotoModeState(isPhotoModeActive);

        // 구역·사진 모드와 무관하게 항상 20초 수명을 진행
        ExecuteSpawnCommand(spawnLifecycle.Tick(Time.deltaTime));

        // 새 스폰만 막는다. 이미 스폰된 오브젝트의 수명은 계속 유지한다.
        if (!isInActiveZone || isPhotoModeActive)
        {
            return;
        }

        UpdateMovementDirection();

        // 현재 필드 오브젝트가 있을 때는 새로 생성하지 않는다.
        if (spawnLifecycle.CurrentState != SpawnState.NoObjects)
        {
            return;
        }

        if (PlayerObjectSpawnManager.Instance == null ||
            PlayerObjectSpawnManager.Instance.CanSpawn())
        {
            SpawnObjectNearUser();
        }
    }
    private void ExecuteSpawnCommand(SpawnCommand command)
    {
        switch (command)
        {
            case SpawnCommand.Hide:
                if (ownedSpawnedObject?.GameObject != null)
                {
                    ownedSpawnedObject.GameObject.SetActive(false);
                }
                break;
            case SpawnCommand.Show:
                if (ownedSpawnedObject?.GameObject != null)
                {
                    ownedSpawnedObject.GameObject.SetActive(true);
                }
                break;
            case SpawnCommand.Remove:
                RemoveOwnedSpawnedObject();
                break;
            case SpawnCommand.None:
            default:
                // Do nothing
                break;
        }
    }
    private void RemoveOwnedSpawnedObject()
    {
        // Hide -> 비활성화, Remove -> 제거, Show -> 활성화
        if (ownedSpawnedObject?.GameObject != null)
        {
            ownedSpawnedObject.GameObject.SetActive(false);
            Destroy(ownedSpawnedObject.GameObject);
        }
        if (PlayerObjectSpawnManager.Instance != null)
        {
            PlayerObjectSpawnManager.Instance.RemoveSpawnedObject(ownedSpawnedObject);
        }
        ownedSpawnedObject = null;
        if (encounterUI != null)
        {
            encounterUI.CloseEncounter();
        }
    }

    private void HandlePhotoModeState(bool isPhotoModeActive)
    {
        if (isPhotoModeActive == wasPhotoModeActive)
        {
            return; // 상태가 변하지 않았으므로 아무 작업도 수행하지 않음
        }

        wasPhotoModeActive = isPhotoModeActive;
        SpawnCommand command = isPhotoModeActive ? spawnLifecycle.OnPhotoOpened() : spawnLifecycle.OnPhotoClosed();
        ExecuteSpawnCommand(command);
        Debug.Log($"Photo mode state changed. Command: {command}, New State: {spawnLifecycle.CurrentState}");
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

        ownedSpawnedObject = new SpawnedObject(instance, spawnedObject.NoonsongEntry);
        if (PlayerObjectSpawnManager.Instance != null)
        {
            PlayerObjectSpawnManager.Instance.AddSpawnedObject(
                buildingName,
                ownedSpawnedObject
            );
        }
        spawnLifecycle.NotifySpawnSucceeded();
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

    private SpawnedObject GetRandomPrefab()
    {
        float probability = Random.Range(0f, 1f);
        if (probability < majorNoonsongProbability)
        {
            List<NoonsongEntry> filteredEntries = GetFilteredNoonsongEntries();

            if (filteredEntries.Count > 0)
            {
                int randomIndex = Random.Range(0, filteredEntries.Count);
                NoonsongEntry selectedEntry = filteredEntries[randomIndex];

                return new SpawnedObject(selectedEntry.prefab, selectedEntry);
            }
        }
        return GetGeneralNoonsongPrefab();
    }

    private SpawnedObject GetGeneralNoonsongPrefab()
    {
        int randomIndex = Random.Range(0, generalNoonsong.Length);
        return new SpawnedObject(generalNoonsong[randomIndex], null);
    }

    private List<NoonsongEntry> GetFilteredNoonsongEntries()
    {
        var activationController = GetComponentInParent<ScriptActivationController>();
        string buildingName = activationController != null ? activationController.gameObject.name : null;

        if (!string.IsNullOrEmpty(buildingName))
        {
            return GetNoonsongEntriesByBuildingName(buildingName);
        }

        return EmptyEntries;
    }

    private List<NoonsongEntry> GetNoonsongEntriesByBuildingName(string buildingName)
    {
        if (testModeSpawnInFrontOfCamera)
        {
            return allEntriesCache;
        }

        if (entriesByBuilding != null && entriesByBuilding.TryGetValue(buildingName, out var entries))
        {
            return entries;
        }
        return EmptyEntries; // 해당 건물 이름에 대한 엔트리가 없는 경우 빈 리스트 반환
    }
    public bool AreAllNoonsongsFriendsInBuilding(string buildingName)
    {
        if (!string.IsNullOrEmpty(buildingName))
        {
            List<NoonsongEntry> entries = GetNoonsongEntriesByBuildingName(buildingName);

            foreach (var entry in entries)
            {
                if (!entry.isFriend)
                {
                    return false;
                }
            }

            return true;
        }

        return false;
    }
}
