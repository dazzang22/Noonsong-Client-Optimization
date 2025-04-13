using System.Collections.Generic;
using UnityEngine;

public class PlayerObjectSpawnManager : MonoBehaviour
{
    public static PlayerObjectSpawnManager Instance;

    private List<PlayerObjectSpawn> spawnControllers = new List<PlayerObjectSpawn>();
    private Dictionary<string, List<SpawnedObject>> _spawnedObjectsPerBuilding;
    private List<SpawnedObject> _spawnedObjects;
    public List<SpawnedObject> SpawnedObjects => _spawnedObjects;

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

    public void RegisterSpawnController(PlayerObjectSpawn controller)
    {
        if (!spawnControllers.Contains(controller))
        {
            spawnControllers.Add(controller);
        }
    }

    // 전체에 대해 아무 오브젝트도 없을 때만 스폰 가능
    public bool CanSpawn()
    {
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

    // 특정 건물에 대한 오브젝트 제거
    public void RemoveSpawnedObjectsForBuilding(string buildingName)
    {
        if (_spawnedObjectsPerBuilding.ContainsKey(buildingName))
        {
            foreach (var obj in _spawnedObjectsPerBuilding[buildingName])
            {
                if (obj.GameObject != null)
                {
                    GameObject.Destroy(obj.GameObject);
                    Debug.Log($"Destroyed object for building: {buildingName}");
                }
            }
            _spawnedObjectsPerBuilding[buildingName].Clear();
        }
    }

    // 전체 제거 (타이머 기반 리셋 등)
    public void RemoveSpawnedObjects()
    {
        foreach (var kvp in _spawnedObjectsPerBuilding)
        {
            foreach (var obj in kvp.Value)
            {
                if (obj.GameObject != null)
                {
                    GameObject.Destroy(obj.GameObject);
                    Debug.Log("Destroy spawnObject (TimeOut)");
                }
                _spawnedObjects.Clear();
            }
            kvp.Value.Clear();
        }
    }

    // 디버깅용 전체 리스트 조회
    public Dictionary<string, List<SpawnedObject>> GetAllSpawnedObjects()
    {
        return _spawnedObjectsPerBuilding;
    }
}
