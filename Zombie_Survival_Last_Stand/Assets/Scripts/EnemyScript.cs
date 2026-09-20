using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private int HP = 100;

    private Animator animator;
    private CapsuleCollider capsuleCollider;
    private ZombieHand zombieHand;

    public bool isDead;

    private void Start()
    {
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        zombieHand = GetComponentInChildren<ZombieHand>();
        
    }

    public void TakeDamage(int damageAmount)
    {
        // Don't allow damage after death
        if (isDead)
            return;

        HP -= damageAmount;

        if (HP <= 0)
        {
            Die();
        }
        else
        {   
            animator.SetBool("isDead", false);
            animator.SetTrigger("DAMAGE");
            AudioManager.Instance.zombieChannel2.PlayOneShot(AudioManager.Instance.zombieHurt);
        }
    }

    private void Die()
    {
        isDead = true;

        // Tell Animator zombie is dead
        animator.SetBool("isDead", true);

        if (capsuleCollider != null)
        {
            capsuleCollider.enabled = false;
        }
        if (zombieHand != null)
        {
            zombieHand.gameObject.SetActive(false);
        }

        // Random death animation
        int randomValue = Random.Range(0, 3);

        if (randomValue == 0)
        {
            animator.SetTrigger("DIE1");
        }
        else if (randomValue == 1)
        {
            animator.SetTrigger("DIE2");
        }
        else
        {
            animator.SetTrigger("DIE3");
        }

        // Death sound
        AudioManager.Instance.zombieChannel2.PlayOneShot(AudioManager.Instance.zombieDeath);
    }
}