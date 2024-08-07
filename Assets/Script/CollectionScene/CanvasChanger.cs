using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasChanger: MonoBehaviour
{
  [SerializeField] private Canvas mainCanvas;
  [SerializeField] private Canvas popupCanvas;
  [SerializeField] private Button popupButton;
  [SerializeField] private Button backButton;

  private void Start()
  {
    mainCanvas.gameObject.SetActive(true);
    popupCanvas.gameObject.SetActive(false);

    popupButton.onClick.AddListener(ShowPopup);
    backButton.onClick.AddListener(HidePopup);
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
}
