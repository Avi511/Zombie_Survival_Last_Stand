using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision objectWeHit)
    {
        // Don't collide with the player who fired
        if (objectWeHit.gameObject.CompareTag("Player"))
        {
            return;
        }

        if (objectWeHit.gameObject.CompareTag("Target"))
        {
            print("hit " + objectWeHit.gameObject.name + " !");

            // Apply impact force to the target Rigidbody to knock it down
            Rigidbody targetRb = objectWeHit.gameObject.GetComponent<Rigidbody>();
            if (targetRb != null)
            {
                targetRb.AddForce(transform.forward * 10f, ForceMode.Impulse);
            }

            Destroy(gameObject);
        }
        else
        {
            // Destroy on any other solid collision
            Destroy(gameObject);
        }
    }
}
