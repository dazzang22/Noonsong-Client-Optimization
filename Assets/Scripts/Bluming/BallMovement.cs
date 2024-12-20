using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    public Transform[] points;
    public GameObject[] snowflakePrefabs; 
    public float moveSpeed = 2f;

    private int currentPointIndex = 0;

    void Start()
    {
        points = new Transform[6];
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = GameObject.Find($"Point{i + 1}").transform;
            if (points[i] == null)
            {
                Debug.LogError($"Point{i + 1}가 씬에 없습니다!");
            }
        }
    }


    void Update()
    {
        MoveToNextPoint();
    }

    private void MoveToNextPoint()
    {
        if (currentPointIndex < points.Length)
        {
            Transform targetPoint = points[currentPointIndex];
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                currentPointIndex++;

                if (currentPointIndex == 2)
                {
                    ReplacePrefab(snowflakePrefabs[0]);
                }
                else if (currentPointIndex == 4) 
                {
                    ReplacePrefab(snowflakePrefabs[1]);
                }
                else if (currentPointIndex == 6)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    private void ReplacePrefab(GameObject newPrefab)
    {
        Vector3 currentPosition = transform.position;
        Quaternion currentRotation = transform.rotation;

        Destroy(gameObject);

        GameObject newObject = Instantiate(newPrefab, currentPosition, currentRotation);

        BallMovement newController = newObject.GetComponent<BallMovement>();
        if (newController != null)
        {
            newController.points = points;
            newController.currentPointIndex = currentPointIndex;
            newController.moveSpeed = moveSpeed;
        }
    }
}