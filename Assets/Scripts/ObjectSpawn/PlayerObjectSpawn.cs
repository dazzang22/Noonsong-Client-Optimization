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

    [SerializeField] private float majorNoonsongProbability = 0.8f;

    public Transform xrOrigin; // XR Origin 참조
    // 상태머신
    private SpawnLifecycle spawnLifecycle;
    public bool IsSpawnLifetimeRunning => spawnLifecycle != null && spawnLifecycle.CurrentState != SpawnState.NoObjects;
    private SpawnedObject ownedSpawnedObject;
    private bool wasPhotoModeActive;
    private const float changeInterval = 20f;

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
        bool isInActiveZone = IsInSpawnArea();
        bool shouldHideFieldObject = ShouldHideFieldObject();

        HandleExternallyRemovedObjec();
        // hide/show field object based on photo mode and encounter state
        HandlePhotoModeState(shouldHideFieldObject);

        // 구역·사진 모드와 무관하게 항상 20초 수명을 진행
        ExecuteSpawnCommand(spawnLifecycle.Tick(Time.deltaTime));

        // 새 스폰만 막는다. 이미 스폰된 오브젝트의 수명은 계속 유지한다.
        if (!isInActiveZone)
        {
            return;
        }

        UpdateMovementDirection();
    }
    public bool IsInSpawnArea()
    {
        var activationController = GetComponentInParent<ScriptActivationController>();

        return testModeSpawnInFrontOfCamera ||
               (activationController != null && activationController.IsActive());
    }
    public bool IsReadyToSpawn()
    {
        return isActiveAndEnabled &&
               spawnLifecycle != null &&
               IsInSpawnArea() &&
               spawnLifecycle.CurrentState == SpawnState.NoObjects;
    }
    private bool IsPhotoModeActive()
    {
        return (noonsongManager != null && noonsongManager.Is3DViewActive()) ||
               (friendsManager != null && friendsManager.Is3DViewActive());
    }
    private void HandleExternallyRemovedObjec()
    {
        if (ownedSpawnedObject == null || ownedSpawnedObject.GameObject != null)
        {
            return;
        }
        ownedSpawnedObject = null;
        spawnLifecycle.NotifyObjectRemovedExternally();

        Debug.Log("Owned spawned object has been removed externally. Transitioning to WaitingForLifetimeEnd state.");
    }
    private bool ShouldHideFieldObject()
    {
        // 조우 -> 숨길지 말지 판단
        var manager = PlayerObjectSpawnManager.Instance;

        GameObject encounterTarget =
            manager != null && manager.ObjectCatch != null
                ? manager.ObjectCatch.EncounterTarget
                : null;

        GameObject fieldObject =
            ownedSpawnedObject != null
                ? ownedSpawnedObject.GameObject
                : null;

        // 현재 조우 중인 A -> 숨기지 no
        if (encounterTarget != null && encounterTarget == fieldObject)
            return false;

        // 사진 모드 또는 조우 중이라면 필드 캐릭터를 숨긴다.
        return IsPhotoModeActive() || encounterTarget != null;
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
        if (ownedSpawnedObject == null)
            return;

        var manager = PlayerObjectSpawnManager.Instance;
        GameObject fieldObject = ownedSpawnedObject.GameObject;

        // 수명이 끝난 캐릭터가 현재 조우 대상인지 확인한다. 
        bool isEncounterTarget =
            manager != null &&
            manager.ObjectCatch != null &&
            fieldObject != null &&
            manager.ObjectCatch.EncounterTarget == fieldObject;

        if (isEncounterTarget)
        {
            // 조우 중인 A는 화면에 남겨둔다.
            Debug.Log(
                $"[SpawnLifecycle] 조우 대상 유지 id={fieldObject.GetInstanceID()}"
            );
        }
        else if (fieldObject != null)
        {
            // 조우 대상이 아니면 평소처럼 제거한다.
            Debug.Log(
                $"[SpawnLifecycle] 수명 만료로 제거 id={fieldObject.GetInstanceID()}"
            );

            fieldObject.SetActive(false);
            Destroy(fieldObject);
        }

        // 조우 중인 A는 남기더라도 필드 슬롯은 비운다.
        if (manager != null)
        {
            manager.RemoveSpawnedObject(ownedSpawnedObject);
        }

        ownedSpawnedObject = null;
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
        //Debug.Log($"Photo mode state changed. Command: {command}, New State: {spawnLifecycle.CurrentState}");
    }

    public void TrySpawnFromManager()
    {
        // 현재도 이 건물이 스폰 가능한지 확인
        if (!IsReadyToSpawn())
        {
            return;
        }

        // 전역 슬롯이 비어 있는지 확인
        if (PlayerObjectSpawnManager.Instance == null ||
            !PlayerObjectSpawnManager.Instance.CanSpawn())
        {
            return;
        }

        SpawnObjectNearUser();
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
        // 조우 중이거나 사진 모드일 때는 숨김 처리
        if (ShouldHideFieldObject())
        {
            SpawnCommand hideCommand = spawnLifecycle.OnPhotoOpened();
            ExecuteSpawnCommand(hideCommand);
            wasPhotoModeActive = true; // 상태를 업데이트하여 다음 프레임에서 중복 실행 방지
            Debug.Log(
    $"[SpawnLifecycle] PhotoSpawn id={instance.GetInstanceID()}, " +
    $"command={hideCommand}, activeSelf={instance.activeSelf}"
);
        }
        //debug lifetime
        Debug.Log(
    $"[SpawnLifecycle] Spawned building={buildingName}, " +
    $"id={instance.GetInstanceID()}, " +
    $"lifetime={changeInterval:F1}s"
);
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
