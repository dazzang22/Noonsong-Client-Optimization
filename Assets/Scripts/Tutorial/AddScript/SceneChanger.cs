using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  // SceneManager를 사용하기 위해 필요한 네임스페이스

public class SceneChanger : MonoBehaviour
{
    // 전환될 씬의 이름
    public string nextSceneName;

    // 현재 씬에서 기다릴 시간 (초 단위)
    public float delay = 5f;

    // Start 메서드에서 코루틴 시작
    void Start()
    {
        StartCoroutine(ChangeSceneAfterDelay());
    }

    // 코루틴: 일정 시간 후 씬을 전환
    IEnumerator ChangeSceneAfterDelay()
    {
        // 지정된 시간만큼 대기
        yield return new WaitForSeconds(delay);

        // 다음 씬 로드
        SceneManager.LoadScene(nextSceneName);
    }
}