using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public PowerUpEffect powerUpEffect;

    private void Start()
    {
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider collision)
    {
        powerUpEffect.Apply(collision.gameObject);
        gameObject.SetActive(false);
        Invoke("Respawn", 10f);
    }

    void Respawn()
    {
        gameObject.SetActive(true);
    }
}
