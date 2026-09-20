using UnityEngine;
using UnityEngine.AI;

public class ZombieChaseState : StateMachineBehaviour
{
    NavMeshAgent agent;
    Transform player;

    public float chaseSpeed=6f;

    public float stopChasingDistance = 25f;
    public float attackingDistance = 5f;



    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<NavMeshAgent>();
        agent.speed = chaseSpeed;
    }


    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if(AudioManager.Instance.zombieChannel.isPlaying == false)
        {
            AudioManager.Instance.zombieChannel.PlayOneShot(AudioManager.Instance.zombieChase);
        }

        agent.SetDestination(player.position);
        animator.transform.LookAt(player);

        float distanceFromPlayer = Vector3.Distance(player.position,animator.transform.position);

        //Checking if the agent should stop chasing
        if(distanceFromPlayer > stopChasingDistance)
        {
            animator.SetBool("isChasing",false);
        }

        //Checking if the agent should attack
        if(distanceFromPlayer < attackingDistance)
        {
            animator.SetBool("isAttacking",true);
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
