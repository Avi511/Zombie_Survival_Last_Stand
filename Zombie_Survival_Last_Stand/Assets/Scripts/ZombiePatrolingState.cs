using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiePatrolingState : StateMachineBehaviour
{
    float timer;
    public float patrolingTime = 10f;

    Transform player;
    NavMeshAgent agent;

    public float detectionAreaRadius = 20f;
    public float patrolSpeed = 2f;

    List<Transform> waypointsList = new List<Transform>();


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;

        timer = 0f;

        //Get all waypoints and Move to first WayPoint
        GameObject waypointCluster = GameObject.FindGameObjectWithTag("WayPoints");
        foreach(Transform t in waypointCluster.transform)
        {
            waypointsList.Add(t);
        }

        Vector3 nextPosition = waypointsList[Random.Range(0,waypointsList.Count)].position;
        agent.SetDestination(nextPosition);
    }



    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if(AudioManager.Instance.zombieChannel.isPlaying == false)
        {
            AudioManager.Instance.zombieChannel.PlayOneShot(AudioManager.Instance.zombieWalking);
        }

        if(agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.SetDestination(waypointsList[Random.Range(0,waypointsList.Count)].position);
        }


        //Transition to idle State
        timer = timer + Time.deltaTime;
        if(patrolingTime <= timer)
        {
            animator.SetBool("isPatroling",false);
        }

        //Transition to Chase State
        float distanceFromPlayer = Vector3.Distance(player.position,animator.transform.position);
        if(distanceFromPlayer < detectionAreaRadius)
        {
            animator.SetBool("isChasing",true);
        }
    }



    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Stop the agent
        agent.SetDestination(agent.transform.position);
        AudioManager.Instance.zombieChannel.Stop();
    }

}
