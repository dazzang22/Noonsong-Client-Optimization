using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    public int noonsongCurrency;

    // 여러 TextMeshProUGUI 컴포넌트를 관리하기 위한 리스트
    [SerializeField] private List<TextMeshProUGUI> currencyTexts = new List<TextMeshProUGUI>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateCurrencyUI(); // 초기화 시 UI 업데이트
    }

    public bool HasEnoughCurrency(int amount)
    {
        return noonsongCurrency >= amount;
    }

    public void UseCurrency(int amount)
    {
        if (HasEnoughCurrency(amount))
        {
            noonsongCurrency -= amount;
            UpdateCurrencyUI(); // 통화 사용 후 UI 업데이트
        }
    }

    public void AddCurrency(int amount)
    {
        noonsongCurrency += amount;
        UpdateCurrencyUI(); // 통화 추가 후 UI 업데이트
    }

    private void UpdateCurrencyUI()
    {
        // 등록된 모든 TextMeshProUGUI 컴포넌트를 업데이트
        foreach (var currencyText in currencyTexts)
        {
            if (currencyText != null)
            {
                currencyText.text = noonsongCurrency.ToString();
            }
        }
    }

    // TextMeshProUGUI 컴포넌트를 등록하는 메서드
    public void RegisterCurrencyText(TextMeshProUGUI newCurrencyText)
    {
        if (!currencyTexts.Contains(newCurrencyText))
        {
            currencyTexts.Add(newCurrencyText);
            newCurrencyText.text = noonsongCurrency.ToString(); // 등록 시 초기화
        }
    }
}


