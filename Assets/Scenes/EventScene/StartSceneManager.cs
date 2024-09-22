using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneManager : MonoBehaviour
{
    public Image fadeImage; 
    public float fadeSpeed = 1.0f;

    public Canvas loginCanvas;  
    public Canvas fadeCanvas;  

    
    public void SceneChanger()
    {
        StartCoroutine(FadeAndLoadScene());
    }

 
    IEnumerator FadeAndLoadScene()
    {
        loginCanvas.sortingOrder = 0;
        fadeCanvas.sortingOrder = 1;

        float alpha = 0.0f;
        while (alpha < 1) {
            alpha += Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        SceneManager.LoadScene("EventScene");
    }

   
    // IEnumerator Fade(float targetAlpha)
    // {
  
    //     Color currentColor = fadeImage.color;
    //     float startAlpha = currentColor.a;

     
    //     for (float t = 0; t < fadeDuration; t += Time.deltaTime)
    //     {
    //         float normalizedTime = t / fadeDuration;
    //         float alpha = Mathf.Lerp(startAlpha, targetAlpha, normalizedTime);
    //         fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
    //         yield return null;
    //     }


    //     fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);
    // }

    // public void SceneChanger()
    // {
    //     SceneManager.LoadScene("EventScene");
    // }
}
