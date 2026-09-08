using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    [SerializeField] private int HP = 100;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;

        if (HP <= 0)
        {
            if (animator != null)
            {
                animator.SetTrigger("DIE");
            }

            Destroy(gameObject, 0.1f);
            return;
        }

        if (animator != null)
        {
            animator.SetTrigger("DAMAGE");
        }
    }
}