using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasChanger : MonoBehaviour
{
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Canvas popupCanvas;
    [SerializeField] private Canvas turyCanvas;
    [SerializeField] private Canvas mapCanvas;
    [SerializeField] private Button popupButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button turyButton;
    [SerializeField] private Button turyBackButton;
    [SerializeField] private Button mapButton;
    [SerializeField] private Button mapbackButton;

    public MapManager mapManager;

    private void Start()
    {
        mainCanvas.gameObject.SetActive(true);
        popupCanvas.gameObject.SetActive(false);
        turyCanvas.gameObject.SetActive(false);
        mapCanvas.gameObject.SetActive(false);

        popupButton.onClick.AddListener(ShowPopup);
        backButton.onClick.AddListener(HidePopup);
        turyButton.onClick.AddListener(ToggleTuryCanvas);
        turyBackButton.onClick.AddListener(ToggleTuryCanvas);
        mapButton.onClick.AddListener(ToggleMapCanvas);
        mapbackButton.onClick.AddListener(ToggleMapCanvas);

    }

    private void ShowPopup()
    {
        popupCanvas.gameObject.SetActive(true);
        mainCanvas.gameObject.SetActive(false);
    }

    private void HidePopup()
    {
        mainCanvas.gameObject.SetActive(true);
        popupCanvas.gameObject.SetActive(false);
    }

    private void ToggleTuryCanvas()
    {
        // 현재 활성 상태를 반전
        turyCanvas.gameObject.SetActive(!turyCanvas.gameObject.activeSelf);
    }

    private void ToggleMapCanvas()
    {
        mapCanvas.gameObject.SetActive(!mapCanvas.gameObject.activeSelf);
        mapManager.LoadMapState();
    }
}
