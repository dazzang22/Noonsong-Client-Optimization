using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public GameObject openPanel;
    public GameObject closePanel;

    public void Start()
    {
        
        // PlayerPrefs.DeleteKey("TutorialCompleted");
    }

    public void TogglePanels()
    {
        if (openPanel != null)
        {
            openPanel.SetActive(true);
        }

        if (closePanel != null)
        {
            closePanel.SetActive(false);
        }
    }

    public void GoScene()
    {
        if (PlayerPrefs.GetInt("TutorialCompleted", 0) == 1)
        {
            // 튜토리얼이 완료되었으므로 main Scene으로 바로 이동
            SceneManager.LoadScene("MainScene(Release)");
        }
        else
        {
            SceneManager.LoadScene("Merge-TutorialScene");
        }
    }
}