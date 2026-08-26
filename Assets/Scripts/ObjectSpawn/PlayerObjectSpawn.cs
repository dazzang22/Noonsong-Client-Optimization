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
    private float timer = 0f;

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
