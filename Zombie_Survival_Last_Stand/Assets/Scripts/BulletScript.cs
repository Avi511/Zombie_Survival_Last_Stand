using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public int bulletDamage;    //Value set from weaponScript (At the place where bullets are created)

    private void OnCollisionEnter(Collision objectWeHit)
    {
        if(objectWeHit.gameObject.CompareTag("Target"))
        {
            Debug.Log("Hit " + objectWeHit.gameObject.name + " !");

            CreateBulletImpactEffect(objectWeHit);

            Destroy(gameObject);
        }

        if(objectWeHit.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Hit " + objectWeHit.gameObject.name + " !");

            CreateBulletImpactEffect(objectWeHit);

            Destroy(gameObject);
        }

        if(objectWeHit.gameObject.CompareTag("Crate"))
        {
            Debug.Log("Hit " + objectWeHit.gameObject.name + " !");

            objectWeHit.gameObject.GetComponent<CrateScript>().Shatter();   //Calls the Shatter() from the CrateScript

            Destroy(gameObject);
        }

        if(objectWeHit.gameObject.CompareTag("Explosibles"))
        {
            Debug.Log("Hit " + objectWeHit.gameObject.name + " !");

            objectWeHit.gameObject.GetComponent<OilDrumScript>().Blast();   //Calls the Blast() from the OilDrumScript

            Destroy(gameObject);
        }

        if(objectWeHit.gameObject.CompareTag("Zombie"))
        {

            if(objectWeHit.gameObject.GetComponent<EnemyScript>().isDead == false)
            {
                objectWeHit.gameObject.GetComponent<EnemyScript>().TakeDamage(bulletDamage);  
            }

            CreateBloodSprayEffect(objectWeHit);

            Destroy(gameObject);
        }

        Destroy(gameObject,2f);
    }


    void CreateBulletImpactEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0]; //A collision may have several contact points. Here we only use the first one

        GameObject hole = Instantiate(GlobalReferences.Instance.bulletImpactEffectPrefab, contact.point, Quaternion.LookRotation(contact.normal));
        //contact.point -> ensures the effect appears exactly where the collision happened.
        //LookRotation() makes the prefab face the direction of the surface normal so the bullet hole sits flat against the wall instead of floating at an odd angle.
        //Quaternion.LookRotation(contact.normal) aligns the effect with the surface so it looks natural on walls, floors, and slopes.
        
        hole.transform.SetParent(objectWeHit.gameObject.transform);  //SetParent() makes the spawned effect move together with the object that was hit, which is especially important for moving targets.
        //SetParent() belongs to the Transform component.Not GameObject
    }


    private void CreateBloodSprayEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0]; 

        GameObject bloodEffect = Instantiate(GlobalReferences.Instance.bloodSprayEffect, contact.point, Quaternion.LookRotation(contact.normal));
        
        bloodEffect.transform.SetParent(objectWeHit.gameObject.transform);  
        
    }



}
