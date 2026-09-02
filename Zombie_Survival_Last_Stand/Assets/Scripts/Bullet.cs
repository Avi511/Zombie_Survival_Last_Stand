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

            CreateBulletImpactEffect(objectWeHit);

            // Apply impact force to the target Rigidbody to knock it down
            Rigidbody targetRb = objectWeHit.gameObject.GetComponent<Rigidbody>();
            if (targetRb != null)
            {
                targetRb.AddForce(transform.forward * 15f, ForceMode.Impulse);
                targetRb.useGravity = true;
            }

            Destroy(gameObject);
        }
        if (objectWeHit.gameObject.CompareTag("Wall"))
        {
            print("hit " + objectWeHit.gameObject.name + " !");

            CreateBulletImpactEffect(objectWeHit);

            Destroy(gameObject);
        }
        else
        {
            // Destroy on any other solid collision
            Destroy(gameObject);
        }
    }

    void CreateBulletImpactEffect(Collision objectWeHit)
    {
         ContactPoint contact = objectWeHit.contacts[0];

         GameObject hole = Instantiate(GlobalReferences.Instance.bulletImpactEffectPrefab, contact.point, Quaternion.LookRotation(contact.normal));

         hole.transform.SetParent(objectWeHit.gameObject.transform);
    }
}
