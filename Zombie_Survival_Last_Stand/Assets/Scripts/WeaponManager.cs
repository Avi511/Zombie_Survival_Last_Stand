using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance{ get; set; }

    public List<GameObject> weaponSlots; 

    public GameObject activeWeaponSlot;
    private int currentWeaponIndex = 0; //For the gun change scroll feature


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

        Weapon activeWeapon = activeWeaponSlot.GetComponentInChildren<Weapon>();
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f)
        {
            currentWeaponIndex = (currentWeaponIndex + 1) % weaponSlots.Count;
            SwitchActiveSlot(currentWeaponIndex);
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
    
        Weapon weapon = pickedupWeapon.GetComponent<Weapon>();

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

            weaponToDrop.GetComponent<Weapon>().isActiveWeapon = false;
            weaponToDrop.GetComponent<Weapon>().animator.enabled = false;

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
            Weapon currentWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();   //weapon inside the slot that is currently active (the weapon you're holding before switching).
            currentWeapon.isActiveWeapon = false; //Marks this weapon as inactive.
        }

        activeWeaponSlot = weaponSlots[slotNumber];

        if(activeWeaponSlot.transform.childCount > 0)
        {
            Weapon newWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();       //weapon inside the slot you're switching to (the weapon you'll hold after switching).
            newWeapon.isActiveWeapon = true;    //Marks the weapon in the new slot as active.
        }
    }



}
