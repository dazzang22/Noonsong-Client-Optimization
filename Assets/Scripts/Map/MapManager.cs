using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject UI;
    public GameObject mapUI;
    //public GameObject messageUI;
    public GameObject[] regions;
    public bool[] regionUnlocked;

    private Vector2 startTouchPosition;
    private Vector2 currentTouchPosition;
    private bool isScrolling = false;

    void Start()
    {
        LoadMapState();
        mapUI.SetActive(false);
    }

    void Update()
    {
        HandleTouchInput();
        HandleBackButton();
    }

    public void OpenMap()
    {
        UI.SetActive(false);
        mapUI.SetActive(true);
        Debug.Log("Map UI opened.");
    }

    public void CloseMap()
    {
        UI.SetActive(true);
        mapUI.SetActive(false);
        Debug.Log("Returning to game.");
    }

    private void LoadMapState()
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

    private void HandleBackButton()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMap();
        }
    }
}