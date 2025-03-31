using System.Collections.Generic;
using UnityEngine;

public class PlayerObjectSpawnManager : MonoBehaviour
{
    public static PlayerObjectSpawnManager Instance; // 싱글턴 패턴 사용

    private List<PlayerObjectSpawn> spawnControllers = new List<PlayerObjectSpawn>(); // 모든 스폰 컨트롤러 리스트
    private List<SpawnedObject> _spawnedObjects;

    public List<SpawnedObject> SpawnedObjects => _spawnedObjects;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _spawnedObjects = new List<SpawnedObject>();
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

    public bool CanSpawn()
    {
        if (SpawnedObjects == null) // 리스트가 null이면 초기화
        {
            Debug.LogWarning("SpawnedObjects가 null이므로 초기화합니다.");
            _spawnedObjects = new List<SpawnedObject>();
        }
        return _spawnedObjects.Count == 0; // 현재 스폰된 오브젝트가 하나도 없을 때만 스폰 가능
    }

    public void AddSpawnedObject(SpawnedObject obj)
    {
        _spawnedObjects.Add(obj);
    }

    public void RemoveSpawnedObjects()
    {
        foreach (var obj in _spawnedObjects)
        {
            if (obj.GameObject != null)
            {
                GameObject.Destroy(obj.GameObject);
                Debug.Log("destroy spawnObject(TimeOut)");
            }
        }
        _spawnedObjects.Clear();
    }
}
