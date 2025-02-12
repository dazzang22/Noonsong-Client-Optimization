using System.Collections;
using UnityEngine;

public class NPCPatrol : MonoBehaviour
{
    public Transform npcCharacter; // NPC 캐릭터 (애니메이션 적용된 오브젝트)
    public float moveDistance = 5f; // 삼각형 한 변의 길이
    public float moveSpeed = 2f; // 이동 속도
    public float idleTime = 5f; // 정지 시간
    public float rotationSpeed = 90f; // 회전 속도 (1초에 90도)

    private Animator animator;
    private Vector3[] waypoints; // 정삼각형의 꼭짓점 좌표
    private int currentWaypoint = 0; // 현재 목표 지점
    private bool isFirstMove = true; // ✅ 처음 이동 여부 체크

    private void Start()
    {
        animator = npcCharacter.GetComponent<Animator>();

        // ✅ 정삼각형 꼭짓점 계산 (처음에는 회전 없이 직진)
        waypoints = new Vector3[3];
        waypoints[0] = transform.position; // 시작점
        waypoints[1] = waypoints[0] + transform.forward * moveDistance; // 직진
        waypoints[2] = waypoints[1] + (Quaternion.Euler(0, 120, 0) * transform.forward * moveDistance); // 120도 회전 후 전진

        StartCoroutine(PatrolRoutine());
    }

    private IEnumerator PatrolRoutine()
    {
        while (true)
        {
            // ✅ 처음 이동할 때는 회전 없이 바로 이동
            if (isFirstMove)
            {
                yield return StartCoroutine(MoveToNextWaypoint());
                isFirstMove = false; // 이후부터는 정상 루틴 실행
            }
            else
            {
                // 1. 이동
                yield return StartCoroutine(MoveToNextWaypoint());

                // 2. 5초 정지
                yield return StartCoroutine(Idle());

                // 3. 제자리 회전
                yield return StartCoroutine(RotateTowardsNextWaypoint());
            }

            // 4. 다음 이동을 위해 목표 지점 변경
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
    }

    private IEnumerator MoveToNextWaypoint()
    {
        animator.SetBool("isWalking", true);
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = waypoints[currentWaypoint];
        float startTime = Time.time;
        float journeyLength = Vector3.Distance(startPosition, targetPosition);
        float duration = journeyLength / moveSpeed;

        while (Time.time - startTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, (Time.time - startTime) / duration);
            yield return null;
        }

        transform.position = targetPosition;
        animator.SetBool("isWalking", false);
    }

    private IEnumerator Idle()
    {
        animator.SetBool("isWalking", false);
        yield return new WaitForSeconds(idleTime);
    }

    private IEnumerator RotateTowardsNextWaypoint()
    {
        animator.SetBool("isWalking", true); // ✅ 회전 중에도 걷기 애니메이션 유지

        Vector3 nextWaypoint = waypoints[(currentWaypoint + 1) % waypoints.Length];
        Vector3 direction = (nextWaypoint - transform.position).normalized;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float startTime = Time.time;
        float rotationDuration = 120f / rotationSpeed; // 120도 회전하는 데 걸리는 시간

        while (Time.time - startTime < rotationDuration)
        {
            float progress = (Time.time - startTime) / rotationDuration;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, progress);
            yield return null;
        }

        transform.rotation = targetRotation;
    }

    private void Update()
    {
        // ✅ NPC 캐릭터가 항상 정면을 유지하도록 로컬 회전 고정
        npcCharacter.localRotation = Quaternion.identity;
    }
}
