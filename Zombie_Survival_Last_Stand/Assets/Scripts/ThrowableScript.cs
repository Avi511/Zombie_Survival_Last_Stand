using UnityEngine;

public class ThrowableScript : MonoBehaviour
{
    [SerializeField] float delay = 3f;
    [SerializeField] float damageRadius = 20f;
    [SerializeField] float explosionForce = 1200f;

    float countdown;

    bool hasExploded = false;
    public bool hasBeenThrown = false;

    public enum ThrowableType
    {
        None,
        Grenade,
        SmokeGrenade
    }
    public ThrowableType throwableType;


    [Header("Throwables Information")]
    public Sprite throwablesIcon;
    public string throwablesName;
    [TextArea]
    public string throwablesDescription;

    [Header("Audio Clips")]
    public AudioClip grenadeClipSound;
    public AudioClip grenadeBlastSound;
    public AudioClip smokeSound;

    bool clipSoundPlayed = false;





    public void Start()
    {
        countdown = delay;
    }


    public void Update()
    {
        if(hasBeenThrown)
        {
            // Play sound only once
            if (!clipSoundPlayed)
            {
                AudioManager.Instance.PlayClippingSound(grenadeClipSound);
                clipSoundPlayed = true;
            }

            countdown = countdown - Time.deltaTime;
            if(countdown <= 0f && !hasExploded)
            {
                Explode();
                hasExploded = true;
            }
        }
    }


    private void Explode()
    {
        GetThrowableEffect();
        Destroy(gameObject);
    }

    private void GetThrowableEffect()
    {
        switch (throwableType)
        {
            case ThrowableType.Grenade:
                GrenadeEffect();
                break;
            case ThrowableType.SmokeGrenade:
                SmokeGrenadeEffect();
                break;
        }
    }


    private void GrenadeEffect()
    {
        AudioManager.Instance.PlayBlastSound(grenadeBlastSound);

        //Visual Effect
        GameObject explosionEffect = GlobalReferences.Instance.grenadeExplosionEffect;
        Instantiate(explosionEffect, transform.position, transform.rotation);
    

        //Physical Effect
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach(Collider objectInRange in colliders)
        {
            Rigidbody rb = objectInRange.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, damageRadius);
            }
            //Apply damage to enemy
            if (objectInRange.gameObject.GetComponent<EnemyScript>())
            {
                objectInRange.gameObject.GetComponent<EnemyScript>().TakeDamage(100);
            }
        }

    }


    private void SmokeGrenadeEffect()
    {
        AudioManager.Instance.PlayBlastSound(smokeSound);

        //Visual Effect
        GameObject smokeEffect = GlobalReferences.Instance.smokeGrenadeEffect;
        Instantiate(smokeEffect, transform.position, transform.rotation);
    

        //Physical Effect
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach(Collider objectInRange in colliders)
        {
            Rigidbody rb = objectInRange.GetComponent<Rigidbody>();
            if(rb != null)
            {
                //Apply blindness to enemy
            }
        }
    }



}
