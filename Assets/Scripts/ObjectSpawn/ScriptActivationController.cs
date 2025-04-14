using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using System;
using System.Collections;

public class ScriptActivationController : MonoBehaviour
{
    [SerializeField] Vector2d[] rectangleVertices; // 사각형의 꼭짓점

    [SerializeField] AbstractMap map; // Mapbox 맵

    [SerializeField]  Transform xrOrigin; // XR Origin

    [SerializeField] Canvas cameraCanvas; // 카메라 캔버스
    private bool isInsideArea = false;

    void OnEnable()
    {
        LocationManager.OnLocationUpdated += OnLocationUpdated;
    }

    void OnDisable()
    {
        LocationManager.OnLocationUpdated -= OnLocationUpdated;
    }

    void Start()
    {
        if (rectangleVertices.Length != 4)
        {
            Debug.LogError("You must specify exactly 4 vertices for the rectangle.");
            return;
        }
    }
    public bool IsActive()
    {
        return isInsideArea;
    }

    void OnLocationUpdated(Vector2d userLocation)
    {
        CheckAndSetupUserLocation(userLocation);
    }

    void CheckAndSetupUserLocation(Vector2d userLocation)
    {
        // Debug.Log($"User Location: Latitude = {userLocation.x:F6}, Longitude = {userLocation.y:F6}");

        Vector3 worldPosition = map.GeoToWorldPosition(userLocation, true);
        worldPosition.y = 0;
        xrOrigin.position = worldPosition;

        if (IsLocationInsideRectangle(userLocation, rectangleVertices))
        {
            Debug.Log($"(Object: {gameObject.name}), User is inside the designated area. ");
            isInsideArea = true;

            // Debug.Log("inside : Player position at " + xrOrigin.position);

        }
        else
        {
            isInsideArea = false;
            // Debug.Log($"(Object: {gameObject.name}), outside : Player position at " + xrOrigin.position);
        }

        if (cameraCanvas != null && cameraCanvas.gameObject.activeSelf)
        {
            xrOrigin.position = Vector3.zero;
            // Debug.Log("CameraCanvas is active. XR Origin set to (0,0,0).");
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
        double signValue = (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    
        // 작은 값에 대해 오차 범위를 둔 비교
        if (Math.Abs(signValue) < 1e-8) // epsilon 값 (작은 값)
        {
            return 0;
        }
        return signValue;
    }
}
