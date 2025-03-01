using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using System.Collections;

public class ScriptActivationController : MonoBehaviour
{
    [SerializeField]
    Vector2d[] rectangleVertices; // 사각형의 꼭짓점

    [SerializeField]
    MonoBehaviour scriptToActivate; // 활성화할 스크립트

    [SerializeField]
    AbstractMap map; // Mapbox 맵

    [SerializeField]
    Transform xrOrigin; // XR Origin

    [SerializeField]
    GameObject spawnObject; // 스폰할 오브젝트

    [SerializeField]
    Canvas cameraCanvas; // 카메라 캔버스

    private bool isXROriginPositionSet = false;
    private bool isObjectSpawned = false;
    private float checkInterval = 5f; // 위치 확인 간격 (초)

    void Start()
    {
        if (rectangleVertices.Length != 4)
        {
            Debug.LogError("You must specify exactly 4 vertices for the rectangle.");
            return;
        }

        if (scriptToActivate != null)
        {
            scriptToActivate.enabled = false;
        }

        StartCoroutine(CheckUserLocationPeriodically());
    }
    public bool IsActive()
    {
        return scriptToActivate != null && scriptToActivate.enabled;
    }

    IEnumerator CheckUserLocationPeriodically()
    {
        while (true)
        {
            // 위치 권한이 승인되었는지 확인
            if (LocationPermissionManager.Instance.IsLocationServiceInitialized)
            {
                Debug.Log("위치 권한이 활성화되었습니다");
                CheckAndSetupUserLocation();
            }
            else
            {
                Debug.Log("Waiting for location permission...");
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }

    void CheckAndSetupUserLocation()
    {
        Vector2d userLocation = new Vector2d(Input.location.lastData.latitude, Input.location.lastData.longitude);
        Debug.Log($"User Location: Latitude = {userLocation.x:F6}, Longitude = {userLocation.y:F6}");

        if (IsLocationInsideRectangle(userLocation, rectangleVertices))
        {
            Debug.Log($"User is inside the designated area. (Object: {gameObject.name})");

            if (scriptToActivate != null && !scriptToActivate.enabled)
            {
                scriptToActivate.enabled = true;
                Debug.Log("Script activated.");
            }

            Vector3 worldPosition = map.GeoToWorldPosition(userLocation, true);
            if (!isXROriginPositionSet || Vector3.Distance(xrOrigin.position, worldPosition) > 1f)
            {
                worldPosition.y = 0;
                xrOrigin.position = worldPosition;
                isXROriginPositionSet = true;
                Debug.Log(xrOrigin.position);

                if (!isObjectSpawned)
                {
                    SpawnObject(worldPosition);
                    isObjectSpawned = true;
                }
            }
        }
        else
        {
            Debug.Log("User is outside the designated area.");
            if (scriptToActivate != null && scriptToActivate.enabled)
            {
                scriptToActivate.enabled = false;
                Debug.Log("Script deactivated.");
            }
        }

        if (cameraCanvas != null && cameraCanvas.gameObject.activeSelf)
        {
            xrOrigin.position = Vector3.zero;
            Debug.Log("CameraCanvas is active. XR Origin set to (0,0,0).");
        }
    }

    bool IsLocationInsideRectangle(Vector2d point, Vector2d[] vertices)
    {
        if (vertices.Length != 4)
        {
            Debug.LogError("The rectangle must have exactly 4 vertices.");
            return false;
        }

        bool inside = IsPointInTriangle(point, vertices[0], vertices[1], vertices[2]) ||
                      IsPointInTriangle(point, vertices[0], vertices[2], vertices[3]);

        return inside;
    }

    bool IsPointInTriangle(Vector2d pt, Vector2d v1, Vector2d v2, Vector2d v3)
    {
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
}
