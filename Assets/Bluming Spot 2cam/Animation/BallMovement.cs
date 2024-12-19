using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    private Transform[] waypoints;
    private int currentWaypointIndex = 0;

    public float speed = 5f;

    public void SetWaypoints(Transform[] waypoints)
    {
        this.waypoints = waypoints;
        StartCoroutine(MoveToNextWaypoint());
    }

    IEnumerator MoveToNextWaypoint()
    {
        while (currentWaypointIndex < waypoints.Length)
        {
            Vector3 targetPosition = waypoints[currentWaypointIndex].position;

            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                yield return null;
            }

            currentWaypointIndex++;
        }

        Destroy(gameObject);
    }
}