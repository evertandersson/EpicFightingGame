using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/HealthBuff")]
public class HealthBuff : PowerUpEffect
{
    public float amount;
    public override void Apply(GameObject target)
    {
        if (target != null)
        {
            target.GetComponent<PlayerController>().currentHealth += amount;
            target.GetComponent<PlayerController>().UpdateHealth();
        }
    }

}
