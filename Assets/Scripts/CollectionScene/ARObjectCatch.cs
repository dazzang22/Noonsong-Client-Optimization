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

    //�׽�Ʈ�� �ڵ�
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

        //�׽�Ʈ�� �ڵ�
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TestEncounter();
        }
    }

    //�׽�Ʈ�� �ڵ�
    void TestEncounter()
    {
        if (testNoonsong != null)
        {
            Debug.Log("�׽�Ʈ ĳ���� ���� UI ����");
            encounterUI.Show(testNoonsong, () => Debug.Log("�׽�Ʈ ĳ���� ���� �Ϸ�!"));
        }
        else
        {
            Debug.LogWarning("�׽�Ʈ�� NoonsongEntry�� �������� �ʾҽ��ϴ�!");
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
                    Debug.Log("Encounter UI showing...");
                    encounterUI.Show(entry, CollectCharacter); // UI ���� �� CollectCharacter ����
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
    public GameObject GetCurrentTarget()
    {
        return currentTarget;
    }
}