using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiePatrollingState : StateMachineBehaviour
{
    private float timer;

    public float patrollingTime = 10f;
    public float detectionArea = 18f;
    public float patrolSpeed = 2f;

    private Transform player;
    private NavMeshAgent agent;
    private float footstepTimer;
    private const float FootstepInterval = 0.5f;

    private List<Transform> waypointsList =
        new List<Transform>();

    override public void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        timer = 0f;
        footstepTimer = 0f;

        GameObject playerObj =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        agent = animator.GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            agent = animator.GetComponentInChildren<NavMeshAgent>();
        }

        if (agent == null)
        {
            agent = animator.GetComponentInParent<NavMeshAgent>();
        }

        if (agent == null || !agent.isOnNavMesh)
            return;

        agent.speed = patrolSpeed;
        agent.isStopped = false;

        GameObject waypointsCluster =
            GameObject.FindGameObjectWithTag("Waypoints");

        if (waypointsCluster == null)
        {
            animator.SetBool("isPatrolling", false);
            animator.SetBool("isChasing", true);
            return;
        }

        waypointsList.Clear();

        foreach (Transform t in waypointsCluster.transform)
        {
            waypointsList.Add(t);
        }

        if (waypointsList.Count == 0)
        {
            Debug.LogError("No waypoints found!");
            return;
        }

        MoveToRandomWaypoint();
    }

    override public void OnStateUpdate(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        if (agent == null || player == null)
            return;

        timer += Time.deltaTime;

        // Check player distance
        float distanceFromPlayer =
            Vector3.Distance(
                player.position,
                animator.transform.position);

        if (distanceFromPlayer < detectionArea)
        {
            animator.SetBool("isPatrolling", false);
            animator.SetBool("isChasing", true);
            return;
        }

        // Pick another waypoint
        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance)
        {
            MoveToRandomWaypoint();
        }

        PlayFootstepSound(animator);

        // Finish patrol
        if (timer >= patrollingTime)
        {
            animator.SetBool("isPatrolling", false);
        }
    }

    override public void OnStateExit(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    private void MoveToRandomWaypoint()
    {
        if (waypointsList.Count == 0)
            return;

        Transform waypoint =
            waypointsList[
                Random.Range(0, waypointsList.Count)
            ];

        agent.SetDestination(waypoint.position);
    }

    private void PlayFootstepSound(Animator animator)
    {
        if (agent == null || agent.isStopped || agent.velocity.sqrMagnitude < 0.01f)
            return;

        footstepTimer += Time.deltaTime;
        if (footstepTimer < FootstepInterval)
            return;

        footstepTimer = 0f;
        Zombie zombie = animator.GetComponentInParent<Zombie>();
        if (zombie != null)
        {
            zombie.PlayWalkingSound();
        }
    }
}