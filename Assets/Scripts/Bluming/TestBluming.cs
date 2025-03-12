using UnityEngine;
using System.Collections;
using UnityEngine.XR.ARFoundation;
using Mapbox.Unity.Map;

public class TestBluming : MonoBehaviour
{
    [SerializeField]
    float spawnRadius = 4f; //¹Ý°æ 4m

    [SerializeField]
    GameObject spawnPrefab;

    [SerializeField]
    Vector2 targetLocation;

    [SerializeField]
    ARAnchorManager anchorManager;

    public Transform xrOrigin;
    [SerializeField]
    private Camera arCamera;

    private GameObject spawnedObject;
    private Transform othersObject;

    private void OnEnable()
    {
        MusicManager.OnEnterArea4 += HandleEnterArea4;
        MusicManager.OnExitArea4 += HandleExitArea4;
    }

    private void OnDisable()
    {
        MusicManager.OnEnterArea4 -= HandleEnterArea4;
        MusicManager.OnExitArea4 -= HandleExitArea4;
    }

    private void HandleEnterArea4()
    {
        Debug.Log("Entered Area 4 - Spawning Object");
        if (spawnedObject == null)
        {
            SpawnObject();
        }
    }

    private void HandleExitArea4()
    {
        Debug.Log("Exited Area 4 - Destroying Object");
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
            spawnedObject = null;
        }
    }

    IEnumerator Start()
    {
        if (!Input.location.isEnabledByUser)
        {
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait <= 0)
        {
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            yield break;
        }
    }

    void SpawnObject()
    {
        Vector3 userPosition = xrOrigin.position;
        Vector3 spawnPosition = userPosition;
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

        Debug.Log($"Turi Object spawned at {spawnPosition} with scale {spawnedObject.transform.localScale}");

        GameObject instance = Instantiate(spawnedObject, spawnPosition, Quaternion.identity);
        ARAnchor anchor = instance.AddComponent<ARAnchor>();
        instance.transform.parent = anchor.transform;
    }

    public void UnlockBluming()
    {
        if (othersObject != null)
        {
            othersObject.gameObject.SetActive(true);
            Debug.Log("Others activated!");
        }
        else
        {
            Debug.LogWarning("Others null");
        }
    }
}
