using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance{ get; set; }


    [Header("Ammo")]
    public TextMeshProUGUI magazineAmmoUI;
    public TextMeshProUGUI totalAmmoUI;
    public Image ammoTypeUI;

    [Header("Weapon")]
    public Image activeWeaponUI;
    public Image unActiveWeaponUI;


    [Header("Throwables")]
    public Image lethalUI;
    public TextMeshProUGUI lethalAmountUI;
    public Image tacticalUI;
    public TextMeshProUGUI tacticalAmountUI;

    [Header("Other")]
    public Sprite emptySlot;
    public GameObject crossHairDot;

    [Header("Reload")]
    public Image reloadSpriteUI;


    [Header("Item Info")]
    public GameObject itemInfoPanel;
    public Image itemIcon;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;


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


    private void Update()
    {
        WeaponScript activeWeapon = WeaponManager.Instance.activeWeaponSlot.GetComponentInChildren<WeaponScript>(); //Get the WeaponScript of active weapon
        WeaponScript unActiveWeapon = GetUnActiveWeaponSlot().GetComponentInChildren<WeaponScript>(); //Get the WeaponScript of Unactive weapon


        if(activeWeapon)
        {
            magazineAmmoUI.text = $"{activeWeapon.bulletsLeft / activeWeapon.bulletsPerBurst}";
            totalAmmoUI.text = $"{WeaponManager.Instance.CheckAmmoLeftFor(activeWeapon.thisWeaponModel)}";

            WeaponScript.WeaponModel model = activeWeapon.thisWeaponModel;
            ammoTypeUI.sprite = GetAmmoSprite(model);

            activeWeaponUI.sprite = GetWeaponSprite(model); 

            if (unActiveWeapon)
            {
                unActiveWeaponUI.sprite = GetWeaponSprite(unActiveWeapon.thisWeaponModel);
            }

        }
        else
        {
            magazineAmmoUI.text = "";
            totalAmmoUI.text = "";

            ammoTypeUI.sprite = emptySlot;
            unActiveWeaponUI.sprite = emptySlot;
        }


        if(WeaponManager.Instance.lethalsCount <= 0)
        {
            lethalUI.sprite = emptySlot;
            lethalAmountUI.text = "";
        }

        if(WeaponManager.Instance.tacticalsCount <= 0)
        {
            tacticalUI.sprite = emptySlot;
            tacticalAmountUI.text = "";
        }



    }


    private Sprite GetWeaponSprite(WeaponScript.WeaponModel model)
    {
        switch(model)
        {
            case WeaponScript.WeaponModel.AK74: 
                return Resources.Load<GameObject>("AK74_weapon").GetComponent<SpriteRenderer>().sprite;       
        
            case WeaponScript.WeaponModel.Bennelli_M4: 
                return Resources.Load<GameObject>("BennelliM4_weapon").GetComponent<SpriteRenderer>().sprite;       

            case WeaponScript.WeaponModel.M107: 
                return Resources.Load<GameObject>("M107_weapon").GetComponent<SpriteRenderer>().sprite;       

            case WeaponScript.WeaponModel.M1911: 
                return Resources.Load<GameObject>("M1911_weapon").GetComponent<SpriteRenderer>().sprite;       

            case WeaponScript.WeaponModel.M249: 
                return Resources.Load<GameObject>("M249_weapon").GetComponent<SpriteRenderer>().sprite;       

            case WeaponScript.WeaponModel.M416: 
                return Resources.Load<GameObject>("M416_weapon").GetComponent<SpriteRenderer>().sprite;    

            default: 
                return null;   
        }
    }


    private Sprite GetAmmoSprite(WeaponScript.WeaponModel model)
    {
        switch(model)
        {
            case WeaponScript.WeaponModel.AK74: 
                return Resources.Load<GameObject>("5.56_Ammo").GetComponent<SpriteRenderer>().sprite;       
    
            case WeaponScript.WeaponModel.Bennelli_M4: 
                return Resources.Load<GameObject>("12Gauge_Ammo").GetComponent<SpriteRenderer>().sprite;       

            case WeaponScript.WeaponModel.M107: 
                return Resources.Load<GameObject>("0.50BMG_Ammo").GetComponent<SpriteRenderer>().sprite;       

            case WeaponScript.WeaponModel.M1911: 
                return Resources.Load<GameObject>("0.45ACP_Ammo").GetComponent<SpriteRenderer>().sprite;       

            case WeaponScript.WeaponModel.M249: 
                return Resources.Load<GameObject>("5.56_Ammo").GetComponent<SpriteRenderer>().sprite;       

            case WeaponScript.WeaponModel.M416: 
                return Resources.Load<GameObject>("5.56_Ammo").GetComponent<SpriteRenderer>().sprite;    

            default: 
                return null;   
        }
    }




    private GameObject GetUnActiveWeaponSlot()
    {
        foreach(GameObject weaponSlot in WeaponManager.Instance.weaponSlots)
        {
            if(weaponSlot != WeaponManager.Instance.activeWeaponSlot)
            {
                return weaponSlot;
            }
        }

        //This will never happen, but we need to return something.C# requires you to return a GameObject in every possible path.
        return null;
    }



    public void ShowReloadUI()
    {
        reloadSpriteUI.enabled = true;
        reloadSpriteUI.fillAmount = 0;
    }


    public void UpdateReloadProgress(float progress)
    {
        reloadSpriteUI.fillAmount = progress;
    }


    public void HideReloadUI()
    {
        reloadSpriteUI.fillAmount = 0;
        reloadSpriteUI.enabled = false;
    }



    //Information Panel
    public void ShowItemInfo(Sprite icon, string name, string description)
    {
        itemInfoPanel.SetActive(true);

        itemIcon.sprite = icon;
        itemName.text = name;
        itemDescription.text = description;
    }

    public void HideItemInfo()
    {
        itemInfoPanel.SetActive(false);
    }


    internal void UpdateThrowablesUI()
    {
        lethalAmountUI.text = $"{WeaponManager.Instance.lethalsCount}";
        tacticalAmountUI.text = $"{WeaponManager.Instance.tacticalsCount}";

        switch(WeaponManager.Instance.equippedLethalType)
        {
            case ThrowableScript.ThrowableType.Grenade:
                lethalUI.sprite = Resources.Load<GameObject>("Grenade").GetComponent<SpriteRenderer>().sprite;    
                break;
        }

        switch(WeaponManager.Instance.equippedTacticalType)
        {
            case ThrowableScript.ThrowableType.SmokeGrenade:
                tacticalUI.sprite = Resources.Load<GameObject>("Smoke_Grenade").GetComponent<SpriteRenderer>().sprite;    
                break;
        }
    }


}
