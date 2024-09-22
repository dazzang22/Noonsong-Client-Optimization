using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotHandler : MonoBehaviour
{
    // 캔버스를 참조로 받습니다.
    public Canvas canvasToHide;
    public AudioSource audioSource;  
    public AudioClip shutterSound;  

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

    private void CaptureScreenForPC(string fileName)
    {
        string filePath = $"{fileName}.png";
        ScreenCapture.CaptureScreenshot(filePath);
        Debug.Log($"스크린샷 저장됨: {filePath}");
    }

    private void CaptureScreenForMobile(string fileName)
    {
        Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();

        // do something with texture
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
            Debug.Log(success);
            Debug.Log(path);
        });

        // cleanup
        Object.Destroy(texture);
    }
}
