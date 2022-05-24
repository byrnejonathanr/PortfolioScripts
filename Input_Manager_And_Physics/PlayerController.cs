using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody playerRB;
    [SerializeField] private float speed;
    [SerializeField] private Transform throwTracker;
    [SerializeField] private GameObject heldBall;
    [SerializeField] private GameObject dummyBall;
    private PlayerAction playerAction;
    private Vector2 movementVector;

    private void Awake()
    {
        playerAction = new PlayerAction();
        if (this.gameObject.tag == "Player1")
        {
            playerAction.Player1.Enable();
            playerAction.Player1.Throw.performed += ThrowBall;
            playerAction.Player1.Movement.performed += Movement;
        }
        else
        {
            playerAction.Player2.Enable();
            playerAction.Player2.Throw.performed += ThrowBall;
            playerAction.Player2.Movement.performed += Movement;

        }
    }
    private void FixedUpdate()
    {
        playerRB.MovePosition(playerRB.position + new Vector3(movementVector.x, 0.0f, movementVector.y) * speed * Time.fixedDeltaTime);
        //playerRB.AddForce(new Vector3(movementVector.x, 0.0f, movementVector.y) * speed, ForceMode.Force);
    }

    public void Movement(InputAction.CallbackContext context)
    {
        movementVector = context.ReadValue<Vector2>();
    }

    public void ThrowBall(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ThrowBall();
        }
    }

    public void HoldBall(GameObject ball)
    {
        if (heldBall == null)
        {
            heldBall = ball;
            heldBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
            heldBall.SetActive(false);
            dummyBall.SetActive(true);
        }
    }

    public void ThrowBall()
    {
        if (heldBall != null)
        {
            dummyBall.SetActive(false);
            heldBall.SetActive(true);
            heldBall.transform.position = throwTracker.position;
            heldBall.GetComponent<Rigidbody>().AddForce(throwTracker.forward * 15, ForceMode.Impulse);
            heldBall = null;
        }
    }
}
