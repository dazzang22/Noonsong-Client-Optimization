using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARObjectCatch : MonoBehaviour
{
    private PlayerObjectSpawnManager playerObjectSpawnManager;
    // Camera Caching
    private Camera mainCamera;

    [SerializeField]
    private NoonsongManager noonsongManager;

    private GameObject currentTarget;

    public GameObject noonsongPrefab;

    [SerializeField] private Button catchButton;
    [SerializeField] private EncounterUI encounterUI;
    [SerializeField] private GameObject exitPopup;

    // GC Alloc 최적화를 위해 체크 포인트 배열을 미리 할당
    private readonly Vector3[] checkPoints = new Vector3[5];
    private WaitForSeconds detectWait;

    // detection
    [Header("Detection")]
    [SerializeField] private float detectInterval = 0.1f;
    [SerializeField] private float screenPadding = 50f;
    [SerializeField] private float defaultBoundingRadius = 0.5f;


    void Start()
    {
        playerObjectSpawnManager = PlayerObjectSpawnManager.Instance;
        mainCamera = Camera.main;
        detectWait = new WaitForSeconds(detectInterval);
        catchButton.onClick.AddListener(OnCatchButtonClicked);
        StartCoroutine(CheckForObjectInViewCoroutine());
    }

    private IEnumerator CheckForObjectInViewCoroutine()
    {
        while (true)
        {
            CheckForObjectInView();
            yield return detectWait; // detectInterval마다 체크
        }
    }

    void CheckForObjectInView()
    {
        var spawnedObjects = playerObjectSpawnManager.SpawnedObjects;
        if (spawnedObjects.Count > 0)
        {
            foreach (var obj in spawnedObjects)
            {
                GameObject target = obj.GameObject;
                if (target == null)
                    continue;

                Vector3 objectPosition = target.transform.position;

                // 기본 반지름 설정 (콜라이더 없을 때 대비)
                float boundingRadius = defaultBoundingRadius;

                // 콜라이더가 있으면 크기 기반으로 반지름 계산
                Collider col = target.GetComponent<Collider>();
                if (col != null)
                {
                    boundingRadius = col.bounds.extents.magnitude * 0.5f;
                }

                // 체크 포인트 계산 (중심 + 4방향)
                checkPoints[0] = objectPosition; // 중심
                checkPoints[1] = objectPosition + new Vector3(boundingRadius, 0, 0); // 오른쪽
                checkPoints[2] = objectPosition + new Vector3(-boundingRadius, 0, 0); // 왼쪽
                checkPoints[3] = objectPosition + new Vector3(0, boundingRadius, 0); // 위
                checkPoints[4] = objectPosition + new Vector3(0, -boundingRadius, 0); // 아래

                bool isVisible = false;
                foreach (Vector3 point in checkPoints)
                {
                    Vector3 screenPoint = mainCamera.WorldToScreenPoint(point);

                    if (screenPoint.z > 0 &&
                        screenPoint.x > -screenPadding && screenPoint.x < Screen.width + screenPadding &&
                        screenPoint.y > -screenPadding && screenPoint.y < Screen.height + screenPadding)
                    {
                        isVisible = true;
                        break;
                    }
                }

                if (isVisible)
                {
                    currentTarget = target;
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
        if (currentTarget == null) return;

        if (encounterUI.gameObject.activeSelf)
        {
            encounterUI.ShowExitConfirmation();
            return;
        }
        var spawnedObject = playerObjectSpawnManager.SpawnedObjects
        .Find(obj => obj.GameObject == currentTarget);

        if (spawnedObject == null) return;
        NoonsongEntry entry = spawnedObject.NoonsongEntry;

        // General Noonsong
        if (entry == null)
        {
            encounterUI.ShowDefaultDialogue(noonsongPrefab, () =>
            {
                StartCoroutine(CleanupDestroyedSpawnedObject(spawnedObject));
                UnityEngine.Debug.Log("Default dialogue closed");
            });
            return;
        }
        // Major Noonsong
        encounterUI.Show(entry, () =>
        {
            StartCoroutine(CleanupDestroyedSpawnedObject(spawnedObject));
            Debug.Log("Encounter dialogue closed");
        });
    }

    private IEnumerator CleanupDestroyedSpawnedObject(SpawnedObject spawnedObject)
    {
        GameObject destroyedTarget = spawnedObject?.GameObject;

        // Destroy는 프레임 종료 시 반영되므로 다음 프레임에 실제 제거 여부를 확인한다.
        yield return null;

        if (spawnedObject == null || destroyedTarget != null)
            yield break;

        playerObjectSpawnManager.RemoveSpawnedObject(spawnedObject);

        if (currentTarget == destroyedTarget)
            currentTarget = null;
    }

    public void CollectCharacter()
    {
        if (currentTarget == null) return;

        var spawnedObject = playerObjectSpawnManager.SpawnedObjects.Find(obj => obj.GameObject == currentTarget);
        if (spawnedObject == null) return;

        NoonsongEntry entry = spawnedObject.NoonsongEntry;

        if (entry == null || entry.isDiscovered) return;

        noonsongManager.DiscoverItem(entry);
        entry.isDiscovered = true;

        CheckForObjectInView();
    }

    public GameObject GetCurrentTarget()
    {
        return currentTarget;
    }

    private void CloseEncounterCallback()
    {
        exitPopup.SetActive(false);
    }
}
