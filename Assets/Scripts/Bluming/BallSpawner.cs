using Mapbox.Directions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject[] ballPrefabs;
    public Transform spawnPoint;
    public float spawnInterval = 2f;

    void Start()
    {
        StartCoroutine(SpawnBalls());
    }

    private IEnumerator SpawnBalls()
    {
        while (true)
        {
            SpawnBall();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnBall()
    {
        int randomIndex = Random.Range(0, ballPrefabs.Length);
        Instantiate(ballPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);
    }
}
