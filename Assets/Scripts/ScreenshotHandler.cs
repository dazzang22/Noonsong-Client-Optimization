using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotHandler : MonoBehaviour
{

    public void CaptureScreen()
    {
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        string fileName = "Screenshot-" + timestamp;

        #if UNITY_IOS || UNITY_ANDROID
        CaptureScreenForMobile(fileName);
        #else
        CaptureScreenForPC(fileName);
        #endif
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
