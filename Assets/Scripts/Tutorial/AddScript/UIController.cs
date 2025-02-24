using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Canvas friendsCanvas;
    [SerializeField] private Canvas mapCanvas;
    [SerializeField] private Canvas skipCanvas;

    [SerializeField] private Button friendsButton;
    [SerializeField] private Button mapButton;
    [SerializeField] private Button skipButton;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    void Start()
    {
        friendsCanvas.gameObject.SetActive(false);
        mapCanvas.gameObject.SetActive(false);
        skipCanvas.gameObject.SetActive(false);

        friendsButton.onClick.AddListener(ToggleFriendsCanvas);
        mapButton.onClick.AddListener(ToggleMapCanvas);
        skipButton.onClick.AddListener(ToggleSkipCanvas);
        yesButton.onClick.AddListener(SkipTutorial);
        noButton.onClick.AddListener(HideSkipCanvas);
    }

    private void ToggleFriendsCanvas()
    {
        friendsCanvas.gameObject.SetActive(!friendsCanvas.gameObject.activeSelf);
    }

    private void ToggleMapCanvas()
    {
        mapCanvas.gameObject.SetActive(!mapCanvas.gameObject.activeSelf);
    }

    private void ToggleSkipCanvas()
    {
        skipCanvas.gameObject.SetActive(!skipCanvas.gameObject.activeSelf);
    }

    private void SkipTutorial()
    {
        // "MainScene"으로 전환
        SceneManager.LoadScene("MainScene(Release)");
    }

    private void HideSkipCanvas()
    {
        // skipCanvas 비활성화
        skipCanvas.gameObject.SetActive(false);
    }
}
