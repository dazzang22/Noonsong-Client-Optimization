using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasChanger : MonoBehaviour
{
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Canvas popupCanvas;
    [SerializeField] private Canvas turyCanvas;
    [SerializeField] private Button popupButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button turyButton;

    private void Start()
    {
        mainCanvas.gameObject.SetActive(true);
        popupCanvas.gameObject.SetActive(false);
        turyCanvas.gameObject.SetActive(false);

        popupButton.onClick.AddListener(ShowPopup);
        backButton.onClick.AddListener(HidePopup);
        turyButton.onClick.AddListener(ToggleTuryCanvas); 
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
}
