using UnityEngine;
using TMPro;

public class AmmoManager : MonoBehaviour
{
    public static AmmoManager Instance { get; set; }

    public TextMeshProUGUI ammoText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
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
        if (WeaponManager.Instance == null)
        {
            return;
        }

        GameObject activeWeaponSlot = WeaponManager.Instance.activeWeaponSlot;

        if (activeWeaponSlot == null)
        {
            return;
        }

        Weapon activeWeapon = activeWeaponSlot.GetComponentInChildren<Weapon>();

        if (activeWeapon != null)
        {
            ammoText.text = activeWeapon.bulletsLeft + " / " + activeWeapon.totalAmmo;
        }
        else
        {
            ammoText.text = "";
        }
    }
}