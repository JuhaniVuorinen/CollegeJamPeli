using UnityEngine;

public class CatCollisionHandler : MonoBehaviour
{
    public bool isGrounded = false;
    public bool IsOnWall = false;

    private Collider2D groundCheckCollider;
    private Collider2D wallCheckCollider;

    private void Awake()
    {
        groundCheckCollider = GameObject.FindGameObjectWithTag("Ground").GetComponent<Collider2D>();
        wallCheckCollider = GameObject.FindGameObjectWithTag("Wall").GetComponent<Collider2D>();
    }

    private void Update()
    {
        CheckGroundCollision();
        CheckWallCollision();
    }

    private void CheckGroundCollision()
    {
        isGrounded = groundCheckCollider.bounds.Contains(transform.position);
    }

    private void CheckWallCollision()
    {
        IsOnWall = wallCheckCollider.bounds.Contains(transform.position);
    }
}
