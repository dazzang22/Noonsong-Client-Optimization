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

    //�׽�Ʈ�� �ڵ�
    [SerializeField] private NoonsongEntry testNoonsong;

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

                Vector3 screenPoint = Camera.main.WorldToViewportPoint(target.transform.position);
                bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

                if (onScreen)
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
            Debug.Log("SpawnedObjects is null or empty.");
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

        if (currentTarget != null && currentTarget.name == "nunsong(Clone)")
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
                        CollectCharacter();
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

    void CollectCharacter()
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
                        if (currencyManager.HasEnoughCurrency(requiredCurrency))
                        {
                            noonsongManager.DiscoverItem(entry);
                            entry.isDiscovered = true;
                            currencyManager.UseCurrency(requiredCurrency);
                        }
                        else
                        {
                            Debug.Log("Not enough currency to discover this item.");
                        }
                    }
                }

                Destroy(currentTarget);
                playerObjectSpawn.SpawnedObjects.Remove(spawnedObject);

                currentTarget = null;
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