using System.Collections;
using UnityEngine;

public class GoodsManager : MonoBehaviour
{
    [SerializeField]
    private CurrencyManager currencyManager;
    private const int DEFAULT_INCREMENT = 3;              // 추가할 재화의 양
    private const float INTERVAL = 300f;                   // 5분 (300초)

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

    // 눈꽃송이가 5분마다 3개의 눈의 결정 제공
    private IEnumerator AddCurrencyPeriodically()
    {
        while (true) // 계속 반복
        {
            yield return new WaitForSeconds(INTERVAL); // 5분 대기
            if (currencyManager != null)
            {
                currencyManager.IncreaseCurrency("Default", DEFAULT_INCREMENT);
                Debug.Log($"5분 경과: 눈꽃송이가 {DEFAULT_INCREMENT}만큼 재공.");
            }
        }
    }

    //파견 시스템
    
}
