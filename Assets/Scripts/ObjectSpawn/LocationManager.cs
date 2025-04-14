using UnityEngine;
using UnityEngine.Android;
using System.Collections;
using System;
using Mapbox.Utils;

public class LocationManager : MonoBehaviour
{
    public static LocationManager Instance { get; private set; }
    
    public bool IsLocationServiceInitialized { get; private set; } = false;

    public static event Action<Vector2d> OnLocationUpdated;

    [SerializeField] private float updateInterval = 5f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(RequestLocationPermission());
    }

    IEnumerator RequestLocationPermission()
    {
        // Android 권한 요청
        while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            yield return null;
        }

        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("Location services are not enabled by the user.");
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1 || Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Failed to initialize location services.");
            yield break;
        }

        IsLocationServiceInitialized = true;
        Debug.Log("Location services initialized.");

        StartCoroutine(UpdateLocationRoutine());
    }

    IEnumerator UpdateLocationRoutine()
    {
        while (true)
        {
            if (IsLocationServiceInitialized)
            {
                Vector2d userLocation = new Vector2d(Input.location.lastData.latitude, Input.location.lastData.longitude);
                Debug.Log($"[LocationManager] Broadcasting location: {userLocation}");
                OnLocationUpdated?.Invoke(userLocation);
            }

            yield return new WaitForSeconds(updateInterval);
        }   
    }
}
