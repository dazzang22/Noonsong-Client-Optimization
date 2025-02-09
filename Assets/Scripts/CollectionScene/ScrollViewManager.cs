using UnityEngine;
using UnityEngine.UI;

public class ScrollViewManager : MonoBehaviour
{
  public GameObject[] scrollViews;  // 각 스크롤뷰를 담은 배열
  public Toggle[] toggles;          // 각 토글을 담은 배열

  void Start()
  {
    // 초기 설정: 첫 번째 스크롤뷰만 활성화
    for (int i = 0; i < scrollViews.Length; i++)
    {
      scrollViews[i].SetActive(i == 0);
    }

    // 토글 값 변경 이벤트 리스너 추가
    for (int i = 0; i < toggles.Length; i++)
    {
      int index = i;
      toggles[i].onValueChanged.AddListener((isOn) => OnToggleChanged(index, isOn));
    }
  }

  void OnToggleChanged(int index, bool isOn)
  {
    // 토글이 활성화 되었을 때만 처리
    if (isOn)
    {
      Debug.Log($"Toggle {index} activated");

      // 해당 스크롤뷰만 활성화
      for (int i = 0; i < scrollViews.Length; i++)
      {
        scrollViews[i].SetActive(false);
      }
      scrollViews[index].SetActive(true);
    }
  }
}
