using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
  public static CurrencyManager Instance { get; private set; }

  public int noonsongCurrency;

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

  public bool HasEnoughCurrency(int amount)
  {
    return noonsongCurrency >= amount;
  }

  public void UseCurrency(int amount)
  {
    if (HasEnoughCurrency(amount))
    {
      noonsongCurrency -= amount;
    }
  }
}

