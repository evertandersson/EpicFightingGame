using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    public List<Transform> targets;
    [SerializeField] Transform centerPoint;
    public LayerMask playerLayer;

    private float radius = 70f;

    private Vector3 offset = new Vector3(0,0,-50f);
    public float smoothTime = 0.5f;

    public float minZoom = 40f;
    public float maxZoom = 10f;
    public float zoomLimiter = 50f;

    private Vector3 velocity;
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        UpdateList();
        Move();
        Zoom();
    }

    void UpdateList()
    {
        targets.Clear();

        Collider[] colliders = Physics.OverlapSphere(centerPoint.position, radius, playerLayer);

        foreach (var collider in colliders)
        {
            Transform playerController = collider.GetComponent<PlayerController>().transform;
            if (playerController != null)
            {
                targets.Add(playerController);
            }
        }
    }

    private void Move()
    {
        Vector3 centerPoint = GetCenterPoint();

        Vector3 newPosition = centerPoint + offset;

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    void Zoom()
    {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
    }

    float GetGreatestDistance()
    {
        var bounds = new Bounds(centerPoint.position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        float size = (bounds.size.x + bounds.size.y) / 2;

        return size;
    }

    Vector3 GetCenterPoint()
    {
        if (targets.Count == 0) return centerPoint.position;

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }
        return bounds.center;
    }
}
