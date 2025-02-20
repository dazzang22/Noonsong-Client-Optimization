using UnityEngine;
using System.Collections;
using UnityEngine.XR.ARFoundation;
using Mapbox.Unity.Map;

public class TestBluming : MonoBehaviour
{
    [SerializeField]
    GameObject spawnPrefab;

    [SerializeField]
    AbstractMap map;

    [SerializeField]
    Transform xrOrigin;

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
        Vector3 spawnPosition = xrOrigin.position;
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