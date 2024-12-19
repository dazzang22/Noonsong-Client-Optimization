using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject[] ballPrefabs;
    public Transform spawnPoint;
    public Transform[] waypoints;

    private float spawnInterval = 2f;

    void Start()
    {
        StartCoroutine(SpawnBallRoutine());
    }

    IEnumerator SpawnBallRoutine()
    {
        while (true)
        {
            SpawnBall();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnBall()
    {
        int randomIndex = Random.Range(0, ballPrefabs.Length);
        GameObject ball = Instantiate(ballPrefabs[randomIndex], spawnPoint.position, Quaternion.identity, spawnPoint);
        BallMovement ballMovement = ball.GetComponent<BallMovement>();
        ballMovement.SetWaypoints(waypoints);
    }
}