using UnityEngine;
using UnityEngine.UI;

public class ARObjectCatch : MonoBehaviour
{
    [SerializeField]
    private ARObjectSpawn arObjectSpawn;

    [SerializeField]
    private NoonsongManager noonsongManager;

    [SerializeField]
    private CurrencyManager currencyManager;

    [SerializeField]
    private Button catchButton;

    private GameObject currentTarget;

    private const int generalNoonsongCost = 5; //임의 지정

    void Start()
    {
        catchButton.onClick.AddListener(OnCatchButtonClicked);
    }

    void Update()
    {
        CheckForObjectInView();
    }

    void CheckForObjectInView()
    {
        if (arObjectSpawn != null && arObjectSpawn.SpawnedObjects.Count > 0)
        {
            GameObject target = arObjectSpawn.SpawnedObjects[0].GameObject;
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
            currentTarget = null;
        }
    }

    void OnCatchButtonClicked()
    {
        if (currentTarget != null)
        {
            SpawnedObject spawnedObject = arObjectSpawn.SpawnedObjects.Find(obj => obj.GameObject == currentTarget);

            if (spawnedObject != null)
            {
                NoonsongEntry entry = spawnedObject.NoonsongEntry;

                // MajorNoonsong 인 경우
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
                    Destroy(currentTarget);
                    arObjectSpawn.SpawnedObjects.Remove(spawnedObject);
                }
                else
                {
                    // generalNoonsong인 경우
                    if (currencyManager.HasEnoughCurrency(generalNoonsongCost))
                    {
                        currencyManager.UseCurrency(generalNoonsongCost);  
                       
                        Destroy(currentTarget);
                        arObjectSpawn.SpawnedObjects.Remove(spawnedObject);
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



