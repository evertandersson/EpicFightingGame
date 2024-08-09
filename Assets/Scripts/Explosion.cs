using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Explosion : MonoBehaviour
{
    ParticleSystem explosion;

    private void Start()
    {
        explosion = GetComponent<ParticleSystem>();
        explosion.Play();
        Destroy(gameObject, explosion.main.duration);
    }


}
