using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    [SerializeField] private int HP = 100;
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private bool isDead = false;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (navMeshAgent == null)
        {
            navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        }

        if (navMeshAgent == null)
        {
            Debug.LogWarning("Zombie has no NavMeshAgent on the root or children.");
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        HP -= damageAmount;

        if (HP <= 0)
        {
            Die();
            return;
        }

        if (animator != null)
        {
            animator.SetTrigger("DAMAGE");
        }

        PlayZombieSound(SoundManager.Instance != null ? SoundManager.Instance.zombieHurt : null);
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        StopChaseSound();

        PlayZombieSound(SoundManager.Instance != null ? SoundManager.Instance.zombieDeath : null);

        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
            navMeshAgent.enabled = false;
        }

        if (animator != null)
        {
            animator.SetTrigger("DIE1");
        }

        Destroy(gameObject, 2.5f);
    }

    public void PlayAttackSound()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayZombieSound(SoundManager.Instance.zombieAttack);
        }
    }

    public void PlayChaseSound()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StartChaseSound();
        }
    }

    public void StopChaseSound()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopChaseSound();
        }
    }

    public void PlayWalkingSound()
    {
        if (SoundManager.Instance != null)
        {
            PlayZombieSound(SoundManager.Instance.zombieWalking);
        }
    }

    private void PlayZombieSound(AudioClip clip)
    {
        if (clip == null) return;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayZombieSound(clip);
        }
        else
        {
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }
    }

    public void OnDrawGizmos(){
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 28f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 18f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 2.5f);
    }

      
}