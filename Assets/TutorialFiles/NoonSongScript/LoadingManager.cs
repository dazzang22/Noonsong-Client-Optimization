using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    public GameObject loadingPanel; // 로딩 화면 패널
    public float loadingDuration = 3f; // 로딩 화면 표시 시간
    public GameObject studentCard; // 미묘한 학생증 오브젝트

    void Start()
    {
        StartCoroutine(ShowLoadingScreen());
    }

    IEnumerator ShowLoadingScreen()
    {
        // 로딩 화면 표시
        loadingPanel.SetActive(true);
        studentCard.SetActive(false);

        // 지정된 시간 동안 로딩 화면 유지
        yield return new WaitForSeconds(loadingDuration);

        // 로딩 화면 숨기고 학생증 등장
        loadingPanel.SetActive(false);
        studentCard.SetActive(true);

        // 학생증 등장 애니메이션 시작
        StartCoroutine(ShowStudentCard());
    }

    IEnumerator ShowStudentCard()
    {
        // 학생증 등장 애니메이션 로직 (예: 위치 이동)
        Vector3 startPosition = new Vector3(0, -5, 0);
        Vector3 endPosition = Vector3.zero;
        float animationDuration = 2f;
        float elapsedTime = 0f;

        studentCard.transform.position = startPosition;

        while (elapsedTime < animationDuration)
        {
            studentCard.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / animationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        studentCard.transform.position = endPosition;
    }
}