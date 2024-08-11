using UnityEngine;
using UnityEngine.UI;

public class ARObjectCatch : MonoBehaviour
{
    [SerializeField]
    private ARObjectSpawn arObjectSpawn;

    [SerializeField]
    private NoonsongManager noonsongManager;

    [SerializeField]
    private CurrencyManager currencyManager; // CurrencyManager 추가

    [SerializeField]
    private Button catchButton;

    private GameObject currentTarget;

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

                if (entry != null)
                {
                    int requiredCurrency = spawnedObject.NoonsongEntry.requiredNoonsongs;

                    if (!entry.isDiscovered)
                    {
                        if (currencyManager.HasEnoughCurrency(requiredCurrency))
                        {
                            noonsongManager.DiscoverItem(entry);
                            entry.isDiscovered = true;
                            currencyManager.UseCurrency(requiredCurrency); // 통화 차감
                        }
                        else
                        {
                            Debug.Log("Not enough currency to discover this item.");
                        }
                    }

                    Destroy(currentTarget);
                    arObjectSpawn.SpawnedObjects.Remove(spawnedObject);
                }
            }
        }
    }
}


