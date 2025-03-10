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


    void Update()
    {
        if (IsPlayerInZone())
        {
            if (spawnedObject == null)
            {
                Debug.Log("Player is in Bluming Spot");
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
    bool IsPlayerInZone()
    {
        Vector2 playerLocation = GetPlayerLocation();

        float distance = GetDistanceFromTarget(playerLocation, targetLocation);
        return distance <= spawnRadius;
    }

    Vector2 GetPlayerLocation()
    {
        if (!Input.location.isEnabledByUser)
        {
            return Vector2.zero;
        }

        if (Input.location.status == LocationServiceStatus.Running)
        {
            return new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
        }

        return Vector2.zero;
    }

    float GetDistanceFromTarget(Vector2 playerLocation, Vector2 targetLocation)
    {
        float earthRadius = 6371000f;
        float dLat = Mathf.Deg2Rad * (targetLocation.x - playerLocation.x);
        float dLon = Mathf.Deg2Rad * (targetLocation.y - playerLocation.y);

        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
                  Mathf.Cos(Mathf.Deg2Rad * playerLocation.x) *
                  Mathf.Cos(Mathf.Deg2Rad * targetLocation.x) *
                  Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);

        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        return earthRadius * c;
    }
}