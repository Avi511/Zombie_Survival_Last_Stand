using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Hit Settings")]
    [Tooltip("Damage dealt upon impact.")]
    public float damage = 25f;
    [Tooltip("Whether the bullet destroys itself when hitting any obstacle or target.")]
    public bool destroyOnAnyHit = true;

    private void OnCollisionEnter(Collision collision)
    {
        HandleHit(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleHit(other.gameObject);
    }

    private void HandleHit(GameObject hitObject)
    {
        // Don't collide with the player who shot it or other bullets
        if (hitObject.CompareTag("Player") || hitObject.GetComponent<Bullet>() != null)
        {
            return;
        }

        Debug.Log("Bullet hit: " + hitObject.name);

        // Check if the object hit is an Enemy, Target, or named Target/Enemy
        string objNameLower = hitObject.name.ToLower();
        if (hitObject.CompareTag("Enemy") || hitObject.CompareTag("Target") || objNameLower.Contains("target") || objNameLower.Contains("enemy"))
        {
            Debug.Log("Destroyed: " + hitObject.name);
            Destroy(hitObject);
        }

        // Destroy the bullet upon hitting anything solid
        if (destroyOnAnyHit)
        {
            Destroy(gameObject);
        }
    }
}

