using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Bullet Settings")]
    [Tooltip("The bullet prefab to shoot. If left empty, a default bullet will be generated automatically.")]
    public GameObject bulletPrefab;
    [Tooltip("The point from which bullets spawn. If left empty, the weapon's transform is used.")]
    public Transform bulletSpawn;
    [Tooltip("Speed of the bullet.")]
    public float bulletVelocity = 50f;
    [Tooltip("Lifetime of the bullet before being destroyed.")]
    public float bulletPrefabLifetime = 3f;

    [Header("Firing Settings")]
    [Tooltip("Cooldown between shots in seconds.")]
    public float fireRate = 0.2f;
    [Tooltip("If true, holding down the fire button will shoot continuously.")]
    public bool isAutomatic = false;

    [Header("Aiming")]
    [Tooltip("If true, bullets are aimed towards the center of the screen/crosshair.")]
    public bool aimTowardsCrosshair = true;

    private float nextTimeToFire = 0f;

    void Update()
    {
        // Detect fire input (Left Mouse Button, Mouse0, or Fire1 axis)
        bool fireInput = isAutomatic 
            ? (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Mouse0) || Input.GetButton("Fire1"))
            : (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Mouse0) || Input.GetButtonDown("Fire1"));

        if (fireInput && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            FireWeapon();
        }
    }

    public void FireWeapon()
    {
        // Determine spawn position & rotation
        Vector3 spawnPos = bulletSpawn != null ? bulletSpawn.position : transform.position;
        Quaternion spawnRot = bulletSpawn != null ? bulletSpawn.rotation : transform.rotation;

        // Calculate shooting direction
        Vector3 shootDirection = bulletSpawn != null ? bulletSpawn.forward : transform.forward;

        if (aimTowardsCrosshair && Camera.main != null)
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Vector3 targetPoint;

            if (Physics.Raycast(ray, out RaycastHit hit, 500f))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.GetPoint(500f);
            }

            shootDirection = (targetPoint - spawnPos).normalized;
            spawnRot = Quaternion.LookRotation(shootDirection);
        }

        // Instantiate or create bullet
        GameObject bullet;
        if (bulletPrefab != null)
        {
            bullet = Instantiate(bulletPrefab, spawnPos, spawnRot);
        }
        else
        {
            // Fallback: Create a default sphere bullet if no prefab is assigned
            bullet = CreateDefaultBullet(spawnPos, spawnRot);
        }

        // Ensure bullet has Rigidbody
        if (!bullet.TryGetComponent<Rigidbody>(out var rb))
        {
            rb = bullet.AddComponent<Rigidbody>();
        }

        rb.isKinematic = false;
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        
        // Add impulse force in shoot direction
        rb.AddForce(shootDirection * bulletVelocity, ForceMode.Impulse);

        // Ensure bullet has the Bullet script for hit detection
        if (!bullet.TryGetComponent<Bullet>(out _))
        {
            bullet.AddComponent<Bullet>();
        }

        // Ignore collision between bullet and player
        Collider bulletCol = bullet.GetComponent<Collider>();
        Collider[] playerCols = transform.root.GetComponentsInChildren<Collider>();
        if (bulletCol != null)
        {
            foreach (var pCol in playerCols)
            {
                if (pCol != bulletCol)
                {
                    Physics.IgnoreCollision(bulletCol, pCol, true);
                }
            }
        }

        // Destroy after lifetime
        Destroy(bullet, bulletPrefabLifetime);
    }

    private GameObject CreateDefaultBullet(Vector3 position, Quaternion rotation)
    {
        GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bullet.name = "DefaultBullet";
        bullet.transform.position = position;
        bullet.transform.rotation = rotation;
        bullet.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        // Make it yellow
        Renderer rend = bullet.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = Color.yellow;
        }

        return bullet;
    }
}

