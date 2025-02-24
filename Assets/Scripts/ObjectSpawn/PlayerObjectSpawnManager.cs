using System.Collections.Generic;
using UnityEngine;

public class PlayerObjectSpawnManager : MonoBehaviour
{
    public static PlayerObjectSpawnManager Instance; // 싱글턴 패턴 사용

    private List<PlayerObjectSpawn> spawnControllers = new List<PlayerObjectSpawn>(); // 모든 스폰 컨트롤러 리스트
    private int activeSpawnCount = 0; // 현재 스폰된 오브젝트 개수

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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
        return activeSpawnCount == 0; // 현재 스폰된 오브젝트가 없을 때만 스폰 가능
    }

    public void OnObjectSpawned()
    {
        activeSpawnCount++;
    }

    public void OnObjectDestroyed()
    {
        activeSpawnCount--;
    }
}
