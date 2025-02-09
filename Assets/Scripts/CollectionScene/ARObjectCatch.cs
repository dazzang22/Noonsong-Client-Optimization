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

    [SerializeField]
    private Button catchButton;

    [SerializeField]
    private EncounterUI encounterUI;

    private GameObject currentTarget;

    private const int generalNoonsongCost = 5;

    //테스트용 코드
    [SerializeField] private NoonsongEntry testNoonsong;


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

        //테스트용 코드
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TestEncounter();
        }
    }

    //테스트용 코드
    void TestEncounter()
    {
        if (testNoonsong != null)
        {
            Debug.Log("테스트 캐릭터 조우 UI 실행");
            encounterUI.Show(testNoonsong, () => Debug.Log("테스트 캐릭터 수집 완료!"));
        }
        else
        {
            Debug.LogWarning("테스트용 NoonsongEntry가 설정되지 않았습니다!");
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
            Debug.Log($"SpawnedObjects Count: {playerObjectSpawn.SpawnedObjects.Count}");

            GameObject target = playerObjectSpawn.SpawnedObjects[0].GameObject;
            Vector3 screenPoint = Camera.main.WorldToViewportPoint(target.transform.position);
            bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

            if (onScreen)
            {
                currentTarget = target;

                target.transform.LookAt(Camera.main.transform);
            }
            else
            {
                currentTarget = null;
            }
        }
        else
        {
            Debug.Log("SpawnedObjects is null or empty.");
            currentTarget = null;
        }
    }

    void OnCatchButtonClicked()
    {
        if (currentTarget != null)
        {
            var spawnedObject = playerObjectSpawn.SpawnedObjects.Find(obj => obj.GameObject == currentTarget);
            if (spawnedObject != null)
            {
                NoonsongEntry entry = spawnedObject.NoonsongEntry;
                if (entry != null)
                {
                    encounterUI.Show(entry, CollectCharacter); // UI 닫힐 때 CollectCharacter 실행
                }
            }
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
            }
        }
    }
}