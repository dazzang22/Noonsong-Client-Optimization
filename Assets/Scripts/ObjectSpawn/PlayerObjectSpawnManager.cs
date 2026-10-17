using System.Collections.Generic;
using UnityEngine;

public class PlayerObjectSpawnManager : MonoBehaviour
{
    public static PlayerObjectSpawnManager Instance;

    private List<PlayerObjectSpawn> spawnControllers = new List<PlayerObjectSpawn>();
    private readonly List<PlayerObjectSpawn> spawnCandidates = new List<PlayerObjectSpawn>();
    private Dictionary<string, List<SpawnedObject>> _spawnedObjectsPerBuilding;
    private List<SpawnedObject> _spawnedObjects;
    private PlayerObjectSpawn slotOwner;
    public List<SpawnedObject> SpawnedObjects => _spawnedObjects;
    public ARObjectCatch ObjectCatch { get; private set; }

    public void RegisterObjectCatch(ARObjectCatch objectCatch)
    {
        ObjectCatch = objectCatch;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _spawnedObjects = new List<SpawnedObject>();
            _spawnedObjectsPerBuilding = new Dictionary<string, List<SpawnedObject>>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        // 전역 슬롯이 사용 중이면 선택하지 않음
        if (Instance != this || !CanSpawn())
        {
            return;
        }

        // 지금 스폰할 수 있는 건물을 다시 모음
        RefreshSpawnCandidates();

        if (spawnCandidates.Count == 0)
        {
            return;
        }
        DebugSpawnCandidates();

        // 후보 중 하나를 균등 랜덤으로 선택
        int randomIndex = Random.Range(0, spawnCandidates.Count);
        PlayerObjectSpawn selected = spawnCandidates[randomIndex];

        var area = selected.GetComponentInParent<ScriptActivationController>();
        string buildingName = area != null ? area.gameObject.name : selected.gameObject.name;

        Debug.Log(
            $"[SpawnSelection] 후보 수={spawnCandidates.Count}, 선택={buildingName}",
            selected
        );

        selected.TrySpawnFromManager();
        if (selected.IsSpawnLifetimeRunning)
        {
            slotOwner = selected;
        }
    }

    public void RegisterSpawnController(PlayerObjectSpawn controller)
    {
        if (!spawnControllers.Contains(controller))
        {
            spawnControllers.Add(controller);
        }
    }
    // 스폰 후보 갱신
    private void RefreshSpawnCandidates()
    {
        spawnCandidates.Clear();

        foreach (PlayerObjectSpawn controller in spawnControllers)
        {
            if (controller != null && controller.IsReadyToSpawn())
            {
                spawnCandidates.Add(controller);
            }
        }
    }
    [ContextMenu("스폰 후보 확인")]
    private void DebugSpawnCandidates()
    {
        RefreshSpawnCandidates();

        Debug.Log($"[SpawnCandidates] 후보 수: {spawnCandidates.Count}");

        foreach (PlayerObjectSpawn candidate in spawnCandidates)
        {
            var area = candidate.GetComponentInParent<ScriptActivationController>();

            string buildingName = area != null
                ? area.gameObject.name
                : candidate.gameObject.name;

            Debug.Log($"[SpawnCandidates] 후보: {buildingName}", candidate);
        }
    }

    // 전체에 대해 아무 오브젝트도 없을 때만 스폰 가능
    public bool CanSpawn()
    {
        if (slotOwner != null && slotOwner.IsSpawnLifetimeRunning) return false;
        foreach (var kvp in _spawnedObjectsPerBuilding)
        {
            if (kvp.Value.Count > 0)
                return false;
        }
        return true;
    }

    // 특정 건물에 오브젝트 추가
    public void AddSpawnedObject(string buildingName, SpawnedObject obj)
    {
        if (!_spawnedObjectsPerBuilding.ContainsKey(buildingName))
        {
            _spawnedObjectsPerBuilding[buildingName] = new List<SpawnedObject>();
        }
        _spawnedObjectsPerBuilding[buildingName].Add(obj);
        _spawnedObjects.Add(obj);
    }

    // Destroy된 단일 오브젝트의 registry 정보 제거
    public void RemoveSpawnedObject(SpawnedObject target)
    {
        if (target == null)
            return;

        _spawnedObjects.Remove(target);

        string emptyBuildingKey = null;

        foreach (var kvp in _spawnedObjectsPerBuilding)
        {
            if (!kvp.Value.Remove(target))
                continue;

            if (kvp.Value.Count == 0)
                emptyBuildingKey = kvp.Key;

            break;
        }

        if (emptyBuildingKey != null)
            _spawnedObjectsPerBuilding.Remove(emptyBuildingKey);
    }

    // 특정 건물에 대한 오브젝트 제거
    public void RemoveSpawnedObjectsForBuilding(string buildingName)
    {
        if (!_spawnedObjectsPerBuilding.TryGetValue(buildingName, out var objects))
            return;

        foreach (var obj in objects)
        {
            if (obj?.GameObject != null)
            {
                obj.GameObject.SetActive(false);
                GameObject.Destroy(obj.GameObject);
                Debug.Log($"Destroyed object for building: {buildingName}");
            }

            _spawnedObjects.Remove(obj);
        }

        objects.Clear();
        _spawnedObjectsPerBuilding.Remove(buildingName);
    }

    // 전체 제거 (타이머 기반 리셋 등)
    public void RemoveSpawnedObjects()
    {
        foreach (var kvp in _spawnedObjectsPerBuilding)
        {
            foreach (var obj in kvp.Value)
            {
                if (obj?.GameObject != null)
                {
                    obj.GameObject.SetActive(false);
                    GameObject.Destroy(obj.GameObject);
                    Debug.Log("Destroy spawnObject (TimeOut)");
                }
            }
        }

        _spawnedObjects.Clear();
        _spawnedObjectsPerBuilding.Clear();
    }

    // 디버깅용 전체 리스트 조회
    public Dictionary<string, List<SpawnedObject>> GetAllSpawnedObjects()
    {
        return _spawnedObjectsPerBuilding;
    }
}
