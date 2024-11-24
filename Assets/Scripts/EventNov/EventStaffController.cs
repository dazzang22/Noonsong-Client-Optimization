using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventStaffController : MonoBehaviour
{
    private int buttonClickCount = 0;

    [SerializeField]
    private Button targetButton;


    private const int requiredClicks = 5; 

    void Start()
    {
        if (targetButton != null)
        {
            targetButton.onClick.AddListener(OnButtonClick); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 추가적인 로직이 필요하면 여기에 작성
    }

    private void OnButtonClick()
    {
        buttonClickCount++; 

        if (buttonClickCount >= requiredClicks)
        {
            //통화 999로 초기화
            Button[] currencyButtons = CurrencyManager.Instance.GetCurrencyButtons();
            CurrencyManager.Instance.SetAllCurrenciesTo999(currencyButtons);

            //도감 전체 해금
            NoonsongEntryManager entryManager = FindObjectOfType<NoonsongEntryManager>();  
            if (entryManager != null)
            {
                entryManager.SetAllEntriesDiscovered(); 
            }
            else
            {
                Debug.LogError("NoonsongEntryManager not found in the scene.");
            }

            // 클릭 횟수 초기화
            buttonClickCount = 0;  
        }
    }
}

