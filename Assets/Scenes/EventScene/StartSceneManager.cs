using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneManager : MonoBehaviour
{
    // public Image fadeImage; // 페이드 효과에 사용할 이미지 (검은 배경 이미지)
    // public float fadeDuration = 1.0f; // 페이드 효과의 지속 시간

    // // 씬을 변경하면서 페이드 아웃 -> 씬 변경 -> 페이드 인
    // public void SceneChanger()
    // {
    //     StartCoroutine(FadeAndLoadScene());
    // }

    // // 페이드 아웃 -> 씬 변경 -> 페이드 인을 처리하는 코루틴
    // IEnumerator FadeAndLoadScene()
    // {
    //     // 페이드 아웃
    //     yield return StartCoroutine(Fade(1));

    //     // 씬 로드
    //     SceneManager.LoadScene("EvenetScene");

    //     // 씬이 로드된 후 페이드 인
    //     yield return StartCoroutine(Fade(0));
    // }

    // // 페이드 효과를 처리하는 코루틴
    // IEnumerator Fade(float targetAlpha)
    // {
    //     // 현재 색상
    //     Color currentColor = fadeImage.color;
    //     float startAlpha = currentColor.a;

    //     // 시간에 따른 페이드 효과 처리
    //     for (float t = 0; t < fadeDuration; t += Time.deltaTime)
    //     {
    //         float normalizedTime = t / fadeDuration;
    //         float alpha = Mathf.Lerp(startAlpha, targetAlpha, normalizedTime);
    //         fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
    //         yield return null;
    //     }

    //     // 최종 알파 값 설정
    //     fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);
    // }

    public void SceneChanger()
    {
        SceneManager.LoadScene("EventScene");
    }
}
