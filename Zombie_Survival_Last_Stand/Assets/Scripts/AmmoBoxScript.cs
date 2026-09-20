using UnityEngine;

public class AmmoBoxScript : MonoBehaviour
{
    
    public int ammoAmount = 200;        //I can change this amount from inspector
    public AmmoType ammoType;

    public enum AmmoType
    {
        Ammo_5_56,
        Ammo_BMG,
        Ammo_ACP,
        Ammo_Gauge
    }

    [Header("Ammo Information")]
    public Sprite ammoIcon;
    public string ammoName;
    [TextArea]
    public string ammoDescription;


}
