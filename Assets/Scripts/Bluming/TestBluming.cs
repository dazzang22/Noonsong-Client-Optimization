using UnityEngine;
using System.Collections;
using UnityEngine.XR.ARFoundation;
using Mapbox.Unity.Map;

public class TestBluming : MonoBehaviour
{

    [SerializeField]
    GameObject spawnPrefab;

    [SerializeField]
    ARAnchorManager anchorManager;

    public Transform xrOrigin;
    [SerializeField]
    private Camera arCamera;

    private GameObject spawnedObject;
    private Transform othersObject;

    private bool isObjectSpawned = false;

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
        if (!isObjectSpawned)
        {
            Debug.Log("Spawning Object");
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
            isObjectSpawned = false;
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
        Vector3 spawnPosition = new Vector3(userPosition.x, userPosition.y - 15f, userPosition.z);

        spawnedObject = Instantiate(spawnPrefab, spawnPosition, Quaternion.identity);

        if (spawnedObject == null)
        {
            Debug.LogError("Spawned object is NULL!");
            return;
        }

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

        ARAnchor anchor = spawnedObject.AddComponent<ARAnchor>();
        if (anchor == null)
        {
            Debug.LogError("ARAnchor could not be added!");
            return;
        }

        spawnedObject.transform.SetParent(anchor.transform, true);
        Debug.Log($"ARAnchor successfully added at {spawnPosition}");
        isObjectSpawned = true;
        Debug.Log($"Turi Object spawned at {spawnPosition} with scale {spawnedObject.transform.localScale}");
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
