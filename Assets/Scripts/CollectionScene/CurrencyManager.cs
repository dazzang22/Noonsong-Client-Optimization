using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CurrencyManager : MonoBehaviour
{
  public static CurrencyManager Instance { get; private set; }

  private int playerCurrency = 1000; // ⭐ 단일 화폐 (초기값 1000)
  [SerializeField] private TextMeshProUGUI currencyText; // UI 텍스트

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
    if (currencyText != null)
    {
      currencyText.text = $"{playerCurrency} Gold";
    }
  }

  public void RegisterCurrencyText(TextMeshProUGUI newCurrencyText)
  {
    currencyText = newCurrencyText;
    UpdateCurrencyUI();
  }
}
