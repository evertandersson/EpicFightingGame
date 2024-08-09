using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    PlayerController playerController;

    private RectTransform prefab;

    private RectTransform waypoint;

    Vector3 offset = new Vector3(0, 6, 0);

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();

        if (gameObject == playerController.Player1GO)
            prefab = FindObjectOfType<PlayerSelection>().Player1Waypoint;
        else if (gameObject == playerController.Player2GO)
            prefab = FindObjectOfType<PlayerSelection>().Player2Waypoint;

        var canvas = GameObject.Find("WaypointsUI").transform;

        waypoint = Instantiate(prefab, canvas);
    }

    // Update is called once per frame
    void Update()
    {
        var screenPos = Camera.main.WorldToScreenPoint(transform.position + offset);
        waypoint.position = screenPos;
    }
}
