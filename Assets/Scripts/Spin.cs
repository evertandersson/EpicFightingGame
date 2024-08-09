using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    void FixedUpdate()
    {
        transform.Rotate(0, 40f * Time.deltaTime, 0, Space.Self);
    }
}
