using UnityEngine;
using UnityEngine.UI;

public class ScrollViewManager : MonoBehaviour
{
  public GameObject[] scrollViews;  // 각 스크롤뷰를 담은 배열
  public Toggle[] toggles;          // 각 토글을 담은 배열
  public ToggleGroup toggleGroup;   

  void Start()
  {
    for (int i = 0; i < scrollViews.Length; i++)
    {
      scrollViews[i].SetActive(false);
    }
    scrollViews[0].SetActive(true);

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
      }
      scrollViews[index].SetActive(true);
      Canvas.ForceUpdateCanvases();
    }
  }
}
