using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlaneTrigger : MonoBehaviour
{
    
    public Camera camera2; // 두 번째 카메라
    public GameObject targetObject;
    public GameObject targetObject2; // 활성화/비활성화할 오브젝트

    void Start()
    {
        targetObject.SetActive(true);
        targetObject2.SetActive(true); 
    }

    void Update()
    {
        StartCoroutine(OnMapCameraOn());
    }

    private IEnumerator OnMapCameraOn()
    {
        yield return new WaitForSeconds(3.0f); // 5초 대기

        // 첫 번째 카메라가 활성화된 경우
        if (camera2.enabled)
        {
            targetObject.SetActive(true);
            targetObject2.SetActive(true); // 오브젝트 비활성화
        }
        // 두 번째 카메라가 활성화된 경우
        else
        {
            targetObject.SetActive(false);
            targetObject2.SetActive(false); // 오브젝트 활성화
        }
    }
}
