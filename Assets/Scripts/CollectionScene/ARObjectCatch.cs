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

    private GameObject encounterTarget;
    public GameObject EncounterTarget => encounterTarget;

    [SerializeField] private GameObject noonsongPrefab;

    [SerializeField] private Button catchButton;
    [SerializeField] private EncounterUI encounterUI;

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
        if (playerObjectSpawnManager != null)
        {
            playerObjectSpawnManager.RegisterObjectCatch(this);
            Debug.Log("[Encounter] ARObjectCatch registered with PlayerObjectSpawnManager");
        }
        mainCamera = Camera.main;
        detectWait = new WaitForSeconds(detectInterval);
        catchButton.onClick.AddListener(OnCatchButtonClicked);
        StartCoroutine(CheckForObjectInViewCoroutine());
    }

    private IEnumerator CheckForObjectInViewCoroutine()
    {
        while (true)
        {
            UpdateCurrentTarget();
            yield return detectWait; // detectInterval마다 체크
        }
    }

    void UpdateCurrentTarget()
    {
        var spawnedObjects = playerObjectSpawnManager.SpawnedObjects;
        foreach (var obj in spawnedObjects)
        {
            GameObject target = obj.GameObject;
            if (target == null || !target.activeInHierarchy) continue;

            if (IsVisibleInView(target))
            {
                currentTarget = target;
                return;
            }
        }

        currentTarget = null;
    }

    private bool IsVisibleInView(GameObject target)
    {
        Vector3 objectPosition = target.transform.position;
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

        foreach (Vector3 point in checkPoints)
        {
            Vector3 screenPoint = mainCamera.WorldToScreenPoint(point);

            if (screenPoint.z > 0 &&
                screenPoint.x > -screenPadding && screenPoint.x < Screen.width + screenPadding &&
                screenPoint.y > -screenPadding && screenPoint.y < Screen.height + screenPadding)
            {
                return true;
            }
        }
        return false;
    }

    void OnCatchButtonClicked()
    {
        if (encounterTarget != null)
        {
            encounterUI.ShowExitConfirmation();
            return;
        }
        if (currentTarget == null) return;

        if (encounterUI.gameObject.activeSelf)
        {
            encounterUI.ShowExitConfirmation();
            return;
        }
        var spawnedObject = FindCurrentSpawnedObject(currentTarget);

        if (spawnedObject == null) return;
        // 조우를 시작한 캐릭터를 따로 기억
        encounterTarget = spawnedObject.GameObject;
        Debug.Log($"[Encounter] 조우 대상 id={encounterTarget.GetInstanceID()}");
        NoonsongEntry entry = spawnedObject.NoonsongEntry;

        // General Noonsong
        if (entry == null)
        {
            encounterUI.ShowDefaultDialogue(noonsongPrefab, () =>
            {
                encounterTarget = null;
                StartCoroutine(CleanupDestroyedSpawnedObject(spawnedObject));
                UnityEngine.Debug.Log("Default dialogue closed");
            });
            return;
        }
        // Major Noonsong
        encounterUI.Show(entry, () =>
        {
            encounterTarget = null;
            StartCoroutine(CleanupDestroyedSpawnedObject(spawnedObject));
            Debug.Log("Encounter dialogue closed");
        });
    }

    private IEnumerator CleanupDestroyedSpawnedObject(SpawnedObject spawnedObject)
    {
        GameObject destroyedTarget = spawnedObject?.GameObject;

        yield return null;

        if (spawnedObject == null || destroyedTarget != null)
            yield break;

        playerObjectSpawnManager.RemoveSpawnedObject(spawnedObject);

        if (currentTarget == destroyedTarget)
            currentTarget = null;
    }

    public void CollectCharacter()
    {
        if (encounterTarget == null || encounterUI == null)
            return;

        // 현재 조우 중인 A의 캐릭터 데이터
        NoonsongEntry entry = encounterUI.GetCurrentNoonsongEntry();

        if (entry == null || entry.isDiscovered)
            return;

        Debug.Log($"[Collect] 조우 대상 발견: {entry.noonsongName}");

        noonsongManager.DiscoverItem(entry);
        UpdateCurrentTarget();
    }


    private SpawnedObject FindCurrentSpawnedObject(GameObject target)
    {
        return playerObjectSpawnManager.SpawnedObjects.Find(obj => obj.GameObject == target);
    }

    public GameObject GetCurrentTarget()
    {
        // 조우 중에는 처음 조우한 A를 반환한다.
        if (encounterTarget != null)
        {
            return encounterTarget;
        }

        return currentTarget;
    }
}
