// Name: Michail (Xan) Mavromatis
// Date of Creation: 12/2/2025
// Description: Script responsible for base overworld player movement

using UnityEngine;
using UnityEngine.InputSystem;

public class MI_PlayerMovement : MonoBehaviour
{
    private enum Direction { Up, Down, Left, Right }

    [SerializeField]
    private Direction currentDirection;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator;
    public float moveSpeed; // Set this in editor

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void OnMove(InputValue value)
    {
        movement = value.Get<Vector2>();
        if (movement.y > 0)
        {
            currentDirection = Direction.Up;
        }
        else if (movement.y < 0)
        {
            currentDirection = Direction.Down;
        }
        else if (movement.x > 0)
        {
            currentDirection = Direction.Right;
        }
        else if (movement.x < 0)
        {
            currentDirection = Direction.Left;
        }
        animator.SetInteger("Direction", (int)currentDirection);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        animator.SetFloat("Speed", movement.sqrMagnitude);
    }
}
