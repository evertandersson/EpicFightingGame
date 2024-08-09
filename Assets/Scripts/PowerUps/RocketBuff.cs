using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Rocket")]
public class RocketBuff : PowerUpEffect
{
    public override void Apply(GameObject target)
    {
        if (!target.GetComponent<PlayerController>().rocketPickedUp)
        {
            target.GetComponent<PlayerController>().rocketPickedUp = true;
            FindObjectOfType<HealthBar>().UpdateRocketIcon(target);
        }
    }
}
