using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(menuName = "PowerUps/SpeedBuff")]
public class SpeedBuff : PowerUpEffect
{
    public float amount;

    public override void Apply(GameObject target)
    {
        if (target != null)
        {
            target.GetComponent<PlayerController>().moveAcc += amount;
            target.GetComponent<PlayerController>().CheckSpeed(amount);
        }
    }
}
