using UnityEngine;

public class ZombieIdleState : StateMachineBehaviour
{
    private float timer;

    public float idleTime = 0f;
    public float detectionAreaRadius = 18f;

    private Transform player;

    override public void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        timer = 0f;

        GameObject playerObj =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (animator == null)
        {
            return;
        }
    }

    override public void OnStateUpdate(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        timer += Time.deltaTime;

        // Idle -> Patrol
        if (timer >= idleTime)
        {
            animator.SetBool("isPatrolling", true);
        }

        if (player == null)
            return;

        float distanceFromPlayer =
            Vector3.Distance(
                player.position,
                animator.transform.position);

        // Idle -> Chase
        if (distanceFromPlayer <= detectionAreaRadius)
        {
            animator.SetBool("isChasing", true);
            animator.SetBool("isPatrolling", false);
        }
    }
}