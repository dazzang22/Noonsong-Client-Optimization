using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class NoonsongManager : MonoBehaviour
{
  public GameObject discoveredEntryPrefab;    // 발견된 항목 프리팹
  public GameObject undiscoveredEntryPrefab;  // 발견되지 않은 항목 프리팹
  public GameObject placeholderPrefab;        // 빈 항목 프리팹
  public Transform entryParent;               // 도감 항목의 부모 오브젝트

  public GameObject categoryScrollView;       
  public GameObject entryScrollView;          
  public Button closeButton;
  public TextMeshProUGUI progressText;

  public GameObject detailsPanel;             // 엔트리 상세 정보를 표시할 패널
  public TextMeshProUGUI detailsNameText;     // 상세 정보의 이름 텍스트
  public TextMeshProUGUI detailsDescriptionText;
  public Image detailsImage;
  public Button view3DButton;
  public Canvas collectionCanvas;
  public Canvas cameraCanvas;
  public Camera renderCamera;
  public Button[] categoryButtons;
  public string selectedCategory = "All";

  [SerializeField]
  private NoonsongEntryManager noonsongEntryManager;

  private List<NoonsongEntry> entries;
  private GameObject lastClickedEntry;
  private GameObject currentNoonsongObject;

  private bool is3DViewActive = false;

    [SerializeField] private Image[] loveMeterImages;
    [SerializeField] private Sprite loveSprite1;
    [SerializeField] private Sprite loveSprite2;

    void Start()
    {
    if (noonsongEntryManager != null)
    {
      entries = new List<NoonsongEntry>(noonsongEntryManager.GetNoonsongEntries());
    }
    else
    {
      Debug.LogError("NoonsongEntryManager is not assigned in the inspector!");
    }

    foreach (var button in categoryButtons)
    {
      button.onClick.AddListener(() => OnCategoryButtonClicked(button.name));
    }

    if (closeButton != null)
    {
      closeButton.onClick.AddListener(ShowCategoryScrollView);
    }

    ShowCategoryScrollView();
  }
  public void ShowCategoryScrollView()
  {
    categoryScrollView.SetActive(true);
    entryScrollView.SetActive(false);
    if (progressText != null)
    {
      progressText.gameObject.SetActive(false);
    }
  }

  public bool View3DButtonPressed()
    {
        // 3D 뷰가 활성화되었음을 알려줌
        is3DViewActive = true;
        Debug.Log("View 3D Button pressed.");
        return true;
    }

  public void OnCategoryButtonClicked(string category)
  {
    selectedCategory = category;
    categoryScrollView.SetActive(false); // 카테고리 선택 화면 숨기기
    entryScrollView.SetActive(true); // 도감 엔트리 목록 보이기
    PopulateNoonsong();
  }

  public void PopulateNoonsong()
  {
    foreach (Transform child in entryParent)
    {
      if (child.gameObject != closeButton.gameObject)
      {
        Destroy(child.gameObject);
      }
    }

    closeButton.transform.SetAsFirstSibling();

    int totalItems = 0;
    int discoveredItems = 0;

    foreach (var entry in entries)
    {
      if (selectedCategory == "All" || entry.university == selectedCategory)
      {
        GameObject newEntry;
        if (entry.isDiscovered)
        {
          newEntry = Instantiate(discoveredEntryPrefab, entryParent);
          discoveredItems++; 

          Button button = newEntry.GetComponent<Button>() ?? newEntry.AddComponent<Button>();
          button.onClick.RemoveAllListeners();
          button.onClick.AddListener(() => ShowDetails(entry, newEntry));

          var noonsongImage = newEntry.transform.Find("NoonsongImage").GetComponent<Image>();
          if (noonsongImage != null)
          {
            noonsongImage.sprite = entry.noonsongSprite;
          }
        }
        else
        {
          newEntry = Instantiate(undiscoveredEntryPrefab, entryParent);
        }
        totalItems++;  
      }
    }

    if (totalItems < 4)
    {
      int placeholdersNeeded = 4 - totalItems;
      for (int i = 0; i < placeholdersNeeded; i++)
      {
        Instantiate(placeholderPrefab, entryParent);
      }
    }

    closeButton.transform.SetAsFirstSibling();

    if (progressText != null)
    {
      progressText.text = $"{discoveredItems} / {totalItems}";
      progressText.gameObject.SetActive(true);
    }
  }

  public void OnCloseButtonClicked()
  {
    ShowCategoryScrollView();
  }


  public void SetAllEntriesDiscovered()
    {
        if (noonsongEntryManager != null)
        {
            entries = new List<NoonsongEntry>(noonsongEntryManager.GetNoonsongEntries());
        }

        foreach (var entry in entries)
        {
            if (entry != null)
            {
                entry.isDiscovered = true;
                Debug.Log($"Entry '{entry.noonsongName}' has been marked as discovered.");
            }
        }

        PopulateNoonsong();
    }

    public bool AreAllItemsDiscoveredInCategory(string category)
    {
        foreach (var entry in entries)
        {
            if (entry.university == category && !entry.isDiscovered)
            {
                return false;
            }
        }
        return true;
    }


    void AddEventTriggerListener(EventTrigger trigger, EventTriggerType eventType, System.Action action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener((eventData) =>
        {
            action();
            ExecuteEvents.ExecuteHierarchy<IScrollHandler>(
                ((PointerEventData)eventData).pointerPress,
                (PointerEventData)eventData,
                ExecuteEvents.scrollHandler
            );
        });
        trigger.triggers.Add(entry);
    }



    public void DiscoverItem(NoonsongEntry entry)
    {
        entry.isDiscovered = true;
        PopulateNoonsong();
    }

    void ShowDetails(NoonsongEntry entry, GameObject clickedEntry)
    {
        Debug.Log($"ShowDetails called for entry: {entry.noonsongName}");

        if (lastClickedEntry != null && lastClickedEntry != clickedEntry)
        {
            var lastBackground = lastClickedEntry.transform.Find("BackgroundImage").gameObject;
            var lastHighlight = lastClickedEntry.transform.Find("HighlightBackground").gameObject;

            if (lastBackground != null && lastHighlight != null)
            {
                lastBackground.SetActive(true);
                lastHighlight.SetActive(false);
            }
        }

        if (detailsPanel != null)
        {
            detailsPanel.SetActive(true);
            if (detailsNameText != null)
            {
                detailsImage.sprite = entry.noonsongSprite;

                if (entry.loveLevel >= 20)
                {
                    detailsNameText.text = entry.noonsongName;
                }
                else
                {
                    detailsNameText.text = "???";
                }

                if (entry.loveLevel >= 50)
                {
                    detailsDescriptionText.text = entry.description;
                }
                else
                {
                    detailsDescriptionText.text = "???";
                }

            }

            if (view3DButton != null)
            {
                view3DButton.onClick.RemoveAllListeners();
                view3DButton.onClick.AddListener(() =>
                {
                    View3DButtonPressed();
                    Open3DView(entry);

                });
                Debug.Log("3D View button click event added!");
            }
            UpdateLoveMeter(entry.loveLevel);
        }
        else
        {
            Debug.LogError("DetailsPanel is not set in the inspector!");
        }

        var background = clickedEntry.transform.Find("BackgroundImage").gameObject;
        var highlight = clickedEntry.transform.Find("HighlightBackground").gameObject;

        if (background != null && highlight != null)
        {
            background.SetActive(false);
            highlight.SetActive(true);
        }

        lastClickedEntry = clickedEntry;
    }

    private void UpdateLoveMeter(int loveLevel)
    {
        for (int i = 0; i < loveMeterImages.Length; i++)
        {
            loveMeterImages[i].enabled = false;
            loveMeterImages[i].sprite = loveSprite1;
        }

        for (int i = 0; i < loveMeterImages.Length; i++)
        {
            if (loveLevel >= (i + 1) * 10)
            {
                loveMeterImages[i].enabled = true;

                if (loveLevel >= 60)  // 60 이상이면 loveSprite2 적용
                    loveMeterImages[i].sprite = loveSprite2;
                else
                    loveMeterImages[i].sprite = loveSprite1;
            }
        }
    }


    public void Open3DView(NoonsongEntry entry)
    {
        if (collectionCanvas != null)
        {
            collectionCanvas.gameObject.SetActive(false);
        }

        if (cameraCanvas != null)
        {
            cameraCanvas.gameObject.SetActive(true);
        }

        if (renderCamera != null && entry.prefab != null)
        {
            // 카메라 뷰포트에서 중앙 좌표 설정 (0.5, 0.5)
            float randomX = 0.5f;
            float randomY = 0.5f;

            // 휴대폰 화면의 중앙 좌표를 월드 좌표로 변환
            Vector3 randomViewportPosition = new Vector3(randomX, randomY, renderCamera.nearClipPlane + 2f);
            Vector3 randomWorldPosition = renderCamera.ViewportToWorldPoint(randomViewportPosition);

            // 3D 오브젝트 생성 및 참조 저장
            currentNoonsongObject = Instantiate(entry.prefab, randomWorldPosition, Quaternion.identity);

            //3D 오브젝트 position 설정
            currentNoonsongObject.transform.position = new Vector3(0, -3f, -5f);

            //3D 오브젝트 scale 설정
            currentNoonsongObject.transform.localScale = new Vector3(4f, 4f, 4f);

            // 오브젝트가 카메라를 바라보게 설정
            currentNoonsongObject.transform.LookAt(renderCamera.transform);

            // PhotoManager 스크립트를 추가하여 오브젝트 조작 가능하게 설정
            var photoManager = currentNoonsongObject.AddComponent<PhotoManager>();

        }
    }
    public void OnBackButtonPressed()
    {
        if (cameraCanvas != null)
        {
            cameraCanvas.gameObject.SetActive(false);
        }

        if (collectionCanvas != null)
        {
            collectionCanvas.gameObject.SetActive(true);
        }

        if (currentNoonsongObject != null)
        {
            Destroy(currentNoonsongObject);
        }
        is3DViewActive = false;
        Debug.Log("Back Button pressed, 3D View is now inactive.");
    }
    public bool Is3DViewActive()
    {
        return is3DViewActive;
    }
}
