using UnityEngine;

public class GlobalReferences : MonoBehaviour
{
    //GlobalReferences acts as a central place to store shared prefabs and assets, so you only assign them once in the Inspector and every script can access them through GlobalReferences.Instance.
    public static GlobalReferences Instance{get; set;} //It creates a single global variable called instance....  Static -> every script in the project can access it.

    public GameObject bulletImpactEffectPrefab;

    public GameObject grenadeExplosionEffect;
    public GameObject smokeGrenadeEffect;

    public GameObject bloodSprayEffect;


    private void Awake()
    {
        if(Instance != null && Instance != this)        //Singleton
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}
