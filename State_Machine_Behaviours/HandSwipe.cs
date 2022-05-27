using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandSwipe : StateMachineBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    rb.AddForce((spriteRenderer.flipX ? Vector2.left : Vector2.right) * 10.0f, ForceMode2D.Impulse);
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector2 target = rb.position + (spriteRenderer.flipX ? Vector2.right : Vector2.left);
        Vector2 newPosition = Vector2.MoveTowards(rb.position, target, 4.0f * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
}
