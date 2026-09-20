using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; set; }

    private Camera playerCamera;

    public WeaponScript hoveredWeapon = null;   //variable stores a reference to the WeaponScript component
    public AmmoBoxScript hoveredAmmoBox = null; 
    public ThrowableScript hoveredThrowable = null;

    private bool showItemInfo;  //Item panel


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


    private void Start()
    {
        playerCamera = Camera.main;   //Store camera reference instead of searching Camera.main every frame
    }


    private void Update()
    {
        showItemInfo = false;

        //Disable previous hovered object's outline before checking new object
        ClearHoveredObjects();

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f,0.5f,0));
        RaycastHit hit;             //This stores information if the ray hits something.

        if(Physics.Raycast(ray, out hit))
        {
            GameObject objectHitByRaycast = hit.transform.gameObject;   //Get Hit Object (gun)

            //Check if the object contains WeaponScript
            WeaponScript weapon = objectHitByRaycast.GetComponent<WeaponScript>();

            if(weapon != null && weapon.isActiveWeapon == false) //Check Active Weapon. Suppose you're already holding the gun -> You don't want to outline it.
            {
                hoveredWeapon = weapon; //hoveredWeapon points to the WeaponScript attached to the GameObject.

                Outline outline = hoveredWeapon.GetComponent<Outline>();

                if(outline != null)
                {
                    outline.enabled = true;   //Enable weapon outline when looking at it
                }

                HUDManager.Instance.ShowItemInfo(hoveredWeapon.weaponIcon,hoveredWeapon.weaponName,hoveredWeapon.weaponDescription); //For information panel
                showItemInfo = true;

                //Check if player is reloading before allowing weapon pickup
                WeaponScript activeWeapon = WeaponManager.Instance.activeWeaponSlot.GetComponentInChildren<WeaponScript>();

                bool canPickup = activeWeapon == null || !activeWeapon.isReloading;

                if(Input.GetKeyDown(KeyCode.Q) && canPickup)
                {
                    WeaponManager.Instance.PickupWeapon(hoveredWeapon.gameObject);
                }
            }


            //Check if the object contains AmmoBoxScript
            AmmoBoxScript ammoBox = objectHitByRaycast.GetComponent<AmmoBoxScript>();
            if(ammoBox != null)
            {
                hoveredAmmoBox = ammoBox;  //Reference for AmmoBoxScript

                Outline outline = hoveredAmmoBox.GetComponent<Outline>();

                if(outline != null)
                {
                    outline.enabled = true;  //Enable ammo box outline
                }

                HUDManager.Instance.ShowItemInfo(hoveredAmmoBox.ammoIcon, hoveredAmmoBox.ammoName, hoveredAmmoBox.ammoDescription); //For information panel
                showItemInfo = true;

                if(Input.GetKeyDown(KeyCode.Q))
                {
                    WeaponManager.Instance.PickupAmmo(hoveredAmmoBox);

                    Destroy(hoveredAmmoBox.gameObject, 1f);
                }
            }


            //Check if the object contains ThrowableScript
            ThrowableScript throwable = objectHitByRaycast.GetComponent<ThrowableScript>();
            if(throwable != null)
            {
                hoveredThrowable = throwable;  

                Outline outline = hoveredThrowable.GetComponent<Outline>();

                if(outline != null)
                {
                    outline.enabled = true;  
                }

                HUDManager.Instance.ShowItemInfo(hoveredThrowable.throwablesIcon, hoveredThrowable.throwablesName, hoveredThrowable.throwablesDescription); //For information panel
                showItemInfo = true;

                if(Input.GetKeyDown(KeyCode.Q))
                {
                    WeaponManager.Instance.PickupThrowable(hoveredThrowable);
                }
            }
        }


        if(!showItemInfo)
        {
            HUDManager.Instance.HideItemInfo();
        }
    }


    private void ClearHoveredObjects()
    {
        //Remove previous weapon outline
        if(hoveredWeapon != null)
        {
            Outline outline = hoveredWeapon.GetComponent<Outline>();

            if(outline != null)
            {
                outline.enabled = false;
            }

            hoveredWeapon = null;
        }

        //Remove previous ammo box outline
        if(hoveredAmmoBox != null)
        {
            Outline outline = hoveredAmmoBox.GetComponent<Outline>();

            if(outline != null)
            {
                outline.enabled = false;
            }

            hoveredAmmoBox = null;
        }

        //Remove previous throwables outline
        if(hoveredThrowable != null)
        {
            Outline outline = hoveredThrowable.GetComponent<Outline>();

            if(outline != null)
            {
                outline.enabled = false;
            }

            hoveredThrowable = null;
        }
    }
}