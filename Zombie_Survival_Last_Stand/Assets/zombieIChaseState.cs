using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieChaseState : StateMachineBehaviour
{
    NavMeshAgent agent;
    Transform player;

    public float chaseSpeed = 6f;
    public float stopChasingDistance = 21f;
    public float attackingDistance = 2.5f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // --- Initialization --- //
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<NavMeshAgent>();

        agent.speed = chaseSpeed;
        agent.isStopped = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
{
    agent.SetDestination(player.position);

    // Diagnostic readouts
    Debug.Log($"[NavCheck] Stopped: {agent.isStopped} | Speed: {agent.speed} | Path: {agent.pathStatus} | RemainingDist: {agent.remainingDistance}");

    float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);

    if (distanceFromPlayer <= attackingDistance)
    {
        animator.SetBool("isAttacking", true);
    }

    if (distanceFromPlayer > stopChasingDistance)
    {
        animator.SetBool("isChasing", false);
    }
}

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.SetDestination(animator.transform.position);
    }
}