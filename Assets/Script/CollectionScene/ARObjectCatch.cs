using UnityEngine;
using UnityEngine.UI;

public class ARObjectCatch : MonoBehaviour
{
    [SerializeField]
    private ARObjectSpawn arObjectSpawn;

    [SerializeField]
    private NoonsongManager noonsongManager;

    [SerializeField]
    private Button catchButton;

    private GameObject currentTarget;

    void Start()
    {
        catchButton.onClick.AddListener(OnCatchButtonClicked);
        catchButton.gameObject.SetActive(false);
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
                catchButton.gameObject.SetActive(true);
                currentTarget = target;
            }
            else
            {
                catchButton.gameObject.SetActive(false);
                currentTarget = null;
            }
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
                    if (!entry.isDiscovered)
                    {
                        noonsongManager.DiscoverItem(entry);
                        entry.isDiscovered = true; // Update the isDiscovered property
                        Debug.Log($"{entry.noonsongName} has been discovered.");
                    }
                    else
                    {
                        Debug.Log($"{entry.noonsongName} is already discovered.");
                    }

                    Destroy(currentTarget);
                    arObjectSpawn.SpawnedObjects.Remove(spawnedObject); // Remove the SpawnedObject instance
                    catchButton.gameObject.SetActive(false);
                }
                else
                {
                    Debug.Log("No NoonsongEntry associated with the target.");
                }
            }
            else
            {
                Debug.Log("Current target is not found in the spawned objects list.");
            }
        }
    }
}



