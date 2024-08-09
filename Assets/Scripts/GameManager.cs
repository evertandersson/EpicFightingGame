using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public List<Transform> targets;
    [SerializeField] Transform centerPoint;
    public LayerMask playerLayer;

    private float radius = 110f;

    public TextMeshProUGUI FPS;
    public float deltaTime;

    public void GetFPSText(TextMeshProUGUI FPSText)
    {
        FPS = FPSText;
    }

    void Update()
    {
        if (FPS != null)
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;
            FPS.text = Mathf.Ceil(fps).ToString();
        }
    }

    private void LateUpdate()
    {
        UpdateList();
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

}
