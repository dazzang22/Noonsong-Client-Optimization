using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BackEnd;

public class DevHiddenButton : MonoBehaviour
{
    public Button devButton;
    public TMP_Text debugText;

    private int clickCount = 0;
    private float lastClickTime = 0f;
    private float doubleClickThreshold = 0.3f;

    void Start()
    {
        devButton.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        float currentTime = Time.time;

        if (currentTime - lastClickTime > doubleClickThreshold)
        {
            clickCount = 0;
        }

        clickCount++;
        lastClickTime = currentTime;

        if (clickCount >= 5)
        {
            ActivateDevFeature();
            clickCount = 0;
        }
    }

    void ActivateDevFeature()
    {
        string googlehash = Backend.Utils.GetGoogleHash();
        Debug.Log("GoogleHashKey : " + googlehash);
        debugText.text = "GoogleHashKey: " + googlehash;
    }
}
