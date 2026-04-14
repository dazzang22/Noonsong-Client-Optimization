using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UI;

public class ARObjectCatch : MonoBehaviour
{
    private PlayerObjectSpawnManager playerObjectSpawnManager;
    // Camera Caching
    private Camera mainCamera;

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

    // GC Alloc 최적화를 위해 체크 포인트 배열을 미리 할당
    private readonly Vector3[] checkPoints = new Vector3[5];
    private WaitForSeconds detectWait;
    private Coroutine detectionCoroutine;

    // detection
    [Header("Detection")]
    [SerializeField] private float detectInterval = 0.1f;
    [SerializeField] private float screenPadding = 50f;
    [SerializeField] private float defaultBoundingRadius = 0.5f;

    // test tool
    private static readonly ProfilerMarker CheckForObjectMarker = new ProfilerMarker("ARObjectCatch.CheckForObjectInView");
    private Stopwatch stopwatch = new Stopwatch();
    private long totalTicks = 0;
    private int checkCount = 0;
    private float reportTimer = 0f;


    void Start()
    {
        playerObjectSpawnManager = PlayerObjectSpawnManager.Instance;
        // Camera.main 캐싱
        mainCamera = Camera.main;
        detectWait = new WaitForSeconds(detectInterval);
        catchButton.onClick.AddListener(OnCatchButtonClicked);
        detectionCoroutine = StartCoroutine(CheckForObjectInViewCoroutine());
    }

    void Update()
    {
        reportTimer += Time.deltaTime;
        if (reportTimer >= 1f)
        {
            double totalMs = totalTicks * 1000.0 / Stopwatch.Frequency;
            double avgMs = totalMs / checkCount;
            double avgUs = totalTicks * 1000000.0 / Stopwatch.Frequency / checkCount;

            UnityEngine.Debug.Log(
                $"CheckForObjectInView - Total Checks: {checkCount}, Total Time: {totalMs:F2} ms, Average Time: {avgMs:F4} ms ({avgUs:F2} µs)"
            );

            totalTicks = 0;
            checkCount = 0;
            reportTimer = 0f;
        }
    }

    private IEnumerator CheckForObjectInViewCoroutine()
    {
        while (true)
        {
            stopwatch.Restart();
            CheckForObjectInView();
            stopwatch.Stop();

            totalTicks += stopwatch.ElapsedTicks;
            checkCount++;
            yield return detectWait; // detectInterval마다 체크
        }
    }

    void CheckForObjectInView()
    {

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
                    float boundingRadius = 0.5f;

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

                        // Vector3 lookPosition = Camera.main.transform.position - target.transform.position;
                        // lookPosition.y = 0;
                        // Quaternion targetRotation = Quaternion.LookRotation(lookPosition);
                        // target.transform.rotation = Quaternion.Slerp(target.transform.rotation, targetRotation, Time.deltaTime * 5);

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
    }

    void OnCatchButtonClicked()
    {

        if (encounterUI.gameObject.activeSelf)
        {
            encounterUI.ShowExitConfirmation();
            return;
        }

        if (currentTarget != null && currentTarget.name == "noonsong remake 0202(Clone)")
        {
            encounterUI.ShowDefaultDialogue(noonsongPrefeb, () => {
                UnityEngine.Debug.Log("기본 대화 종료 후 로직 실행");
            });
            return;
        }

        if (currentTarget != null)
        {
            UnityEngine.Debug.Log($"Attempting to catch object: {currentTarget.name}");
            var spawnedObject = playerObjectSpawnManager.SpawnedObjects.Find(obj => obj.GameObject == currentTarget);
            if (spawnedObject != null)
            {
                UnityEngine.Debug.Log($"Caught object: {currentTarget.name}, checking for NoonsongEntry...");
                NoonsongEntry entry = spawnedObject.NoonsongEntry;
                if (entry != null)
                {
                    //UnityEngine.Debug.Log($"Found NoonsongEntry for {currentTarget.name}: {entry.itemName}, requiredNoonsongs: {entry.requiredNoonsongs}");
                    encounterUI.Show(entry, () =>
                    {
                        UnityEngine.Debug.Log("대화 종료 후 캐릭터 수집 실행");
                    });

                }
                else
                {
                    UnityEngine.Debug.Log($"No NoonsongEntry found for {currentTarget.name}");
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

    public void CollectCharacter()
    {
        if (currentTarget != null)
        {
            var spawnedObject = playerObjectSpawnManager.SpawnedObjects.Find(obj => obj.GameObject == currentTarget);
            if (spawnedObject != null)
            {
                NoonsongEntry entry = spawnedObject.NoonsongEntry;

                if (entry != null)
                {
                    int requiredCurrency = entry.requiredNoonsongs;

                    if (!entry.isDiscovered)
                    {

                        noonsongManager.DiscoverItem(entry);
                        entry.isDiscovered = true;
                        //currencyManager.UseCurrency(requiredCurrency);
                    }
                }

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
        exitPopup.SetActive(false);
    }
}