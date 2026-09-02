using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Camera Reference")]
    public Camera playerCamera;

    public bool isActiveWeapon;

    [Header("Shooting State")]
    public bool isShooting;
    public bool readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 0.2f;

    [Header("Burst Settings")]
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    [Header("Spread Settings")]
    [Tooltip("Set to 0 for 100% pinpoint accuracy into crosshair")]
    public float spreadIntensity = 0f;

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 100f;
    public float bulletPrefabLifetime = 3f;

    public enum WeaponModel
    {
        M416,
        AK74,
        Bennelli_M4,
        UZI,
        M1911

    }
    public WeaponModel thisWeaponModel;


    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }

    [Header("Shooting Mode")]
    public ShootingMode currentShootingMode;

    [Header("Weapon Setup")]
    public Vector3 spawnPosition;
    public Vector3 spawnRotation;
    public Vector3 spawnScale = Vector3.one;


    public GameObject muzzleFlashEffect; 



    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (bulletSpawn == null)
        {
            bulletSpawn = transform;
        }
    }



    void Update()
    {
        if (isActiveWeapon)
        {
            if (currentShootingMode == ShootingMode.Auto)
            {
                // Holding Down Left Mouse Button
                isShooting = Input.GetKey(KeyCode.Mouse0) || Input.GetMouseButton(0);
            }
            else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
            {
                // Clicking Left Mouse Button Once
                isShooting = Input.GetKeyDown(KeyCode.Mouse0) || Input.GetMouseButtonDown(0);
            }

            if (readyToShoot && isShooting)
            {
                burstBulletsLeft = bulletsPerBurst;
                FireWeapon();
            }
        }
    }

    private void FireWeapon()
    {
        readyToShoot = false;

        if (muzzleFlashEffect != null)
        {
            ParticleSystem muzzleFlash = muzzleFlashEffect.GetComponent<ParticleSystem>();

            if (muzzleFlash != null)
            {
                muzzleFlash.Play();
            }
        }

        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        // Instantiate or create the bullet
        GameObject bullet = SpawnBullet(bulletSpawn.position, Quaternion.identity);

        // Point the bullet forward to face the shooting direction
        bullet.transform.forward = shootingDirection;

        // Ignore collisions between bullet and player/weapon colliders
        Collider bulletCollider = bullet.GetComponent<Collider>();
        if (bulletCollider != null)
        {
            Collider[] playerColliders = transform.root.GetComponentsInChildren<Collider>();
            foreach (Collider playerCol in playerColliders)
            {
                if (playerCol != bulletCollider)
                {
                    Physics.IgnoreCollision(bulletCollider, playerCol, true);
                }
            }
        }

        // Apply straight velocity and force towards target
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = false; // Fly straight to crosshair without bullet drop
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
        }

        // Destroy the bullet after lifetime
        Destroy(bullet, bulletPrefabLifetime);

        // Reset shot cooldown
        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        // Burst Mode
        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
    }

    private GameObject SpawnBullet(Vector3 position, Quaternion rotation)
    {
        GameObject bullet;
        if (bulletPrefab != null)
        {
            bullet = Instantiate(bulletPrefab, position, rotation);
        }
        else
        {
            // Safe fallback: Create a default bullet if no prefab is assigned
            bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bullet.name = "Bullet (Default)";
            bullet.transform.position = position;
            bullet.transform.rotation = rotation;
            bullet.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

            Renderer rend = bullet.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = Color.yellow;
            }

            if (!bullet.TryGetComponent<Bullet>(out _))
            {
                bullet.AddComponent<Bullet>();
            }
        }

        if (!bullet.TryGetComponent<Rigidbody>(out _))
        {
            bullet.AddComponent<Rigidbody>();
        }

        return bullet;
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        Vector3 targetPoint = Vector3.zero;

        if (playerCamera != null)
        {
            // Raycast through center of screen/crosshair
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit[] hits = Physics.RaycastAll(ray, 500f);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            bool hitFound = false;
            foreach (RaycastHit hit in hits)
            {
                // Make sure raycast doesn't hit the player or gun colliders
                if (hit.transform.root != transform.root && !hit.collider.isTrigger)
                {
                    targetPoint = hit.point;
                    hitFound = true;
                    break;
                }
            }

            if (!hitFound)
            {
                targetPoint = ray.GetPoint(100f);
            }
        }
        else
        {
            targetPoint = bulletSpawn.position + bulletSpawn.forward * 100f;
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        // Apply spread relative to camera orientation (screen X and Y)
        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        Vector3 spread = Vector3.zero;
        if (playerCamera != null)
        {
            spread = playerCamera.transform.right * x + playerCamera.transform.up * y;
        }

        return direction + spread;
    }
}
