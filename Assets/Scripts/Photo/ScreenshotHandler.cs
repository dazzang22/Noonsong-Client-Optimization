using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ScreenshotHandler : MonoBehaviour
{
    // 캔버스를 참조로 받습니다.
    public Canvas canvasToHide;
    public AudioSource audioSource;  
    public AudioClip shutterSound;  
    public ARCameraManager arCameraManager; // AR 카메라 매니저 참조

    private bool isUsingFrontCamera = false; // 현재 전면 카메라를 사용 중인지 여부

    // 전환 버튼에서 호출될 함수
    public void SwitchCamera()
    {
        isUsingFrontCamera = !isUsingFrontCamera;
        SetCameraFacingDirection(isUsingFrontCamera);
    }

    // 후면 카메라로 전환하는 메서드
    public void SwitchToBackCamera()
    {
        isUsingFrontCamera = false;
        SetCameraFacingDirection(isUsingFrontCamera);
    }

    // 카메라의 FacingDirection 설정
    private void SetCameraFacingDirection(bool useFrontCamera)
    {
        if (arCameraManager != null)
        {
            // 전면 카메라 사용 시
            if (useFrontCamera)
            {
                arCameraManager.requestedFacingDirection = CameraFacingDirection.User;
                Debug.Log("Switched to front camera.");
            }
            // 후면 카메라 사용 시
            else
            {
                arCameraManager.requestedFacingDirection = CameraFacingDirection.World;
                Debug.Log("Switched to back camera.");
            }
        }
        else
        {
            Debug.LogError("AR Camera Manager is not assigned.");
        }
    }

    void Start()
    {
        // 처음에는 후면 카메라로 시작
        SetCameraFacingDirection(isUsingFrontCamera);
    }

    // 스크린샷 촬영 함수
    public void CaptureScreen()
    {
        StartCoroutine(CaptureScreenCoroutine());
    }

    private IEnumerator CaptureScreenCoroutine()
    {
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        string fileName = "Screenshot-" + timestamp;

        // 캔버스 숨기기
        canvasToHide.enabled = false;

        // 셔터 사운드 재생
        if (audioSource != null && shutterSound != null)
        {
            audioSource.PlayOneShot(shutterSound);
        }

        // 한 프레임 대기 (캔버스 숨김 적용)
        yield return new WaitForEndOfFrame();

        #if UNITY_IOS || UNITY_ANDROID
        CaptureScreenForMobile(fileName);
        #else
        CaptureScreenForPC(fileName);
        #endif

        // 캔버스 다시 나타내기
        canvasToHide.enabled = true;
    }

    // PC에서 스크린샷 저장
    private void CaptureScreenForPC(string fileName)
    {
        string filePath = $"{fileName}.png";
        ScreenCapture.CaptureScreenshot(filePath);
        Debug.Log($"스크린샷 저장됨: {filePath}");
    }

    // 모바일에서 스크린샷 저장
    private void CaptureScreenForMobile(string fileName)
    {
        Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();

        // 스크린샷 저장
        NativeGallery.Permission permission = NativeGallery.CheckPermission(NativeGallery.PermissionType.Write, NativeGallery.MediaType.Image);
        if (permission == NativeGallery.Permission.Denied)
        {
            if (NativeGallery.CanOpenSettings())
            {
                NativeGallery.OpenSettings();
            }
        }

        string albumName = "Screenshot";
        NativeGallery.SaveImageToGallery(texture, albumName, fileName, (success, path) =>
        {
            Debug.Log(success ? $"스크린샷 저장됨: {path}" : "스크린샷 저장 실패");
        });

        // cleanup
        Object.Destroy(texture);
    }
}
