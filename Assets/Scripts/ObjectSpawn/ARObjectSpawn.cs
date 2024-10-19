using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Unity.Utilities;
using System.Collections.Generic;
using Unity.VisualScripting;
using static UnityEngine.EventSystems.EventTrigger;

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

    [SerializeField]
    ARAnchorManager _anchorManager; // ARAnchorManager 추가

    private List<SpawnedObject> _spawnedObjects;

    [SerializeField]
    float changeInterval = 2f; // Time interval in seconds

    private float timer;
    public List<SpawnedObject> SpawnedObjects => _spawnedObjects;

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
        instance.transform.position = new Vector3(worldPosition.x, -5, worldPosition.z);
        instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);

        // Add ARAnchor to the instance directly
        var anchor = instance.AddComponent<ARAnchor>();

        // Optionally, you can use ARAnchorManager to manage anchors in a more advanced setup.
        // In this case, you are directly adding ARAnchor to the instance.
        instance.transform.parent = anchor.transform;

        _spawnedObjects.Add(new SpawnedObject(instance, entry));

    }

    SpawnedObject GetRandomPrefab()
    {
        float probability = Random.Range(0f, 1f); // Generate a random float between 0 and 1
        if (probability < 0.6f) // 60% probability for majorNoonsong
        {
            List<NoonsongEntry> filteredEntries = GetFilteredNoonsongEntries();

            if (filteredEntries.Count > 0)
            {
                int randomIndex = Random.Range(0, filteredEntries.Count);
                return new SpawnedObject(filteredEntries[randomIndex].prefab, filteredEntries[randomIndex]);
            }
            else
            {
                int randomIndex = Random.Range(0, generalNoonsong.Length);
                return new SpawnedObject(generalNoonsong[randomIndex], null);
            }
            

        }
        else // 40% probability for generalNoonsong
        {
            int randomIndex = Random.Range(0, generalNoonsong.Length);
            return new SpawnedObject(generalNoonsong[randomIndex], null);
        }
    }

    List<NoonsongEntry> GetFilteredNoonsongEntries()
    {
        string buildingName = GetBuildingNameFromScript(ScriptActivationController.activatedScriptName);

        if (!string.IsNullOrEmpty(buildingName))
        {
            return GetNoonsongEntriesByBuildingName(buildingName);
        }

        return new List<NoonsongEntry>();
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

    List<NoonsongEntry> GetNoonsongEntriesByBuildingName(string buildingName)
    {
        List<NoonsongEntry> filteredEntries = new List<NoonsongEntry>();

        NoonsongEntry[] entries = noonsongEntryManager.GetNoonsongEntries();

        foreach (var entry in entries)
        {
            if (entry.buildingName == buildingName)
            {
                filteredEntries.Add(entry);
            }
        }

        return filteredEntries; 
    }

    string GetBuildingNameFromScript(string activatedScriptName)
    {
        switch (activatedScriptName)
        {
            case "Sunheon Bldg":
                return "순헌관";
            case "Suryeon Bldg":
                return "수련교수회관";
            case "Haengpa Faculty Bldg":
                return "행파교수회관";
            case "Veritas Bldg":
                return "진리관";
            case "Myungshin Bldg":
                return "명신관";
            case "Wisdom Bldg":
                return "지혜문";
            case "Saehim Bldg":
                return "세힘관";
            case "Acministration Bldg":
                return "행정관";
            case "Peace Bldg":
                return "평화문";
            case "Student Union Bldg":
                return "학생회관";
            case "Arena Theater Bldg":
                return "원형극장";

            default:
                return null;
        }
    }
}
