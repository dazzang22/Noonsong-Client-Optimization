using UnityEngine;

public class NundeongAnimation : MonoBehaviour
{
    public Transform nundeong; // 눈덩이 Transform
    public Transform arCamera; // AR 카메라 Transform
    public float moveDuration = 2f; // 이동 애니메이션 지속 시간

    private Vector3 startPos;
    private Vector3 endPos;
    private float elapsedTime = 0f;
    private bool isMoving = true;
    private Vector3 floatStartPos;

    void Start()
    {
        // 시작 위치와 끝 위치 설정
        startPos = arCamera.TransformPoint(new Vector3(-1.5f, 1.5f, 5f)); // AR 카메라의 좌측 상단 (화면 좌표계)
        endPos = arCamera.TransformPoint(new Vector3(0f, 0f, 2f)); // AR 카메라의 정면 (화면 중앙)

        //위치를 더 조정하고 싶다면 x, y, z 값을 변경하여 원하는 위치를 설정
        //시작 위치를 더 왼쪽 위로 이동시키고 싶다면 x 값을 더 작게 하고, y 값을 더 크게 설정
        //도착 위치를 더 중앙에 맞추고 싶다면 x와 y 값을 0에 가깝게 설정하면 됨.

        // 눈덩이 초기 위치 설정
        nundeong.position = startPos;

        // 애니메이션 시작
        isMoving = true;
    }

    void Update()
    {
        if (isMoving)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / moveDuration);
            nundeong.position = Vector3.Lerp(startPos, endPos, t);

            if (t >= 1f)
            {
                isMoving = false;
                floatStartPos = nundeong.position;
                elapsedTime = 0f; // 떠있는 애니메이션을 위한 시간 초기화
            }
        }
    }
}