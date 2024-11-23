using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Canvas friendsCanvas;
    [SerializeField] private Canvas mapCanvas;
    [SerializeField] private Button friendsButton;
    [SerializeField] private Button mapButton;
    // Start is called before the first frame update
    void Start()
    {
        friendsCanvas.gameObject.SetActive(false);
        mapCanvas.gameObject.SetActive(false);
        friendsButton.onClick.AddListener(TogleFriendsCanvas);
        mapButton.onClick.AddListener(ToggleMapCanvas);
    }

    private void TogleFriendsCanvas()
    {
        friendsCanvas.gameObject.SetActive(!friendsCanvas.gameObject.activeSelf);
    }

    private void ToggleMapCanvas()
    {
        mapCanvas.gameObject.SetActive(!mapCanvas.gameObject.activeSelf);
    }
}
