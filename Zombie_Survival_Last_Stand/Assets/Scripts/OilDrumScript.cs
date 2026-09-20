using UnityEngine;
using System.Collections;

public class OilDrumScript : MonoBehaviour
{
    public GameObject bigExplosionEffectPrefab;
    public AudioClip blastClip;

    public void Blast() //Calls From BulletScript when bullet hits OilDrum
    {
        StartCoroutine(Explode());  //To give time gap for explode
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(0.25f);

        AudioManager.Instance.PlayBlastSound(blastClip);

        Instantiate(bigExplosionEffectPrefab, transform.position, transform.rotation);

        Destroy(gameObject);
    }   
}