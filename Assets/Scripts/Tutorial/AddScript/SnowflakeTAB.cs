using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.SceneManagement; 

public class SnowflakeTAB : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Image[] gauges;
    public float holdTime = 5f;
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
        {
            targetCanvas.SetActive(false);
        }

        // 현재 씬 이름이 "Merge-TutorialScene-Friends_noonsong"이 아닐 때만 실행
        if (SceneManager.GetActiveScene().name != "Merge-TutorialScene-Friends_noonsong")
        {
            if (CurrencyManager.Instance != null)
            {
                Button[] currencyButtons = CurrencyManager.Instance.GetCurrencyButtons();
                foreach (Button button in currencyButtons)
                {
                    string buttonCurrencyType = button.name;
                    button.onClick.AddListener(() => SwitchCurrency(buttonCurrencyType));
                }
            }
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

            if (holdCounter >= holdTime)
            {
                holdCounter = holdTime; 
                if (!soundPlayed) 
                {
                    onCatchButtonClicked?.Invoke();
                    soundPlayed = true; 
                }
            }
        }
        else
        {
            holdCounter = 0f;
            soundPlayed = false;
            foreach (var gauge in gauges)
            {
                gauge.gameObject.SetActive(false);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
        pressStartTime = Time.time;

        if (audioSource != null && clickSound != null && !audioSource.isPlaying)
        {
            audioSource.Play();
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
