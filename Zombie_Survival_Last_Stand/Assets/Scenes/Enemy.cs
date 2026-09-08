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
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        HP -= damageAmount;

        if (HP <= 0)
        {
            isDead = true;
            if (navMeshAgent != null)
            {
                navMeshAgent.isStopped = true;
                navMeshAgent.enabled = false; // Prevents agent from snapping corpse
            }

            if (animator != null)
            {
                animator.SetTrigger("DIE1");
            }
        }
        else
        {
            // Only trigger damage if still alive
            if (animator != null)
            {
                animator.SetTrigger("DAMAGE");
            }
        }
    }

    public void Update()
    {
        if (isDead || navMeshAgent == null || animator == null) return;

        // Matches 'Iswalking' from your Animator Parameters tab
        bool moving = navMeshAgent.velocity.magnitude > 0.1f;
        animator.SetBool("Iswalking", moving);
    }   
}