using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public GameObject popupPanel;

    public void ShowPopup()
    {
        popupPanel.SetActive(true);
    }

    public void NoShowPopup()
    {
        popupPanel.SetActive(false);
    }

    public void GoScene()
    {
        SceneManager.LoadScene("Merge-TutorialScene");
    }
}