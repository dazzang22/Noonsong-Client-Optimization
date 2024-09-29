using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    [SerializeField]
    private NoonsongEntry noonsongEntry; // NoonsongEntry에서 프리팹을 가져오기 위한 변수

    [SerializeField]
    private Transform arCamera; // AR 카메라의 Transform

    [SerializeField]
    private float distanceFromCamera = 2f; // 카메라로부터의 거리

    void Start()
    {
        if (noonsongEntry != null && noonsongEntry.prefab != null && arCamera != null)
        {
            SpawnPrefabInFrontOfCamera();
        }
        else
        {
            Debug.LogError("NoonsongEntry, prefab, or AR Camera is not assigned.");
        }
    }

    void SpawnPrefabInFrontOfCamera()
    {
        // 카메라의 앞쪽 위치를 계산
        Vector3 spawnPosition = arCamera.position + arCamera.forward * distanceFromCamera;
        Quaternion spawnRotation = arCamera.rotation;

        // 프리팹을 생성하고 위치와 회전 설정
        Instantiate(noonsongEntry.prefab, spawnPosition, spawnRotation);
    }
}

