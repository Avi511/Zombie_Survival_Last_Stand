using UnityEngine;
using UnityEngine.AI;

public class ZombieChaseState : StateMachineBehaviour
{
    private NavMeshAgent agent;
    private Transform player;

    public float chaseSpeed = 6f;
    public float stopChasingDistance = 28f;
    public float attackingDistance = 2.5f;

    override public void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        agent = animator.GetComponent<NavMeshAgent>();

        if (agent != null && agent.isOnNavMesh)
        {
            agent.speed = chaseSpeed;
            agent.isStopped = false;
        }

        animator.SetBool("isAttacking", false);
    }

    override public void OnStateUpdate(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        if (agent == null || player == null)
            return;

        if (!agent.isOnNavMesh)
            return;

        float distanceFromPlayer =
            Vector3.Distance(player.position, animator.transform.position);

        // Stop chasing if player is too far
        if (distanceFromPlayer > stopChasingDistance)
        {
            animator.SetBool("isChasing", false);
            return;
        }

        // Start attacking when close enough
        if (distanceFromPlayer <= attackingDistance)
{
    Debug.Log("ZOMBIE SHOULD ATTACK! Distance = " + distanceFromPlayer);

    agent.isStopped = true;
    agent.ResetPath();

    animator.SetBool("isAttacking", true);

    return;
}

        // Continue chasing
        animator.SetBool("isAttacking", false);

        agent.isStopped = false;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(
            player.position,
            out hit,
            3f,
            NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
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