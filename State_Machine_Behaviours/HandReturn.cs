using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandReturn : StateMachineBehaviour
{

    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float holdRange = 0.2f;
    public Transform origin;
    public Rigidbody2D rb;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
        //rb = animator.GetComponent<Rigidbody2D>();
        //origin = animator.GetComponent<HandData>().origin;
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector2 target = new Vector2(origin.position.x, origin.position.y);
        Vector2 newPosition = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);

        if (Vector2.Distance(origin.position, rb.position) <= holdRange)
        {
            animator.SetTrigger("Hold");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Hold");
    }

}
