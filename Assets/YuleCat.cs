using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CatCollisionHandler))]
public class YuleCat : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float walkStopRate = 0.05f;
    public DetectionZone attackZone;
    Damageable damageable;

    private Rigidbody2D rb;
    private Animator animator;
    private CatCollisionHandler catCollisionHandler;

    public enum WalkableDirection { Right, Left }
    private WalkableDirection _catDirection;
    private Vector2 walkDirectionVector;

    private bool _hasTarget = false;

    private WalkableDirection CatDirection
    {
        get { return _catDirection; }
        set
        {
            if (_catDirection != value)
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
            _catDirection = value;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        catCollisionHandler = GetComponent<CatCollisionHandler>();
        walkDirectionVector = Vector2.right;
        CatDirection = WalkableDirection.Right;  // Start walking right
        damageable = GetComponent<Damageable>();
    }

    private void FixedUpdate()
    {
        if (catCollisionHandler.isGrounded && catCollisionHandler.IsOnWall)
        {
            FlipDirection();
        }

        // Move the cat
        rb.velocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.velocity.y);

        // Always play walking animation if the cat is alive
        if (animator != null)
        {
            animator.SetBool("IsWalking", true);
        }
    }

    private void Update()
    {
        _hasTarget = attackZone.detectedColliders.Count > 0;

        if (_hasTarget)
        {
            animator.SetBool("HasTarget", true);
            animator.SetTrigger("Attack");  // Trigger attack animation if a target is detected
        }
    }

    private void FlipDirection()
    {
        if (CatDirection == WalkableDirection.Right)
        {
            CatDirection = WalkableDirection.Left;
        }
        else if (CatDirection == WalkableDirection.Left)
        {
            CatDirection = WalkableDirection.Right;
        }
    }

    // When the cat takes damage
    public void OnHit(int damage, Vector2 knockback)
    {
        if (animator)
        {
            animator.SetTrigger("Hit");  // Play hit animation

            rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
        }

        if (damageable)
        {
            if (damageable.Health <= 0)
            {
                animator.SetTrigger("Death");  // Play death animation
                rb.velocity = Vector2.zero;
            }
        }
    }
}
