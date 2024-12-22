using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARManager : MonoBehaviour
{
    public Transform arCamera; // AR 카메라 (휴대폰의 카메라)
    public Transform mapCamera; // 맵 내부의 카메라

    private Vector3 initialPositionOffset; // AR 카메라와 맵 카메라 간 초기 위치 오프셋
    private Quaternion initialRotationOffset; // 초기 회전 오프셋

    void Start()
    {
        // 초기 오프셋 저장 (필요에 따라 맵 스케일링 추가)
        initialPositionOffset = mapCamera.position - arCamera.position;
        initialRotationOffset = Quaternion.Inverse(arCamera.rotation) * mapCamera.rotation;
    }

    void Update()
    {
        // AR 카메라의 위치와 회전을 맵 카메라에 적용
        SyncCameraWithAR();
    }

    private void SyncCameraWithAR()
    {
        float mapScale = 0.1f;

        // 맵 카메라 위치 동기화
        mapCamera.position = arCamera.position + initialPositionOffset;

        // 맵 카메라 회전 동기화
        mapCamera.rotation = arCamera.rotation * initialRotationOffset;
    }
}