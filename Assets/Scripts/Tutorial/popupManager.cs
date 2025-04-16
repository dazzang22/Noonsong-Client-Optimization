using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class popupManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.DeleteKey("DispatchPopupActivated"); //테스트용
        PlayerPrefs.DeleteKey("CameraPopupActivated"); //테스트용
        PlayerPrefs.DeleteKey("MainTutoPopupActivated"); //테스트용
        PlayerPrefs.DeleteKey("TutorialCompleted");

    }

}
