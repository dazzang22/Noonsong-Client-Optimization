using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    private Dictionary<string, int> currencies = new Dictionary<string, int>();
    private string activeCurrencyType = "Default";

    [SerializeField] private List<TextMeshProUGUI> currencyTexts = new List<TextMeshProUGUI>();
    [SerializeField] private Button[] currencyButtons;

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
        foreach (var button in currencyButtons)
        {
            string currencyType = button.name;
            if (!currencies.ContainsKey(currencyType))
            {
                SetCurrency(currencyType, 998); // 기본적으로 998로 설정
            }
        }

        UpdateCurrencyUI();
    }

    public Button[] GetCurrencyButtons()
    {
        return currencyButtons;
    }
    public void SetAllCurrenciesTo999(Button[] currencyButtons)
    {
        foreach (var button in currencyButtons)
        {
            string currencyType = button.name;

            if (!currencies.ContainsKey(currencyType))
            {
                SetCurrency(currencyType, 999);
            }
            else
            {
                SetCurrency(currencyType, 999);
            }
        }
        UpdateCurrencyUI();
    }

    public void SwitchCurrencyType(string currencyType)
    {
        if (!currencies.ContainsKey(currencyType))
        {
            currencies[currencyType] = 0;
        }
        activeCurrencyType = currencyType;
        UpdateCurrencyUI();
    }

    // 특정 화폐(대학명)에 대한 충분한 양이 있는지 확인
    public bool HasEnoughCurrency(string currencyType, int amount)
    {
        return currencies.ContainsKey(currencyType) && currencies[currencyType] >= amount;
    }

    // 특정 화폐(대학명)을 사용
    public void UseCurrency(string currencyType, int amount)
    {
        if (HasEnoughCurrency(currencyType, amount))
        {
            currencies[currencyType] -= amount;
            UpdateCurrencyUI();
        }
    }

    // 특정 화폐에 양 추가
    public void AddCurrency(string currencyType, int amount)
    {
        if (!currencies.ContainsKey(currencyType))
        {
            currencies[currencyType] = 0;
        }
        currencies[currencyType] += amount;

        if (currencyType == activeCurrencyType)
        {
            UpdateCurrencyUI();
        }
    }

    // 특정 화폐의 양 설정
    public void SetCurrency(string currencyType, int amount)
    {
        if (!currencies.ContainsKey(currencyType))
        {
            currencies[currencyType] = 0;
        }
        currencies[currencyType] = amount;
        UpdateCurrencyUI();
    }

    // 화폐 양 증가
    public void IncreaseCurrency(string currencyType, int amount)
    {
        if (!currencies.ContainsKey(currencyType))
        {
            currencies[currencyType] = 0;
        }
        currencies[currencyType] += amount;

        if (currencyType == activeCurrencyType)
        {
            UpdateCurrencyUI();
        }
    }

    // 화폐 양 감소
    public bool DecreaseCurrency(string currencyType, int amount)
    {
        if (currencies.ContainsKey(currencyType) && currencies[currencyType] >= amount)
        {
            currencies[currencyType] -= amount;
            if (currencyType == activeCurrencyType)
            {
                UpdateCurrencyUI();
            }
            return true;
        }
        return false;
    }

    // 특정 화폐의 양 가져오기
    public int GetCurrencyAmount(string currencyType)
    {
        if (currencies.ContainsKey(currencyType))
        {
            return currencies[currencyType];
        }
        return 0;
    }

    // UI 업데이트
    private void UpdateCurrencyUI()
    {
        foreach (var currencyText in currencyTexts)
        {
            if (currencyText != null)
            {
                currencyText.text = currencies.ContainsKey(activeCurrencyType)
                    ? currencies[activeCurrencyType].ToString()
                    : "0";
            }
        }
    }

    // 새 화폐 텍스트 등록
    public void RegisterCurrencyText(TextMeshProUGUI newCurrencyText)
    {
        if (!currencyTexts.Contains(newCurrencyText))
        {
            currencyTexts.Add(newCurrencyText);
            newCurrencyText.text = currencies.ContainsKey(activeCurrencyType)
                ? currencies[activeCurrencyType].ToString()
                : "0";
        }
    }
    public string GetActiveCurrencyType()
    {
        return activeCurrencyType;
    }

}
