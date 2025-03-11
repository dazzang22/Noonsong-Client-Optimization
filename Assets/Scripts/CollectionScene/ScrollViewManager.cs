using UnityEngine;
using UnityEngine.UI;

public class ScrollViewManager : MonoBehaviour
{
    public GameObject[] scrollViews;  
    public GameObject[] panels;       
    public Toggle[] toggles;          
    public ToggleGroup toggleGroup;

    void Start()
    {
        for (int i = 0; i < scrollViews.Length; i++)
        {
            scrollViews[i].SetActive(false);
            panels[i].SetActive(false);  
        }
        scrollViews[0].SetActive(true);
        panels[0].SetActive(true);  

        if (toggleGroup != null)
        {
            toggleGroup.SetAllTogglesOff();
        }

        toggles[0].isOn = true;
        toggles[0].onValueChanged.Invoke(true);

        Canvas.ForceUpdateCanvases();

        for (int i = 0; i < toggles.Length; i++)
        {
            int index = i;
            toggles[i].onValueChanged.AddListener((isOn) => OnToggleChanged(index, isOn));
        }
    }

    void OnToggleChanged(int index, bool isOn)
    {
        if (isOn)
        {
            Debug.Log($"Toggle {index} activated");

            for (int i = 0; i < scrollViews.Length; i++)
            {
                scrollViews[i].SetActive(false);
                panels[i].SetActive(false); 
            }
            scrollViews[index].SetActive(true);
            //if (index != 2)
            //{
                for (int i = 0; i < panels.Length; i++)
                {
                    panels[i].SetActive(false);
                }
                panels[index].SetActive(true);
           // }

            Canvas.ForceUpdateCanvases();
        }
    }
}

