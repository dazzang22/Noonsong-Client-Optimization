using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    private PlayerObjectSpawn playerObjectSpawn;

    public GameObject mapUI;
    //public GameObject messageUI;
    public GameObject[] regions;
    public bool[] regionUnlocked;

    private Vector2 startTouchPosition;
    private Vector2 currentTouchPosition;
    private bool isScrolling = false;

    private List<string> buildingNames = new List<string> { "Sunheon Bldg", "Myungshin Bldg", "Suryeon Bldg", "Wisdom Bldg", "Renaissance Plaza Bldg", "College of Science Bldg", "College of Music Bldg", "College of Fine Arts Bldg", "Center for Continuing Education  Bldg", "College of Pharmacy Bldg" };

    void Start()
    {
        if (playerObjectSpawn == null)
        {
            playerObjectSpawn = FindObjectOfType<PlayerObjectSpawn>();
            if (playerObjectSpawn == null)
            {
                Debug.LogError("PlayerObjectSpawn is not assigned and cannot be found in the scene.");
                return;
            }
        }

        LoadMapState();
        CheckAndUnlockRegions();
    }

    void Update()
    {
        HandleTouchInput();
    }

    public void LoadMapState()
    {
        for (int i = 0; i < regions.Length; i++)
        {
            if (regionUnlocked[i])
            {
                regions[i].SetActive(false);
            }
            else
            {
                regions[i].SetActive(true);
            }
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                currentTouchPosition = touch.position;
                ScrollMap();
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                CheckRegionTouch(touch.position);
            }
        }
        else if (Input.touchCount == 2)
        {
            HandlePinchZoom();
        }
    }

    private void ScrollMap()
    {
        Vector2 delta = currentTouchPosition - startTouchPosition;
        mapUI.transform.Translate(delta * Time.deltaTime);
        isScrolling = true;
    }

    private void CheckRegionTouch(Vector2 touchPosition)
    {
        if (isScrolling) return;

        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            for (int i = 0; i < regions.Length; i++)
            {
                if (hit.collider.gameObject == regions[i])
                {
                    if (regionUnlocked[i])
                    {
                        ShowRegionDetails(i);
                    }
                    else
                    {
                        ShowUnlockMessage(i);
                    }
                    return;
                }
            }
        }
    }

    private void HandlePinchZoom()
    {
        Debug.Log("Pinch zoom handling");
    }

    private void ShowRegionDetails(int regionIndex)
    {
        //messageUI.SetActive(true);
        Debug.Log($"Showing details for region {regionIndex}");
    }

    private void ShowUnlockMessage(int regionIndex)
    {
        Debug.Log($"Region {regionIndex} is locked! Unlocking now...");
        UnlockRegion(regionIndex);
    }

    public void UnlockRegion(int regionIndex)
    {
        if (!regionUnlocked[regionIndex])
        {
            regionUnlocked[regionIndex] = true;
            regions[regionIndex].SetActive(false);
        }
    }

    public void CheckAndUnlockRegions()
    {
        for (int i = 0; i < regions.Length; i++)
        {
            string buildingName = buildingNames[i];

            bool allDiscovered = playerObjectSpawn.AreAllEntriesDiscoveredForBuilding(buildingName);

            if (allDiscovered)
            {
                Debug.Log($"Building {buildingName} completed. Unlocking region {i}.");
                UnlockRegion(i);
            }
            else
            {
                Debug.Log($"Building {buildingName} not yet completed.");
            }
        }
    }

    public void ActivateMapUI()
    {
        CheckAndUnlockRegions(); // 지역 해금 상태를 갱신
        LoadMapState();         // 지도 상태를 업데이트
        mapUI.SetActive(true);  // 지도 UI를 활성화
    }
}