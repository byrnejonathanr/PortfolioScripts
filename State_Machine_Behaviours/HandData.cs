using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandData : MonoBehaviour
{

    public Animator animator;
    public Transform origin;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    private PlayerInfo playerInfoScript;
    [SerializeField] private bool phase2;

    private void Start()
    {
        HandSeeker seekerBehaviour = animator.GetBehaviour<HandSeeker>();
        HandReturn returnBehaviour = animator.GetBehaviour<HandReturn>();
        HandSwipe swipeBehaviour = animator.GetBehaviour<HandSwipe>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerInfoScript = player.GetComponent<PlayerInfo>();
        animator.SetBool("Phase2", phase2);
        seekerBehaviour.player = player.transform;
        seekerBehaviour.rb = rb;
        seekerBehaviour.phase2 = phase2;
        returnBehaviour.origin = origin;
        returnBehaviour.rb = rb;
        swipeBehaviour.spriteRenderer = spriteRenderer;
        swipeBehaviour.rb = rb;
    }

    /*public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerInfoScript.TakeDamage();
        }
    }*/
}
