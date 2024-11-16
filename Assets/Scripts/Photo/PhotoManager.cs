using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoManager : MonoBehaviour
{
    private Vector3 offset; // 드래그 시 오브젝트와 터치 사이의 거리
    private float initialDistance; // 확대/축소를 위한 초기 거리
    private Vector3 initialScale;   // 초기 스케일
    private float initialRotation;  // 회전을 위한 초기 각도
    private Vector2 lastTouchPosition; // 이전 프레임의 터치 위치

    void Start()
    {
        initialScale = transform.localScale; // 초기 스케일 설정
    }

    void Update()
    {
        if (Input.touchCount == 1)
        {
            // 한 손가락 드래그로 이동 및 회전 처리
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Plane objPlane = new Plane(Camera.main.transform.forward * -1, transform.position);
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                float distance;

                if (objPlane.Raycast(ray, out distance))
                {
                    offset = transform.position - ray.GetPoint(distance);
                }
                
                // 터치 시작 위치 저장
                lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 deltaPosition = touch.position - lastTouchPosition;

                // 수평 드래그일 때 회전
                if (Mathf.Abs(deltaPosition.x) > Mathf.Abs(deltaPosition.y))
                {
                    float angle = deltaPosition.x * 0.1f; // 회전 속도 조정
                    transform.Rotate(Vector3.up, angle, Space.World);
                }
                // 수직 드래그일 때 이동
                else
                {
                    Plane objPlane = new Plane(Camera.main.transform.forward * -1, transform.position);
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    float distance;

                    if (objPlane.Raycast(ray, out distance))
                    {
                        transform.position = ray.GetPoint(distance) + offset;
                    }
                }
                
                // 현재 터치 위치 저장
                lastTouchPosition = touch.position;
            }
        }
        else if (Input.touchCount == 2)
        {
            // 두 손가락으로 확대/축소 처리
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            if (touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began)
            {
                initialDistance = Vector2.Distance(touchZero.position, touchOne.position);
                initialScale = transform.localScale;
            }
            else if (touchZero.phase == TouchPhase.Moved || touchOne.phase == TouchPhase.Moved)
            {
                float currentDistance = Vector2.Distance(touchZero.position, touchOne.position);

                if (Mathf.Approximately(initialDistance, 0))
                    return;

                // 확대/축소 적용
                float scaleMultiplier = currentDistance / initialDistance;
                transform.localScale = initialScale * scaleMultiplier;
            }
        }
    }
}
