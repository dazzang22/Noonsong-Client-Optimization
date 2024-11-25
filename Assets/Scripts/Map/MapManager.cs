using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject mapUI;
    //public GameObject messageUI;
    public GameObject[] regions;
    public bool[] regionUnlocked;
    private NoonsongManager noonsongManager;

    private Vector2 startTouchPosition;
    private Vector2 currentTouchPosition;
    private bool isScrolling = false;

    void Start()
    {
        noonsongManager = FindObjectOfType<NoonsongManager>();
        if (noonsongManager == null)
        {
            Debug.LogError("NoonsongManager could not be found in the scene!");
            return;
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

    private void UnlockRegion(int regionIndex)
    {
        if (!regionUnlocked[regionIndex])
        {
            regionUnlocked[regionIndex] = true;
            regions[regionIndex].SetActive(false);
        }
    }

    public void CheckAndUnlockRegions()
    {
        // 1구역 해금 조건
        if (noonsongManager.AreAllEntriesDiscoveredInCampus1())
        {
            Debug.Log("All items discovered in Campus 1! Unlocking Region 1...");
            UnlockRegion(0); // 0번 구역 해금
        }

        // 2구역 해금 조건
        if (noonsongManager.AreAllEntriesDiscoveredInCampus2())
        {
            Debug.Log("All items discovered in Campus 2! Unlocking Region 2...");
            UnlockRegion(1); // 1번 구역 해금
        }
    }
}