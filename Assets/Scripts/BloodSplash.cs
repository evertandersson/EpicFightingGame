using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class BloodSplash : MonoBehaviour
{
    ParticleSystem bloodSplash;

    private void Start()
    {
        bloodSplash = GetComponent<ParticleSystem>();
        bloodSplash.Play();
        Destroy(gameObject, bloodSplash.main.duration);
    }

}
