using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    float speed = 20f;
    public GameObject targetObj;
    public GameObject explosion;
    bool confirmed = false;
    Vector3 offset = new Vector3(0, 3f, 0);
    int rightOrLeft;
    float bouncePower = 100f;
    float damage = 70f;


    public void OnLaunch(GameObject otherPlayer)
    {
        targetObj = otherPlayer;
        transform.LookAt(otherPlayer.transform.position + offset);
        targetObj.transform.position = otherPlayer.transform.position;
        confirmed = true;
        Invoke("Explode", 3f);
    }

    private void FixedUpdate()
    {
        if (confirmed)
        {
            if (targetObj != null)
            {
                // Rotate the rocket to face the target
                Vector3 direction = ((targetObj.transform.position + offset) - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed);

                // Move the rocket forward
                transform.Translate(Vector3.forward * speed * Time.deltaTime);

                if (direction.x <= 0)
                    rightOrLeft = -1;
                else
                    rightOrLeft = 1;
            }
            else
            {
                // No target, destroy the rocket
                Destroy(gameObject);
            }
        }
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == targetObj)
        {
            // Apply damage or other effects to the player (customize this part)
            Instantiate(explosion, transform.position, transform.rotation);
            targetObj.GetComponent<PlayerController>().Bounce(bouncePower * rightOrLeft);
            targetObj.GetComponent<PlayerController>().TakeDamage(damage);

            // Destroy the rocket on impact
            Destroy(gameObject);
        }
    }

    void Explode()
    {
        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
