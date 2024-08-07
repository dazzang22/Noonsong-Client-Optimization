using UnityEngine;
using UnityEngine.UI;

public class ScrollViewManager : MonoBehaviour
{
    public GameObject[] scrollViews;  // 3개의 스크롤뷰를 담을 배열
    public Button[] buttons;          // 3개의 버튼을 담을 배열
    public Sprite[] buttonSprites;    // 각 버튼에 할당할 이미지 (비활성/활성)

    void Start()
    {
        // 초기 설정: 첫 번째 스크롤뷰만 활성화
        for (int i = 0; i < scrollViews.Length; i++)
        {
            scrollViews[i].SetActive(i == 0);
        }

        // 버튼 클릭 이벤트 리스너 추가
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            Debug.Log($"Adding listener to button {index}");
            buttons[i].onClick.AddListener(() => OnButtonClicked(index));
        }
    }

    void OnButtonClicked(int index)
    {
        Debug.Log($"Button {index} clicked");

        for (int i = 0; i < scrollViews.Length; i++)
        {
            scrollViews[i].SetActive(i == index);
        }

        // 버튼 이미지 변경
        for (int i = 0; i < buttons.Length; i++)
        {
            Image buttonImage = buttons[i].GetComponent<Image>();
            if (buttonImage != null)
            {
                if (i * 2 < buttonSprites.Length)
                {
                    buttonImage.sprite = buttonSprites[i * 2]; // 비활성화 이미지
                    Debug.Log($"Button {i} set to inactive sprite.");
                }
                else
                {
                    Debug.LogError($"No inactive sprite for button {i}.");
                }
            }
            else
            {
                Debug.LogError($"No Image component found on button {i}.");
            }
        }

        Image activeButtonImage = buttons[index].GetComponent<Image>();
        if (activeButtonImage != null)
        {
            if (index * 2 + 1 < buttonSprites.Length)
            {
                activeButtonImage.sprite = buttonSprites[index * 2 + 1]; // 활성화 이미지
                Debug.Log($"Button {index} set to active sprite.");
            }
            else
            {
                Debug.LogError($"No active sprite for button {index}.");
            }
        }
        else
        {
            Debug.LogError($"No Image component found on button {index}.");
        }
    }
}
