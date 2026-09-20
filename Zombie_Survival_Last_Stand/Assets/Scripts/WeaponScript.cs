using System;
using System.Collections;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    //public Camera playerCamera;         //Instead of firing straight from the gun, it fires toward the center of the screen.
        //->This reference is cleared when the weapon is saved as a prefab because the camera exists only in the scene.(Not an asset stored in the Project folder)
        //->Main Camera exists only in the currently open scene, not in the Project folder. Thats the issue.
        // But the Bullet prefab is an asset stored in the Project folder. So we can Reference it. Children of that prefab can also be referenced, as long as they are part of that prefab (or another asset).
        //->Since the weapon to  be a prefab, using Camera.main is a common and simple solution when your game has only one main camera.
        //Prefabs can store references to assets in the Project window, but they cannot permanently store references to objects in the Hierarchy (scene objects).

    public bool isActiveWeapon;
    public bool isADS;

    public int weaponDamage;

    //Shooting
    [Header("Shooting")] 
    public bool isShooting;             //Stores whether the player is pressing the fire button.
    public bool readyToShoot;           //Prevents shooting too quickly.     Gun fire rate:1 bullet every second, When a shot is fired: readyToShoot = false , then After one second: readyToShoot = true                
    bool allowReset = true;             //Makes sure only one cooldown timer is created.
    public float shootingDelay = 1f;    //Time between shots.


    //Burst Mode
    [Header("Burst")] 
    public int bulletsPerBurst = 3;     //Number of bullets fired when Burst mode is selected
    public int burstBulletLeft;         //Keeps track of how many bullets remain in the current burst.


    [Header("Spread")] 
    public float spreadIntensity;       //Controls weapon accuracy.
    public float hipSpreadIntensity;
    public float adsSpreadIntensity;



    //Bullet
    [Header("Bullet")] 
    public GameObject bulletPrefab;
    public Transform bulletSpawn;   //Assign a new Empty Game object at end of the gun
    public float bulletVelocity = 30f;
    public float bulletPrefabLifeTime = 3f;


    public enum WeaponModel
    {
        M416,
        AK74,
        Bennelli_M4,
        M107,
        M249,
        M1911

    }
    public WeaponModel thisWeaponModel;


    public enum ShootingMode
    {
        Single,
        Burst,
        Auto   
    }
    public ShootingMode currentShootingMode;    //Stores the currently selected shooting mode.

    public GameObject muzzleFlashEffect;        //Fire Flash
    internal Animator animator;    //For Idle movement, Recoil purpose, Reload purpose


    [Header("Reload")] 
    //For Loading/Reloading Purpose
    public float reloadTime;
    public int magazineSize;
    public int bulletsLeft;
    public bool isReloading;



    [Header("Weapon Sound")]                //Here we can easily assign.Because the audio is in assets, not in scene view
    public AudioClip fireSound; 
    public AudioClip emptyMagazineSound;
    public AudioClip reloadSound;


    [Header("Weapon Spawn")] 
    public Vector3 spawnPosition;
    public Vector3 spawnRotation;
    public Vector3 spawnScale;



    [Header("Weapon Information")]
    public Sprite weaponIcon;
    public string weaponName;
    [TextArea]
    public string weaponDescription;



    private void Awake()                //Runs before the game starts. so initializes weapon
    {
        readyToShoot = true;                //Gun ready to shoot
        burstBulletLeft = bulletsPerBurst;  //Burst bullets reset
        animator = GetComponent<Animator>();//Allocate Component here

        spreadIntensity = hipSpreadIntensity;
        bulletsLeft = magazineSize;
    }


    void Update()
    {
        if(isActiveWeapon)
        {

            if(Input.GetMouseButtonDown(1))
            {
                EnterADS();
            }
            if(Input.GetMouseButtonUp(1))
            {
                ExitADS();
            }


            if(bulletsLeft == 0 && isShooting)
            {
                AudioManager.Instance.PlayEmptyMagazineSound(emptyMagazineSound);
            }

            if(currentShootingMode == ShootingMode.Auto)
            {
                //Holding Down Left Mouse Button
                isShooting = Input.GetKey(KeyCode.Mouse0);
            }

            else if(currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
            {
                //Clicking Left Mouse Button Once
                isShooting = Input.GetKeyDown(KeyCode.Mouse0);
            }


            if(readyToShoot && isShooting && bulletsLeft>0 && isReloading==false)
            {
                burstBulletLeft = bulletsPerBurst;
                FireWeapon();
            }

            //Manual Reload
            if(Input.GetKeyDown(KeyCode.R) && bulletsLeft<magazineSize && isReloading==false && WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > 0)  
            {
                Reload();
            }
            //Automatic Reload
            if(readyToShoot && isShooting==false && bulletsLeft <= 0 && isReloading==false && WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > 0)
            {
                Reload();
            }

        }
    }


    private void EnterADS()
    {
        isADS = true;
        HUDManager.Instance.crossHairDot.SetActive(false);
        spreadIntensity = adsSpreadIntensity;   //To be more accurate
        animator.SetTrigger("EnterADS");    //Idle_ads
    }

    private void ExitADS()
    {
        isADS = false;
        HUDManager.Instance.crossHairDot.SetActive(true);
        spreadIntensity = hipSpreadIntensity;   //To be less accurate
        animator.SetTrigger("ExitADS");
    }




    private void FireWeapon()
    {
        if (bulletsLeft == 0)
        {
            return;
        }

        bulletsLeft--;

        muzzleFlashEffect.GetComponent<ParticleSystem>().Play();        //Whenever WeaponFires -> FireFlash Plays

        if(isADS)
        {
            animator.SetTrigger("RECOIL_ADS");//ADS Recoil animation
        }
        else
        {
            animator.SetTrigger("RECOIL");  //Same as the name given in parameter(Trigger) => Whenever WeaponFires Recoil animation triggers. Also Disable loop time in animation (Loop for idle and Non Loop for Recoil)
        }
        
        AudioManager.Instance.PlayFireSound(fireSound);  //Play Gun Fire Sound


        readyToShoot = false;   //Stop Immediate Shooting

        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;   //Gets the bullet direction, .normalized makes its length equal to 1 so the speed stays consistent regardless of distance.


        //Instantiate bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position , Quaternion.identity);

        BulletScript bulletScript = bullet.GetComponent<BulletScript>();
        bulletScript.bulletDamage = weaponDamage;

        //Pointing the bullet to face the shooting direction. Makes the bullet face the direction it's traveling.
        bullet.transform.forward = shootingDirection;   

        //Shoot the bullet
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity , ForceMode.Impulse); 
    
        //Destroy the bullet after some delay. So Coroutine
        StartCoroutine(DestroyBulletAfterTime(bullet , bulletPrefabLifeTime));



        //Checking if we are done shooting
        if (allowReset) 
        {
            Invoke("ResetShot", shootingDelay);  //Schedules ResetShot() to run after the specified delay so the weapon becomes ready to fire again.
            allowReset = false;
        }


        //Burst Mode -> Keeps firing until all bullets in the burst have been shot.
        if(currentShootingMode == ShootingMode.Burst && burstBulletLeft > 1)   //We already shoot once before this check
        {
            burstBulletLeft --;
            Invoke("FireWeapon", shootingDelay);    //Invoke method to call a method after a specified delay
        }

    }



    private void Reload()
    {
        HUDManager.Instance.ShowReloadUI();

        AudioManager.Instance.PlayReloadSound(reloadSound);
        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);

        StartCoroutine(ReloadProgress());   //For ring sprite UI

        if (isADS)
        {
            //ADS Reload animation
        }
        else
        {
            animator.SetTrigger("RELOAD");  //Reload animation
        }

    }


    private void ReloadCompleted()
    {
        int bulletsNeeded = magazineSize - bulletsLeft;
        int availableAmmo = WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel);

        int bulletsToLoad = Mathf.Min(bulletsNeeded, availableAmmo);

        bulletsLeft += bulletsToLoad;

        WeaponManager.Instance.DecreaseTotalAmmo(bulletsToLoad, thisWeaponModel);

        isReloading = false;
    }


    private void ResetShot()    //Ends the cooldown
    {
        readyToShoot = true;
        allowReset = true;
    }


    public Vector3 CalculateDirectionAndSpread()    //This function determines where the bullet should travel.
    {
        //Shooting from the middle of the screen to check where are we pointing at
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));  //Creates a ray from the center of the screen. Where the crosshair usually is.
        RaycastHit hit;                                                         //This creates a variable to store information if the ray hits something

        Vector3 targetPoint;
        if(Physics.Raycast(ray , out hit))  //Checks whether the ray hits an object
        {
            //Hitting something
            targetPoint = hit.point;        //(10,4,8)
        }
        else
        {
            //Shooting at the air
            targetPoint = ray.GetPoint(100);
        }


        Vector3 direction = targetPoint - bulletSpawn.position;     //Finds the direction from the gun's muzzle to the target point


        //Adds random horizontal and vertical offsets. This creates bullet spread.
        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);


        //Returning the direction and spread
        return direction + new Vector3(x,y,0);

    }


    private IEnumerator DestroyBulletAfterTime(GameObject bullet , float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet); 
    }



    private IEnumerator ReloadProgress()
    {
        float timer = 0f;

        while(timer < reloadTime)
        {
            timer = timer + Time.deltaTime;

            float progress = timer / reloadTime;

            HUDManager.Instance.UpdateReloadProgress(progress);

            yield return null;
        }

        HUDManager.Instance.HideReloadUI();
    }

}