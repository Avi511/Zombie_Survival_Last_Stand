using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; set; }

    private Camera playerCamera;

    public Weapon hoveredWeapon = null;   //variable stores a reference to the WeaponScript component
    

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
        //Disable previous hovered object's outline before checking new object
        ClearHoveredObjects();

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f,0.5f,0));
        RaycastHit hit;             //This stores information if the ray hits something.

        if(Physics.Raycast(ray, out hit))
        {
            GameObject objectHitByRaycast = hit.transform.gameObject;   //Get Hit Object (gun)

            //Check if the object contains WeaponScript
            Weapon weapon = objectHitByRaycast.GetComponent<Weapon>();

            if(weapon != null && weapon.isActiveWeapon == false) //Check Active Weapon. Suppose you're already holding the gun -> You don't want to outline it.
            {
                hoveredWeapon = weapon; //hoveredWeapon points to the WeaponScript attached to the GameObject.

                Outline outline = hoveredWeapon.GetComponent<Outline>();

                if(outline != null)
                {
                    outline.enabled = true;   //Enable weapon outline when looking at it
                }

                //Check if player is reloading before allowing weapon pickup
                Weapon activeWeapon = WeaponManager.Instance.activeWeaponSlot.GetComponentInChildren<Weapon>();

                if(Input.GetKeyDown(KeyCode.Q))
                {
                    WeaponManager.Instance.PickupWeapon(hoveredWeapon.gameObject);
                }
            }

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

    }
}