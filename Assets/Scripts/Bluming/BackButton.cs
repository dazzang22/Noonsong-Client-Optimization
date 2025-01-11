using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    public Button moveSceneButton;
    public string sceneToLoad;

    void Start()
    {
        if (moveSceneButton != null)
        {
            moveSceneButton.onClick.AddListener(OnButtonClicked);
        }
        else
        {
            Debug.LogError("버튼이 연결되지 않았습니다!");
        }
    }

    void OnButtonClicked()
    {
        Debug.Log("버튼 클릭! 씬 이동: " + sceneToLoad);
        SceneManager.LoadScene(sceneToLoad);
    }
}