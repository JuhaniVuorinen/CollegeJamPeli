using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class Knight : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float walkStopRate = 0.05f;
    public DetectionZone attackZone;
    Damageable damageable;
    


    private Rigidbody2D rb;
    private TouchingDirections touchingDirections;
    private Animator animator;

    public enum WalkableDirection { Right, Left }

    private WalkableDirection _walkDirection;

    private Vector2 walkDirectionVector;

    private WalkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set
        {
            if (_walkDirection != value)
            {
                // Flip the scale to change direction
                gameObject.transform.localScale = new Vector2(
                    gameObject.transform.localScale.x * -1,
                    gameObject.transform.localScale.y
                );

                // Set the new walk direction vector
                if (value == WalkableDirection.Right)
                {
                    walkDirectionVector = Vector2.right;
                }
                else if (value == WalkableDirection.Left)
                {
                    walkDirectionVector = Vector2.left;
                }
            }

            _walkDirection = value;
        }
    }

    public bool _hasTarget = false;
    public bool HasTarget { get
        {
            return _hasTarget;
        }  private set
        {
            _hasTarget = value;
            animator.SetBool("HasTarget", value);
        } }

    public bool CanMove
    {
        get
        {
            return animator.GetBool("canMove");
        }
    }

    public bool IsHit
    {
        get
        {
            return animator.GetBool("IsHit");
        }
        set
        {
            animator.SetBool("IsHit", value);
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        WalkDirection = WalkableDirection.Right; // Initialize direction
        walkDirectionVector = Vector2.right; // Initialize the walk direction vector
        damageable = GetComponent<Damageable>();
    }

    private void FixedUpdate()
    {
        if (touchingDirections.isGrounded && touchingDirections.IsOnWall)
        {
            FlipDirection();
        }
        if (CanMove)
            rb.velocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.velocity.y);
        else
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkStopRate), rb.velocity.y);
    }

    private void Update()
    {
        HasTarget = attackZone.detectedColliders.Count > 0;
    }

    private void FlipDirection()
    {
        if (WalkDirection == WalkableDirection.Right)
        {
            WalkDirection = WalkableDirection.Left;
        }
        else if (WalkDirection == WalkableDirection.Left)
        {
            WalkDirection = WalkableDirection.Right;
        }
        else
        {
            Debug.LogError("Current Walkable Direction is not set");
        }
    }


    public void OnHit(int damage, Vector2 knockback)
    {
        IsHit = true;
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }
}
