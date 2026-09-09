using UnityEngine;
using UnityEngine.AI;

public class ZombieAttackState : StateMachineBehaviour
{
    private NavMeshAgent agent;
    private Transform player;

    public float stopAttackingDistance = 3.0f;

    override public void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        GameObject playerObj =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        agent = animator.GetComponent<NavMeshAgent>();

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    override public void OnStateUpdate(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        if (player == null)
            return;

        float distanceFromPlayer =
            Vector3.Distance(
                player.position,
                animator.transform.position);

        // Look at player
        Vector3 direction =
            player.position - animator.transform.position;

        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            animator.transform.rotation =
                Quaternion.LookRotation(direction);
        }

        // Stop attacking if player moves away
        if (distanceFromPlayer > stopAttackingDistance)
        {
            animator.SetBool("isAttacking", false);
            animator.SetBool("isChasing", true);
        }
    }

    override public void OnStateExit(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }
    }
}