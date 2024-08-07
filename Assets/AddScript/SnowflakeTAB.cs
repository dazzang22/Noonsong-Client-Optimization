using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// 이 클래스는 버튼을 길게 누를 때 게이지를 채우고, 버튼을 떼면 게이지를 초기화하는 기능을 함
public class SnowflakeTAB : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // 게이지 이미지 배열
    public Image[] gauges;
    // 게이지가 하나씩 채워지는 데 걸리는 시간
    public float holdTime = 1f;
    // 버튼이 눌려 있는지 여부를 나타내는 변수
    private bool isHolding = false;
    // 버튼이 눌려진 시간을 누적하는 변수
    private float holdCounter = 0f;

    void Update()
    {
        // 버튼이 눌려있는 상태라면
        if (isHolding)
        {
            // 시간을 누적
            holdCounter += Time.deltaTime;
            // 현재 게이지 인덱스를 계산
            int gaugeIndex = Mathf.FloorToInt(holdCounter / holdTime * gauges.Length);
            // 게이지 이미지를 활성화 또는 비활성화
            for (int i = 0; i < gauges.Length; i++)
            {
                gauges[i].gameObject.SetActive(i <= gaugeIndex);
            }
            // 게이지가 모두 채워졌다면, 최대 시간을 유지
            if (gaugeIndex >= gauges.Length - 1)
            {
                holdCounter = holdTime * gauges.Length;
            }
        }
        else
        {
            // 버튼이 눌려있지 않다면, 시간을 초기화하고 모든 게이지를 비활성화
            holdCounter = 0f;
            foreach (var gauge in gauges)
            {
                gauge.gameObject.SetActive(false);
            }
        }
    }

    // 버튼이 눌렸을 때 호출
    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
    }

    // 버튼이 떼어졌을 때 호출
    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
    }
}