using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance{ get; set; }

    public List<GameObject> weaponSlots; 

    public GameObject activeWeaponSlot;
    private int currentWeaponIndex = 0; //For the gun change scroll feature


    [Header("Ammo")]
    public int total_5_56_Ammo = 0;
    public int total_BMG_Ammo = 0;
    public int total_ACP_Ammo = 0;
    public int total_Gauge_Ammo = 0;


    [Header("Throwables")]
    public float throwForce = 15f;
    public GameObject throwableSpawn;
    public float forceMultiplier = 0;
    public float forceMultiplierLimit = 3f;


    [Header("Lethals")]
    public int lethalsCount = 0;
    public int maxLethalsCount = 3;
    public GameObject grenadePrefab;
    public ThrowableScript.ThrowableType equippedLethalType;

    [Header("Tacticals")]
    public int tacticalsCount = 0;
    public int maxTacticalsCount = 2;
    public GameObject smokeGrenadePrefab;
    public ThrowableScript.ThrowableType equippedTacticalType;


    private void Awake()
    {
        if(Instance != null && Instance != this)    //Singleton
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }


    public void Start()
    {
        activeWeaponSlot = weaponSlots[0];

        equippedLethalType = ThrowableScript.ThrowableType.None;
        equippedTacticalType = ThrowableScript.ThrowableType.None;
    }


    public void Update()
    {
        foreach(GameObject weaponSlot in weaponSlots)
        {
            if(weaponSlot == activeWeaponSlot)
            {
                weaponSlot.SetActive(true);
            }
            else
            {
                weaponSlot.SetActive(false);
            }
        }

        WeaponScript activeWeapon = activeWeaponSlot.GetComponentInChildren<WeaponScript>();
        bool canSwitchGun = activeWeapon == null || !activeWeapon.isReloading;

        if (canSwitchGun)
        {
            // Mouse Scroll Wheel
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll != 0f)
            {
                currentWeaponIndex = (currentWeaponIndex + 1) % 2;
                SwitchActiveSlot(currentWeaponIndex);
            }
        }


        //Throwables Lethal
        if(Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.R))
        {
            forceMultiplier = forceMultiplier + Time.deltaTime;

            if(forceMultiplier > forceMultiplierLimit)
            {
                forceMultiplier = forceMultiplierLimit;
            }
        }
        if(Input.GetKeyUp(KeyCode.E))
        {
            if(lethalsCount > 0)
            {
                ThrowLethal();
            }
            forceMultiplier = 0;
        }

        //Throwables Tactical
        if(Input.GetKeyUp(KeyCode.R))
        {
            if(tacticalsCount > 0)
            {
                ThrowTactical();
            }
            forceMultiplier = 0;
        }
    }




    public void PickupWeapon(GameObject pickedupWeapon)
    {
        AddWeaponIntoActiveSlot(pickedupWeapon);
    }

    private void AddWeaponIntoActiveSlot(GameObject pickedupWeapon)
    {
        DropCurrentWeapon(pickedupWeapon);

        pickedupWeapon.transform.SetParent(activeWeaponSlot.transform, false);
    
        WeaponScript weapon = pickedupWeapon.GetComponent<WeaponScript>();

        pickedupWeapon.transform.localPosition = new Vector3(weapon.spawnPosition.x,weapon.spawnPosition.y,weapon.spawnPosition.z);
        pickedupWeapon.transform.localRotation = Quaternion.Euler(weapon.spawnRotation.x,weapon.spawnRotation.y,weapon.spawnRotation.z);
        pickedupWeapon.transform.localScale = new Vector3(weapon.spawnScale.x,weapon.spawnScale.y,weapon.spawnScale.z);
    

        weapon.isActiveWeapon = true;
        Outline outline = pickedupWeapon.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = false;
        }

        weapon.animator.enabled = true;
    }



    private void DropCurrentWeapon(GameObject pickedupWeapon)
    {
        if(activeWeaponSlot.transform.childCount >0 )
        {
            var weaponToDrop = activeWeaponSlot.transform.GetChild(0).gameObject;

            weaponToDrop.GetComponent<WeaponScript>().isActiveWeapon = false;
            weaponToDrop.GetComponent<WeaponScript>().animator.enabled = false;

            weaponToDrop.transform.SetParent(pickedupWeapon.transform.parent);
            weaponToDrop.transform.localPosition = pickedupWeapon.transform.localPosition;
            weaponToDrop.transform.localRotation = pickedupWeapon.transform.localRotation;
            weaponToDrop.transform.localScale = pickedupWeapon.transform.localScale;

        }
    }



    public void SwitchActiveSlot(int slotNumber)
    {
        if(activeWeaponSlot.transform.childCount > 0)  
        {
            WeaponScript currentWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<WeaponScript>();   //weapon inside the slot that is currently active (the weapon you're holding before switching).
            currentWeapon.isActiveWeapon = false; //Marks this weapon as inactive.
        }

        activeWeaponSlot = weaponSlots[slotNumber];

        if(activeWeaponSlot.transform.childCount > 0)
        {
            WeaponScript newWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<WeaponScript>();       //weapon inside the slot you're switching to (the weapon you'll hold after switching).
            newWeapon.isActiveWeapon = true;    //Marks the weapon in the new slot as active.
        }
    }


    public void PickupAmmo(AmmoBoxScript ammoboxScript)
    {
        ammoboxScript.GetComponent<Animator>().enabled = true;

        switch (ammoboxScript.ammoType)
        {
            case AmmoBoxScript.AmmoType.Ammo_5_56:
                total_5_56_Ammo = total_5_56_Ammo + ammoboxScript.ammoAmount;
                break;
            case AmmoBoxScript.AmmoType.Ammo_ACP:
                total_ACP_Ammo = total_ACP_Ammo + ammoboxScript.ammoAmount;
                break;
            case AmmoBoxScript.AmmoType.Ammo_BMG:
                total_BMG_Ammo = total_BMG_Ammo + ammoboxScript.ammoAmount;
                break;
            case AmmoBoxScript.AmmoType.Ammo_Gauge:
                total_Gauge_Ammo = total_Gauge_Ammo + ammoboxScript.ammoAmount;
                break;
        }
    }

    internal void DecreaseTotalAmmo(int bulletsToDecrease, WeaponScript.WeaponModel thisWeaponModel)
    {
        switch(thisWeaponModel)
        {
            case WeaponScript.WeaponModel.AK74:
                total_5_56_Ammo = total_5_56_Ammo - bulletsToDecrease;
                break;
            case WeaponScript.WeaponModel.Bennelli_M4:
                total_Gauge_Ammo = total_Gauge_Ammo - bulletsToDecrease;
                break;
            case WeaponScript.WeaponModel.M107:
                total_BMG_Ammo = total_BMG_Ammo - bulletsToDecrease;
                break;
            case WeaponScript.WeaponModel.M1911:
                total_ACP_Ammo = total_ACP_Ammo - bulletsToDecrease;
                break;
            case WeaponScript.WeaponModel.M249:
                total_5_56_Ammo = total_5_56_Ammo - bulletsToDecrease;
                break;
            case WeaponScript.WeaponModel.M416:
                total_5_56_Ammo = total_5_56_Ammo - bulletsToDecrease;
                break;
        }
    }

    
    
    public int CheckAmmoLeftFor(WeaponScript.WeaponModel thisWeaponModel)
    {
        switch(thisWeaponModel)
        {
            case WeaponScript.WeaponModel.AK74:
                return total_5_56_Ammo;
            case WeaponScript.WeaponModel.Bennelli_M4:
                return total_Gauge_Ammo;
            case WeaponScript.WeaponModel.M107:
                return total_BMG_Ammo;
            case WeaponScript.WeaponModel.M1911:
                return total_ACP_Ammo;
            case WeaponScript.WeaponModel.M249:
                return total_5_56_Ammo;
            case WeaponScript.WeaponModel.M416:
                return total_5_56_Ammo;
            default:
                return 0;
        }
    }





    public void PickupThrowable(ThrowableScript throwable)
    {
        switch(throwable.throwableType)
        {
            case ThrowableScript.ThrowableType.Grenade:
                PickupThrowableAsLethal(ThrowableScript.ThrowableType.Grenade);
                break;
            case ThrowableScript.ThrowableType.SmokeGrenade:
                PickupThrowableAsTactical(ThrowableScript.ThrowableType.SmokeGrenade);
                break;
        }
    }

    private void PickupThrowableAsLethal(ThrowableScript.ThrowableType lethal)
    {
        if(equippedLethalType == lethal || equippedLethalType == ThrowableScript.ThrowableType.None)
        {
            equippedLethalType = lethal;

            if(lethalsCount < maxLethalsCount)
            {
                lethalsCount = lethalsCount + 1;
                Destroy(SelectionManager.Instance.hoveredThrowable.gameObject);
                HUDManager.Instance.UpdateThrowablesUI();
            }
            else
            {
                print("Lethals Limit reached");
            }
        }
        else
        {
            //Cannot pickup different lethal
            //Option to swap lethals
        }


    }


    private void PickupThrowableAsTactical(ThrowableScript.ThrowableType tactical)
    {
        if(equippedTacticalType == tactical || equippedTacticalType == ThrowableScript.ThrowableType.None)
        {
            equippedTacticalType = tactical;

            if(tacticalsCount < maxTacticalsCount)
            {
                tacticalsCount = tacticalsCount + 1;
                Destroy(SelectionManager.Instance.hoveredThrowable.gameObject);
                HUDManager.Instance.UpdateThrowablesUI();
            }
            else
            {
                print("Tacticals Limit reached");
            }
        }
        else
        {
            //Cannot pickup different tacticls
            //Option to swap tacticals
        }


    }




    private void ThrowLethal()
    {
        GameObject lethalPrefab = GetThrowablePrefab(equippedLethalType);

        GameObject throwable = Instantiate(lethalPrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);
        Rigidbody rb = throwable.GetComponent<Rigidbody>();

        rb.AddForce(Camera.main.transform.forward * (throwForce*forceMultiplier), ForceMode.Impulse);
    
        throwable.GetComponent<ThrowableScript>().hasBeenThrown = true;

        lethalsCount = lethalsCount - 1;


        if(lethalsCount <= 0)
        {
            equippedLethalType = ThrowableScript.ThrowableType.None;
        }

        HUDManager.Instance.UpdateThrowablesUI();
    }


    private void ThrowTactical()
    {
        GameObject tacticalPrefab = GetThrowablePrefab(equippedTacticalType);

        GameObject throwable = Instantiate(tacticalPrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);
        Rigidbody rb = throwable.GetComponent<Rigidbody>();

        rb.AddForce(Camera.main.transform.forward * (throwForce*forceMultiplier), ForceMode.Impulse);
    
        throwable.GetComponent<ThrowableScript>().hasBeenThrown = true;

        tacticalsCount = tacticalsCount - 1;


        if(tacticalsCount <= 0)
        {
            equippedTacticalType = ThrowableScript.ThrowableType.None;
        }

        HUDManager.Instance.UpdateThrowablesUI();
    }



    private GameObject GetThrowablePrefab(ThrowableScript.ThrowableType throwableType)
    {
        switch (throwableType)
        {
            case ThrowableScript.ThrowableType.Grenade:
                return grenadePrefab;
            case ThrowableScript.ThrowableType.SmokeGrenade:
                return smokeGrenadePrefab;
        }

        return new();
    }

}
