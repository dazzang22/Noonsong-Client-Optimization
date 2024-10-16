using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using System.Collections;

public class ScriptActivationController : MonoBehaviour
{
    [SerializeField]
    Vector2d[] rectangleVertices; // 사각형의 각 꼭짓점을 저장하는 배열

    [SerializeField]
    MonoBehaviour scriptToActivate; // 활성화할 스크립트

    [SerializeField]
    AbstractMap map; // Mapbox 맵을 참조

    [SerializeField]
    Transform xrOrigin; // XR Origin

    [SerializeField]
    GameObject spawnObject; // 스폰할 오브젝트

    public static string activatedScriptName;

    private bool isLocationServiceInitialized = false;
    private bool isXROriginPositionSet = false; // XR Origin 위치가 설정되었는지 여부
    private bool isObjectSpawned = false; // 오브젝트가 스폰되었는지 여부
    private float checkInterval = 5f; // 위치 확인 간격 (초)

    void Start()
    {
        if (rectangleVertices.Length != 4)
        {
            Debug.LogError("You must specify exactly 4 vertices for the rectangle.");
            return;
        }

        StartCoroutine(WaitForLocationService());
    }

    IEnumerator WaitForLocationService()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("Location services are not enabled by the user.");
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1)
        {
            Debug.LogError("Timed out while initializing location services.");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Unable to determine device location.");
            yield break;
        }

        isLocationServiceInitialized = true;
        StartCoroutine(CheckUserLocationPeriodically());
    }

    IEnumerator CheckUserLocationPeriodically()
    {
        while (true)
        {
            if (isLocationServiceInitialized)
            {
                CheckAndSetupUserLocation();
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }

    void CheckAndSetupUserLocation()
    {
        if (!isLocationServiceInitialized) return;

        Vector2d userLocation = new Vector2d(Input.location.lastData.latitude, Input.location.lastData.longitude); // Latitude, Longitude
        Debug.Log($"User Location: Latitude = {userLocation.x:F6}, Longitude = {userLocation.y:F6}");

        if (IsLocationInsideRectangle(userLocation, rectangleVertices))
        {
            Debug.Log("User is inside the designated area.");

            if (scriptToActivate != null && !scriptToActivate.enabled)
            {
                scriptToActivate.enabled = true; // 스크립트 활성화

                activatedScriptName = GetActivatedScriptName();
                Debug.Log("Script activated.");
            }

            // 유저 위치를 월드 좌표로 변환
            Vector3 worldPosition = map.GeoToWorldPosition(userLocation, true);

            if (!isXROriginPositionSet || Vector3.Distance(xrOrigin.position, worldPosition) > 1f) // 위치가 변경되었거나 초기 설정되지 않은 경우
            {
                worldPosition.y = 0;
                // XR Origin을 해당 위치로 이동
                xrOrigin.position = worldPosition;
                isXROriginPositionSet = true; // XR Origin 위치가 설정되었음을 기록
                Debug.Log(xrOrigin.position);
                
                // 오브젝트가 스폰되지 않은 경우에만 스폰
                if (!isObjectSpawned)
                {
                    SpawnObject(worldPosition);
                    isObjectSpawned = true; // 오브젝트가 스폰되었음을 기록
                }
            }
        }
        else
        {
            Debug.Log("User is outside the designated area.");

            if (scriptToActivate != null && scriptToActivate.enabled)
            {
                scriptToActivate.enabled = false; // 스크립트 비활성화
                Debug.Log("Script deactivated.");
            }
        }
    }

    bool IsLocationInsideRectangle(Vector2d point, Vector2d[] vertices)
    {
        if (vertices.Length != 4)
        {
            Debug.LogError("The rectangle must have exactly 4 vertices.");
            return false;
        }

        Debug.Log("Checking if the location is inside the rectangle.");

        // Print out the coordinates to verify
        Debug.Log($"Checking if point: Latitude = {point.x}, Longitude = {point.y} is inside rectangle with vertices:");
        for (int i = 0; i < vertices.Length; i++)
        {
            Debug.Log($"Vertex {i}: Latitude = {vertices[i].x}, Longitude = {vertices[i].y}");
        }

        // 사각형 내부에 사용자가 있는지 확인합니다.
        bool inside = IsPointInTriangle(point, vertices[0], vertices[1], vertices[2]) ||
                      IsPointInTriangle(point, vertices[0], vertices[2], vertices[3]);

        Debug.Log($"User is inside the rectangle: {inside}");

        return inside;
    }

    bool IsPointInTriangle(Vector2d pt, Vector2d v1, Vector2d v2, Vector2d v3)
    {
        Debug.Log("Checking if the point is inside a triangle.");
        double d1, d2, d3;
        bool has_neg, has_pos;

        d1 = Sign(pt, v1, v2);
        d2 = Sign(pt, v2, v3);
        d3 = Sign(pt, v3, v1);

        has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

        return !(has_neg && has_pos);
    }

    double Sign(Vector2d p1, Vector2d p2, Vector2d p3)
    {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }

    void SpawnObject(Vector3 worldPosition)
    {
        worldPosition.y = 0;
        GameObject instance = Instantiate(spawnObject, worldPosition, Quaternion.identity);
        instance.transform.localScale = new Vector3(1, 1, 1);
        Debug.Log("Object spawned at: " + worldPosition);
    }

    string GetActivatedScriptName()
    {
        return scriptToActivate.GetType().Name;
    }

}
