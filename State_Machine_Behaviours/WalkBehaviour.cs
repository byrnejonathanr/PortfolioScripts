using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkBehaviour : StateMachineBehaviour
{

    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float attackRange = 1.0f;
    public SpriteRenderer self;
    public Transform player;
    public Rigidbody2D rb;
    Vector2 target;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    /*override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }*/

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        target = Vector3.Normalize(new Vector2(player.position.x, rb.position.y) - rb.position);
        target = (Physics2D.gravity * rb.gravityScale) + (target * speed);
        rb.MovePosition(rb.position + target * Time.fixedDeltaTime);
        //target = new Vector2(player.position.x, rb.position.y);
        //newPosition = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        //rb.MovePosition(newPosition);
        self.flipX = player.position.x > rb.position.x;

        if (Vector2.Distance(player.position, rb.position) <= attackRange)
        {
            animator.SetTrigger("Attack");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");
    }

}
