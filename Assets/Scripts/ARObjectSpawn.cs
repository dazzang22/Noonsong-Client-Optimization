using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using System.Collections.Generic;
using Mapbox.Unity.Utilities;

[RequireComponent(typeof(ARRaycastManager))]
public class ARObjectSpawn : MonoBehaviour
{
    [SerializeField]
    [Geocode]
    string[] _locationStrings;
    Vector2d[] _locations;

    [SerializeField]
    float _spawnScale = 30f;

    [SerializeField]
    NoonsongManager noonsongManager;

    [SerializeField]
    NoonsongEntryManager noonsongEntryManager;

    [SerializeField]
    GameObject[] generalNoonsong;

    [SerializeField]
    AbstractMap _map;

    private ARRaycastManager _raycastManager;
    private List<SpawnedObject> _spawnedObjects;
    private List<ARRaycastHit> _hits = new List<ARRaycastHit>();

    [SerializeField]
    float changeInterval = 2f; // Time interval in seconds

    private float timer;
    public List<SpawnedObject> SpawnedObjects => _spawnedObjects;

    void Awake()
    {
        _raycastManager = GetComponent<ARRaycastManager>();
    }

    void Start()
    {
        InitializeLocations();
        _spawnedObjects = new List<SpawnedObject>();

        // Spawn an initial marker at a random location
        SpawnMarkerAtRandomLocation();
    }

    void InitializeLocations()
    {
        _locations = new Vector2d[_locationStrings.Length];
        for (int i = 0; i < _locationStrings.Length; i++)
        {
            _locations[i] = Conversions.StringToLatLon(_locationStrings[i]);
        }
    }

    void SpawnMarkerAtRandomLocation()
    {
        int randomIndex = Random.Range(0, _locations.Length);
        Vector2d location = _locations[randomIndex];

        var spawnedObject = GetRandomPrefab();
        GameObject prefab = spawnedObject.GameObject;
        NoonsongEntry entry = spawnedObject.NoonsongEntry;

        var instance = Instantiate(prefab);
        var worldPosition = _map.GeoToWorldPosition(location, true);
        instance.transform.position = new Vector3(worldPosition.x, worldPosition.y, worldPosition.z);
        instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);

        // Store the instance and its NoonsongEntry
        _spawnedObjects.Add(new SpawnedObject(instance, entry));
    }

    SpawnedObject GetRandomPrefab()
    {
        float probability = Random.Range(0f, 1f); // Generate a random float between 0 and 1
        if (probability < 0.6f) // 60% probability for majorNoonsong
        {
            NoonsongEntry[] entries = noonsongEntryManager.GetNoonsongEntries();
            int randomIndex = Random.Range(0, entries.Length);
            return new SpawnedObject(entries[randomIndex].prefab, entries[randomIndex]);
        }
        else // 40% probability for generalNoonsong
        {
            int randomIndex = Random.Range(0, generalNoonsong.Length);
            return new SpawnedObject(generalNoonsong[randomIndex], null);
        }
    }

    void Update()
    {
        // Increment timer
        timer += Time.deltaTime;

        // Check if it's time to change the marker position and prefab
        if (timer >= changeInterval)
        {
            ChangeMarker();
            timer = 0f; // Reset timer
        }
    }

    void ChangeMarker()
    {
        // Choose a new random index from the _locations array
        int randomIndex = Random.Range(0, _locations.Length);
        var newWorldPosition = _map.GeoToWorldPosition(_locations[randomIndex], true);

        // Destroy the previous marker
        if (_spawnedObjects.Count > 0)
        {
            Destroy(_spawnedObjects[0].GameObject);
            _spawnedObjects.Clear();
        }

        // Spawn a new marker at the new position with a new prefab
        SpawnMarkerAtRandomLocation();
    }
}
