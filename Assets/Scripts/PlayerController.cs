using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float airWalkSpeed = 3f;

    public float maxJumpImpulse = 10f;
    public float jumpHoldTime = 0.2f; // Max duration to hold the jump

    private float jumpTimer;
    private bool isJumping;

    Vector2 moveInput;
    TouchingDirections touchingDirections;

    Damageable damageable;

    public float CurrentMoveSpeed{ get

        {
            if(CanMove)
            {
                if (IsMoving && !touchingDirections.IsOnWall)
                {
                    if (touchingDirections.isGrounded)
                    {
                        if (IsRunning)
                        {
                            return runSpeed;
                        }
                        else
                        {
                            return walkSpeed;
                        }
                    }
                    else
                    {
                        return airWalkSpeed;
                    }
                }
                else
                {
                    return 0;
                }
            } else { 
                return 0; 
            }
            
        }
    }

    private bool _isMoving = false;

    public bool IsMoving
    {
        get { return _isMoving; }
        private set
        {
            _isMoving = value;
            animator.SetBool("IsMoving", value);
        }
    }

    private bool _isRunning = false;

    public bool IsRunning
    {
        get { return _isRunning; }
        set
        {
            _isRunning = value;
            animator.SetBool("IsRunning", value);
        }
    }

    public bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        private set
        {
            if (_isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }
            _isFacingRight = value;
        }
    }

    public bool CanMove { get
    {
            return animator.GetBool("canMove");
    } }

    public bool IsAlive
    {
        get
        {
            return animator.GetBool("IsAlive");
        }
    }

    public bool IsHit { get 
        {
            return animator.GetBool("IsHit");
        } set 
        {
            animator.SetBool("IsHit", value);
        } 
    }


    Rigidbody2D rb;
    Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();
    }

    private void FixedUpdate()
    {
        if (!damageable.IsHit)
            rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);
        animator.SetFloat("yVelocity", rb.velocity.y);

        // Handle jump holding
        if (isJumping)
        {
            jumpTimer += Time.fixedDeltaTime;
            if (jumpTimer >= jumpHoldTime)
            {
                StopJump();
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        moveInput = new Vector2(input.x, 0); // Ignore vertical input (y-component)

        if (IsAlive)
        {
            IsMoving = moveInput != Vector2.zero;

            SetFacingDirection(moveInput);
        }else
        {
            IsMoving = false;
        }
        
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        }
        else if (moveInput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsRunning = true;
        }
        else if (context.canceled)
        {
            IsRunning = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirections.isGrounded && CanMove)
        {
            StartJump();
        }
        else if (context.canceled)
        {
            StopJump();
        }
    }

    private void StartJump()
    {
        animator.SetTrigger("jump");
        rb.velocity = new Vector2(rb.velocity.x, maxJumpImpulse);
        isJumping = true;
        jumpTimer = 0;
    }

    private void StopJump()
    {
        isJumping = false;
        if (rb.velocity.y > 0) // Allow canceling only if moving upward
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f); // Dampen upward velocity
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started) {

            animator.SetTrigger("attack");
        
        }
    }


    public void OnHit(int damage, Vector2 knockback)
    {
        IsHit = true;
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }
}
