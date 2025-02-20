using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARObjectCatch : MonoBehaviour
{
    private PlayerObjectSpawn playerObjectSpawn;

    [SerializeField]
    private NoonsongManager noonsongManager;

    [SerializeField]
    private CurrencyManager currencyManager;

    private GameObject currentTarget;

    public GameObject noonsongPrefeb;

    private const int generalNoonsongCost = 5;

    [SerializeField] private Button catchButton;
    [SerializeField] private EncounterUI encounterUI;
    [SerializeField] private GameObject exitPopup;


    void Start()
    {
        catchButton.onClick.AddListener(OnCatchButtonClicked);
    }

    void Update()
    {
        UpdateActivePlayerObjectSpawn();

        if (playerObjectSpawn != null)
        {
            CheckForObjectInView();
        }
    }


    void UpdateActivePlayerObjectSpawn()
    {
        var activeControllers = FindObjectsOfType<ScriptActivationController>();
        foreach (var controller in activeControllers)
        {
            if (controller.IsActive())
            {
                playerObjectSpawn = controller.GetComponentInChildren<PlayerObjectSpawn>();
                return;
            }
        }

        playerObjectSpawn = null;
    }

    void CheckForObjectInView()
    {
        if (playerObjectSpawn != null && playerObjectSpawn.SpawnedObjects.Count > 0)
        {
            foreach (var obj in playerObjectSpawn.SpawnedObjects)
            {
                GameObject target = obj.GameObject;
                if (target == null)
                    continue;

                Vector3 objectPosition = target.transform.position;

                // 기본 반지름 설정 (콜라이더 없을 때 대비)
                float boundingRadius = 0.5f;

                // 콜라이더가 있으면 크기 기반으로 반지름 계산
                Collider col = target.GetComponent<Collider>();
                if (col != null)
                {
                    boundingRadius = col.bounds.extents.magnitude * 0.5f; 
                }

                Vector3[] checkPoints = new Vector3[]
                {
                objectPosition,  
                objectPosition + new Vector3(boundingRadius, 0, 0), 
                objectPosition - new Vector3(boundingRadius, 0, 0), 
                objectPosition + new Vector3(0, boundingRadius, 0), 
                objectPosition - new Vector3(0, boundingRadius, 0)  
                };

                bool isVisible = false;
                foreach (Vector3 point in checkPoints)
                {
                    Vector3 screenPoint = Camera.main.WorldToScreenPoint(point);

                    if (screenPoint.z > 0 &&
                        screenPoint.x > -50 && screenPoint.x < Screen.width + 50 &&
                        screenPoint.y > -50 && screenPoint.y < Screen.height + 50)
                    {
                        isVisible = true;
                        break;
                    }
                }

                if (isVisible)
                {
                    currentTarget = target;

                    Vector3 lookPosition = Camera.main.transform.position - target.transform.position;
                    lookPosition.y = 0;
                    Quaternion targetRotation = Quaternion.LookRotation(lookPosition);
                    target.transform.rotation = Quaternion.Slerp(target.transform.rotation, targetRotation, Time.deltaTime * 5);

                    return;
                }
            }
            currentTarget = null;
        }
        else
        {
            currentTarget = null;
        }
    }

    void OnCatchButtonClicked()
    {
        Debug.Log("Catch 버튼 클릭됨!");

        if (encounterUI.gameObject.activeSelf)
        {
            encounterUI.ShowExitConfirmation();
            return;
        }

        if (currentTarget != null && currentTarget.name == "noonsong remake 0202(Clone)")
        {
            encounterUI.ShowDefaultDialogue(noonsongPrefeb, () => {
                Debug.Log("기본 대화 종료 후 로직 실행");
            });
            return;
        }

        if (currentTarget != null)
        {
            Debug.Log($"현재 타겟: {currentTarget.name}");

            var spawnedObject = playerObjectSpawn.SpawnedObjects.Find(obj => obj.GameObject == currentTarget);
            if (spawnedObject != null)
            {
                NoonsongEntry entry = spawnedObject.NoonsongEntry;
                if (entry != null)
                {
                    encounterUI.Show(entry, () => {
                        Debug.Log("대화 종료 후 캐릭터 수집 실행");
                    });
                }
                else
                {
                    Debug.LogWarning("NoonsongEntry가 존재하지 않음!");
                }
            }
            else
            {
                Debug.LogWarning("SpawnedObjects 목록에서 currentTarget을 찾을 수 없음!");
            }
        }
        else
        {
            Debug.LogWarning("currentTarget이 null!");
        }
    }

    /*
else if (entry == null && currencyManager.GetActiveCurrencyType() == "Default")
{
if (currencyManager.HasEnoughCurrency("Default", generalNoonsongCost))
{
currencyManager.UseCurrency("Default", generalNoonsongCost);
Destroy(currentTarget);
playerObjectSpawn.SpawnedObjects.Remove(spawnedObject);
}
else
{
Debug.Log("Not enough currency to catch the generalNoonsong.");
}
}*/

    public void CollectCharacter()
    {
        if (currentTarget != null)
        {
            var spawnedObject = playerObjectSpawn.SpawnedObjects.Find(obj => obj.GameObject == currentTarget);
            if (spawnedObject != null)
            {
                NoonsongEntry entry = spawnedObject.NoonsongEntry;

                if (entry != null)
                {
                    int requiredCurrency = entry.requiredNoonsongs;

                    if (!entry.isDiscovered)
                    {
                        Debug.Log($"첫 번째 발견: {entry.noonsongName}");

                        noonsongManager.DiscoverItem(entry);
                        entry.isDiscovered = true;
                        currencyManager.UseCurrency(requiredCurrency);
                    }
                    else
                    {
                        Debug.LogWarning($"이미 발견된 눈송이: {entry.noonsongName}");
                    }
                }

                UpdateActivePlayerObjectSpawn();
                CheckForObjectInView();
            }
        }
    }
    public GameObject GetCurrentTarget()
    {
        return currentTarget;
    }

    private void CloseEncounterCallback()
    {
        Debug.Log("Encounter UI가 닫혔습니다.");
        exitPopup.SetActive(false);
    }
}