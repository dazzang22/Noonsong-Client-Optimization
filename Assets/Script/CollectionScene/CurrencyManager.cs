using UnityEngine;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    public int noonsongCurrency;

    [SerializeField] private TextMeshProUGUI currencyText; // TextMeshPro UGUI 컴포넌트

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
        if (currencyText != null)
        {
            currencyText.text = noonsongCurrency.ToString();
        }
    }
}


