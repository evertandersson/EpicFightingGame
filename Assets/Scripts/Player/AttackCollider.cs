using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    [SerializeField] PlayerController player;
    private Collider collider;

    private void Start()
    {
        player = GetComponentInParent<PlayerController>();
        collider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (player.attackAnimPlaying && !player.alreadyAttacked)
        {
            collider.enabled = true;
        }
        else
            collider.enabled = false;
    }
}
