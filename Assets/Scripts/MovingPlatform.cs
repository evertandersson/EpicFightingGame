using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    float moveSpeed = 10f;
    public List<Vector3> patrolPoints;
    public Rigidbody rb;

    private GameObject target = null;
    private Vector3 offset;

    private int targetPos = 0;

    private void Start()
    {
        target = null;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (patrolPoints != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, patrolPoints[targetPos], moveSpeed * Time.deltaTime);
            if (transform.position == patrolPoints[targetPos])
            {
                targetPos++;
                if (targetPos >= patrolPoints.Count)
                    targetPos = 0;
            }
        }
    }

}
