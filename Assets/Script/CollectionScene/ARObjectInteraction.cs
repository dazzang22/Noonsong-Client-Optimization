using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using System.Collections.Generic;

public class ARObjectInteraction : MonoBehaviour
{
  public Camera arCamera;
  public Button catchButton;
  public NoonsongEntryManager noonsongEntryManager;
  public GameObject uiPopup;

  private GameObject currentTarget;

  void Start()
  {
    catchButton.onClick.AddListener(OnCatchButtonClick);
  }

  void Update()
  {
    DetectObjectInView();
  }

  void DetectObjectInView()
  {
    Ray ray = new Ray(arCamera.transform.position, arCamera.transform.forward);
    RaycastHit hit;

    if (Physics.Raycast(ray, out hit))
    {
      if (hit.collider.gameObject != currentTarget)
      {
        currentTarget = hit.collider.gameObject;
        Debug.Log("New target detected: " + currentTarget.name);
      }
    }
    else
    {
      currentTarget = null;
    }
  }

  void OnCatchButtonClick()
  {
    if (currentTarget != null)
    {
      NoonsongEntry entry = currentTarget.GetComponent<NoonsongEntry>();
      if (entry != null)
      {
        if (noonsongEntryManager.IsEntryInCollection(entry))
        {
          Debug.Log("Entry already in collection: " + entry.name);
        }
        else
        {
          noonsongEntryManager.AddNoonsongEntry(entry);
          ShowUIPopup(entry);
        }
      }
    }
  }

  void ShowUIPopup(NoonsongEntry entry)
  {
    uiPopup.SetActive(true);
    // Set popup content (e.g., entry name, description) here
    // Example: uiPopup.GetComponentInChildren<Text>().text = entry.name;
    Invoke("HideUIPopup", 3f); // Hide popup after 3 seconds
  }

  void HideUIPopup()
  {
    uiPopup.SetActive(false);
  }
}
