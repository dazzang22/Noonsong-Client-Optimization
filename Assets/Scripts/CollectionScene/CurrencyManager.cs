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
                SetCurrency(currencyType, 998); 
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

    public bool HasEnoughCurrency(int amount)
    {
        return currencies.ContainsKey(activeCurrencyType) && currencies[activeCurrencyType] >= amount;
    }

    public void UseCurrency(int amount)
    {
        if (HasEnoughCurrency(amount))
        {
            currencies[activeCurrencyType] -= amount;
            UpdateCurrencyUI();
        }
    }

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

    public void SetCurrency(string currencyType, int amount)
    {
        if (!currencies.ContainsKey(currencyType))
        {
            currencies[currencyType] = 0;
        }
        currencies[currencyType] = amount;

        /*if (currencyType == activeCurrencyType)
        {
            UpdateCurrencyUI();
        }*/
        UpdateCurrencyUI();
    }

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

    public int GetCurrencyAmount(string currencyType)
    {
        if (currencies.ContainsKey(currencyType))
        {
            return currencies[currencyType];
        }
        return 0;
    }

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
}



