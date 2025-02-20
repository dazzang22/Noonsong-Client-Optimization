using UnityEngine;
using System.Collections;
using UnityEngine.XR.ARFoundation;
using Mapbox.Unity.Map;

public class TestBluming : MonoBehaviour
{
    [SerializeField]
    float spawnRadius = 20f;

    [SerializeField]
    GameObject spawnPrefab;

    [SerializeField]
    AbstractMap map;

    [SerializeField]
    ARAnchorManager anchorManager;

    public Transform xrOrigin;
    [SerializeField]
    private Camera arCamera;

    private GameObject spawnedObject;
    private Transform othersObject;

    void Update()
    {
        var activationController = GetComponentInParent<ScriptActivationController>();
        if (activationController != null && activationController.IsActive())
        {
            if (spawnedObject == null)
            {
                SpawnObject();
            }
        }
        else
        {
            if (spawnedObject != null)
            {
                Destroy(spawnedObject);
                spawnedObject = null;
            }
        }
    }

    void SpawnObject()
    {
        Vector3 userPosition = xrOrigin.position;
        Vector3 spawnPosition = userPosition;
        spawnPosition.y -= 15;
        spawnedObject = Instantiate(spawnPrefab, spawnPosition, Quaternion.identity);
        spawnedObject.transform.localScale *= 2f;

        Transform turiObject = spawnedObject.transform.Find("Turi");
        othersObject = spawnedObject.transform.Find("Others");

        if (turiObject != null)
        {
            turiObject.gameObject.SetActive(true);
        }
        if (othersObject != null)
        {
            othersObject.gameObject.SetActive(false);
        }

        var mapManager = FindObjectOfType<MapManager>();
        if (mapManager != null && mapManager.AreAllRegionsUnlocked())
        {
            UnlockBluming();
        }

        Debug.Log($"Object spawned at {spawnPosition} with scale {spawnedObject.transform.localScale}");

        GameObject instance = Instantiate(spawnedObject, spawnPosition, Quaternion.identity);
        ARAnchor anchor = instance.AddComponent<ARAnchor>();
        instance.transform.parent = anchor.transform;
    }

    public void UnlockBluming()
    {
        if (othersObject != null)
        {
            othersObject.gameObject.SetActive(true);
            Debug.Log("Others 오브젝트 활성화됨!");
        }
        else
        {
            Debug.LogWarning("Others 오브젝트를 찾을 수 없음!");
        }
    }
}