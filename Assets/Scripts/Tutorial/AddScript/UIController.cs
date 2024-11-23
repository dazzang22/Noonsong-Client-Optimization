using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Canvas friendsCanvas;
    [SerializeField] private Button friendsButton;
    // Start is called before the first frame update
    void Start()
    {
        friendsCanvas.gameObject.SetActive(false);
        friendsButton.onClick.AddListener(TogleFriendsCanvas);
    }

    private void TogleFriendsCanvas()
    {
        friendsCanvas.gameObject.SetActive(!friendsCanvas.gameObject.activeSelf);
    }
}
