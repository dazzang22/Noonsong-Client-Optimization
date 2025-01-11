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
        if (PlayerPrefs.GetInt("TutorialCompleted", 0) == 1)
        {
            // 튜토리얼이 완료되었으므로 main Scene으로 바로 이동
            SceneManager.LoadScene("Main Scene");
        }
        else
        {
            SceneManager.LoadScene("Merge-TutorialScene");
        }
    }
}