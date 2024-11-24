using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class SnowflakeTAB : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Image[] gauges;
    public float holdTime = 1f;
    private bool isHolding = false;
    private float holdCounter = 0f;
    private AudioSource audioSource;
    public AudioClip clickSound;
    private bool soundPlayed = false;

    public UnityEvent onCatchButtonClicked;

    [SerializeField]
    private GameObject targetCanvas;

    private float pressStartTime = 0f;
    private float shortPressThreshold = 0.3f;


    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clickSound;
        audioSource.playOnAwake = false;

        if (onCatchButtonClicked == null)
            onCatchButtonClicked = new UnityEvent();

        if (targetCanvas != null)
            targetCanvas.SetActive(false);

        Button[] currencyButtons = CurrencyManager.Instance.GetCurrencyButtons();

        foreach (Button button in currencyButtons)
        {
            string buttonCurrencyType = button.name;
            button.onClick.AddListener(() => SwitchCurrency(buttonCurrencyType));
        }
    }

    void Update()
    {
        if (isHolding)
        {
            holdCounter += Time.deltaTime;
            int gaugeIndex = Mathf.FloorToInt(holdCounter / holdTime * gauges.Length);

            for (int i = 0; i < gauges.Length; i++)
            {
                gauges[i].gameObject.SetActive(i <= gaugeIndex);
            }

            if (gaugeIndex >= gauges.Length - 1)
            {
                holdCounter = holdTime * gauges.Length;
            }
        }
        else
        {
            holdCounter = 0f;
            foreach (var gauge in gauges)
            {
                gauge.gameObject.SetActive(false);
            }
        }

        if (soundPlayed && !audioSource.isPlaying)
        {
            soundPlayed = false;
            onCatchButtonClicked?.Invoke();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
        pressStartTime = Time.time;

        if (audioSource != null && clickSound != null && !audioSource.isPlaying)
        {
            audioSource.Play();
            soundPlayed = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;

        float pressDuration = Time.time - pressStartTime;

        if (pressDuration <= shortPressThreshold)
        {
            HandleShortPress();
        }

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            soundPlayed = false;
        }
    }

    private void HandleShortPress()
    {
        Debug.Log("Short press detected!");
        if (targetCanvas != null)
        {
            targetCanvas.SetActive(!targetCanvas.activeSelf);
        }
    }

    private void SwitchCurrency(string currencyType)
    {
        Debug.Log($"Switching to currency type: {currencyType}");
        CurrencyManager.Instance.SwitchCurrencyType(currencyType);
    }
}
