using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    private int playerCurrency = 0;
    [SerializeField] private List<TextMeshProUGUI> currencyTexts = new List<TextMeshProUGUI>(); // UI 텍스트 리스트

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
        UpdateCurrencyUI();
    }

    public int GetCurrencyAmount()
    {
        return playerCurrency;
    }

    public bool HasEnoughCurrency(int amount)
    {
        return playerCurrency >= amount;
    }

    public bool UseCurrency(int amount)
    {
        if (HasEnoughCurrency(amount))
        {
            playerCurrency -= amount;
            UpdateCurrencyUI();
            return true;
        }
        return false;
    }

    public void AddCurrency(int amount)
    {
        playerCurrency += amount;
        UpdateCurrencyUI();
    }

    public void SetCurrency(int amount)
    {
        playerCurrency = amount;
        UpdateCurrencyUI();
    }

    private void UpdateCurrencyUI()
    {
        foreach (var text in currencyTexts)
        {
            if (text != null)
            {
                text.text = $"{playerCurrency}";
            }
        }
    }

    public void RegisterCurrencyText(TextMeshProUGUI newCurrencyText)
    {
        if (!currencyTexts.Contains(newCurrencyText))
        {
            currencyTexts.Add(newCurrencyText);
            UpdateCurrencyUI();
        }
    }

    public void UnregisterCurrencyText(TextMeshProUGUI currencyText)
    {
        if (currencyTexts.Contains(currencyText))
        {
            currencyTexts.Remove(currencyText);
        }
    }
}

