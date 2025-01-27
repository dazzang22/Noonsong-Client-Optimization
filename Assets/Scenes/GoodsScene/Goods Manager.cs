using System.Collections;
using UnityEngine;
using TMPro;

public class GoodsManager : MonoBehaviour
{
    [SerializeField]
    private CurrencyManager currencyManager;
    [SerializeField]
    private TextMeshProUGUI dispatchTimeText; // 남은 시간 텍스트

    private const int DEFAULT_INCREMENT = 3;   // 5분마다 추가할 재화 양
    private const float INTERVAL = 300f;       // 5분(300초)
    private const int DISPATCH_DURATION = 10; // 파견 총 시간(10분)
   private int remainingMinutes = DISPATCH_DURATION; // 초기 남은 시간(10분)

    private Coroutine dispatchCoroutine;      // 파견 코루틴 저장

    void Start()
    {
        // CurrencyManager가 연결되어 있는지 확인
        if (currencyManager == null)
        {
            currencyManager = CurrencyManager.Instance;
        }

        // 5분마다 재화를 추가하는 코루틴 시작
        StartCoroutine(AddCurrencyPeriodically());
    }

    // 5분마다 재화를 추가
    private IEnumerator AddCurrencyPeriodically()
    {
        while (true) // 반복
        {
            yield return new WaitForSeconds(INTERVAL); // 5분 대기
            if (currencyManager != null)
            {
                currencyManager.IncreaseCurrency("Default", DEFAULT_INCREMENT);
                Debug.Log($"5분 경과: {DEFAULT_INCREMENT}개의 재화 추가.");
            }
        }
    }

    // 파견 시스템
    public void Dispatch()
    {
        // 기존에 파견 중인 경우 중단
        if (dispatchCoroutine != null)
        {
            StopCoroutine(dispatchCoroutine);
        }

        // 파견 코루틴 시작
        dispatchCoroutine = StartCoroutine(HandleDispatch());
    }

    // 파견 처리 코루틴
    private IEnumerator HandleDispatch()
    {

        while (remainingMinutes > 0)
        {
            // 남은 시간을 텍스트에 표시
            if (dispatchTimeText != null)
            {
                dispatchTimeText.text = $"{remainingMinutes}";
            }

            yield return new WaitForSeconds(5f); // 1분 대기
            remainingMinutes--; // 남은 시간 감소
        }

        // 파견 완료 시 보상 지급
        if (currencyManager != null)
        {
            int randomReward = Random.Range(1, 11); // 1에서 10까지의 랜덤 값 생성 (포함)
            currencyManager.IncreaseCurrency("Default", randomReward);
            Debug.Log($"파견 완료: {randomReward}개의 재화 지급.");
        }

        dispatchCoroutine = null; // 코루틴 초기화
    }

    //복귀 완료 이미지로 변경 함수
    public void changeImage()
    {
        // 파견 소요 시간이 지나면 복귀 완료 이미지로 변경
        if (dispatchTimeText != null)
        {
            dispatchTimeText.text = "파견 완료!";
        }
    }
    

}
