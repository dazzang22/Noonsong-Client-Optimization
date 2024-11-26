using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventStaffController : MonoBehaviour
{
    private int buttonClickCount = 0;

    [SerializeField]
    private Button targetButton;

    [SerializeField]
    private float timeLimit = 5f; // 제한 시간 (초)

    private float timer = 0f;
    private bool isTimerActive = false;

    private const int requiredClicks = 5;

    void Start()
    {
        if (targetButton != null)
        {
            targetButton.onClick.AddListener(OnButtonClick);
        }
    }

    void Update()
    {
        if (isTimerActive)
        {
            timer += Time.deltaTime;

            if (timer >= timeLimit)
            {
                ResetClickCount();
            }
        }
    }

    private void OnButtonClick()
    {
        if (!isTimerActive)
        {
            isTimerActive = true;
            timer = 0f;
        }

        buttonClickCount++;

        if (buttonClickCount >= requiredClicks)
        {
            PerformEventActions();
            ResetClickCount();
        }
    }

    private void PerformEventActions()
    {
        //재화 999개로 설정
        Button[] currencyButtons = CurrencyManager.Instance.GetCurrencyButtons();
        CurrencyManager.Instance.SetAllCurrenciesTo999(currencyButtons);

        //모든 눈송이 발견 처리
        NoonsongManager entryManager = FindObjectOfType<NoonsongManager>();
        if (entryManager != null)
        {
            entryManager.SetAllEntriesDiscovered();
        }
        else
        {
            Debug.LogError("NoonsongEntryManager not found in the scene.");
        }

        //지도 잠금 해제
        MapManager mapManager = FindObjectOfType<MapManager>();
        if (mapManager != null)
        {
            mapManager.UnlockRegion(0);
            mapManager.UnlockRegion(1);
        }
        else
        {
            Debug.LogError("mapManager not found in the scene.");
        }
    }

    private void ResetClickCount()
    {
        buttonClickCount = 0;
        timer = 0f;
        isTimerActive = false;
    }
}


