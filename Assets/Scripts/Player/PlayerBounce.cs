using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBounce : MonoBehaviour
{
    [SerializeField] PlayerController player1;
    [SerializeField] PlayerController player2;

    private float attackImpact = 50f;
    private float damageP1;
    private float damageP2;
    private float speedCompare = 0.5f;

    public void GetPlayers()
    {
        player1 = GetComponent<PlayerController>().Player1GO.GetComponent<PlayerController>();
        player2 = GetComponent<PlayerController>().Player2GO.GetComponent<PlayerController>();
        player1.SetUpStats();
        player2.SetUpStats();
    }

    public void SetDamage()
    {
        if (player1 != null || player2 != null) 
        {
            damageP1 = player1.damage;
            damageP2 = player2.damage;
        } 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            AttackPlayer();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Attack"))
            AttackPlayer();
    }

    void AttackPlayer()
    {
        float p1MoveSpeed = Mathf.Abs(player1.MoveSpeed);
        float p2MoveSpeed = Mathf.Abs(player2.MoveSpeed);

        if (!player2.attackAnimPlaying)
        {
            if (p1MoveSpeed > (p2MoveSpeed + speedCompare) && !player1.attackAnimPlaying)
            {
                player2.Bounce(player1.MoveSpeed / 2);
            }
            else if (p1MoveSpeed > (p2MoveSpeed + speedCompare) && player1.attackAnimPlaying && !player1.alreadyAttacked)
            {
                player2.Bounce((player1.MoveSpeed * attackImpact) / 8);
                player1.alreadyAttacked = true;
                player2.TakeDamage(damageP1 * (p1MoveSpeed / 8));
            }
            else if (player1.attackAnimPlaying && !player1.alreadyAttacked)
            {
                player2.Bounce(attackImpact * player1.snapToRot);
                player1.alreadyAttacked = true;
                player2.TakeDamage(damageP1);
            }
        }
        if (!player1.attackAnimPlaying)
        {
            if (p2MoveSpeed > (p1MoveSpeed + speedCompare) && !player2.attackAnimPlaying)
            {
                player1.Bounce(player2.MoveSpeed / 2);
            }
            else if (p2MoveSpeed > (p1MoveSpeed + speedCompare) && player2.attackAnimPlaying && !player2.alreadyAttacked)
            {
                player1.Bounce((player2.MoveSpeed * attackImpact) / 8);
                player2.alreadyAttacked = true;
                player1.TakeDamage(damageP2 * (p2MoveSpeed / 8));
            }
            else if (player2.attackAnimPlaying && !player2.alreadyAttacked)
            {
                player1.Bounce(attackImpact * player2.snapToRot);
                player2.alreadyAttacked = true;
                player1.TakeDamage(damageP2);
            }
        }
    }
}
