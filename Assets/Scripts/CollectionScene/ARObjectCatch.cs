using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARObjectCatch : MonoBehaviour
{
    //private ARObjectSpawn arObjectSpawn;
    private PlayerObjectSpawn playerObjectSpawn;

    [SerializeField]
    private NoonsongManager noonsongManager;

    [SerializeField]
    private CurrencyManager currencyManager;

    [SerializeField]
    private Button catchButton;

    private GameObject currentTarget;

    private const int generalNoonsongCost = 5; //���� ����


    void Start()
    {
        //행사장 : 밑 3줄 추가 
        playerObjectSpawn = FindObjectOfType<PlayerObjectSpawn>();
        if (playerObjectSpawn == null)
        {
            Debug.LogError("PlayerObjectSpawn not found in the scene.");
        }


        catchButton.onClick.AddListener(OnCatchButtonClicked);
    }

    void Update()
    {
        if (playerObjectSpawn != null)
        {
            CheckForObjectInView();
        }

    }
    void UpdateARObjectSpawnReference()
    {
        string activeScriptName = ScriptActivationController.activatedScriptName;
        GameObject activeObject = GameObject.Find(activeScriptName); 

        // if (activeObject != null)
        // {
        //     arObjectSpawn = activeObject.GetComponent<ARObjectSpawn>(); 
        // }
        // else
        // {
        //     arObjectSpawn = null; // Ȱ��ȭ�� ��ũ��Ʈ�� ���� ��� null�� ����
        // }
        if (playerObjectSpawn != null)
        {
            playerObjectSpawn = activeObject.GetComponent<PlayerObjectSpawn>(); 
        }
        else
        {
            playerObjectSpawn = null; // Ȱ��ȭ�� ��ũ��Ʈ�� ���� ��� null�� ����
        }
    }

    void CheckForObjectInView()
    {
        //if (arObjectSpawn != null && arObjectSpawn.SpawnedObjects.Count > 0)
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
            // SpawnedObject spawnedObject = arObjectSpawn.SpawnedObjects.Find(obj => obj.GameObject == currentTarget);
            SpawnedObject spawnedObject = playerObjectSpawn.SpawnedObjects.Find(obj => obj.GameObject == currentTarget);
            if (spawnedObject != null)
            {
                NoonsongEntry entry = spawnedObject.NoonsongEntry;

                // MajorNoonsong �� ���
                if (entry != null && currencyManager.GetActiveCurrencyType() == entry.university)
                {
                    int requiredCurrency = entry.requiredNoonsongs;

                    string university = entry.university;

                    if (!entry.isDiscovered)
                    {
                        if (currencyManager.HasEnoughCurrency(university, requiredCurrency))
                        {
                            noonsongManager.DiscoverItem(entry);
                            entry.isDiscovered = true;
                            currencyManager.UseCurrency(university, requiredCurrency);
                        }
                        else
                        {
                            Debug.Log("Not enough currency to discover this item.");
                        }
                    }
                    Destroy(currentTarget);
                    //arObjectSpawn.SpawnedObjects.Remove(spawnedObject);
                    playerObjectSpawn.SpawnedObjects.Remove(spawnedObject);
                }
                else if (entry == null && currencyManager.GetActiveCurrencyType() == "Default")
                {
                    // generalNoonsong�� ���
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
                }
            }
        }
    }
}



