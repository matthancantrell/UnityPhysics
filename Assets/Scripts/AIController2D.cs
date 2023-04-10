using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class AIController2D : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float speed;
    [SerializeField] float jumpHeight;
    [SerializeField] float doubleJumpHeight;
    [SerializeField, Range(1, 5)] float fallRateMultiplier;
    [SerializeField, Range(1, 5)] float lowJumpRateMultiplier;
    [Header("Ground")]
    [SerializeField] Transform groundTransform;
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] float groundRadius;
    [Header("AI")]
    [SerializeField] Transform[] waypoints;
    [SerializeField] float rayDistance = 1;

    Rigidbody2D rb;
    Vector2 velocity = Vector3.zero;
    bool faceRight = true;
    float groundAngle = 0;
    Transform targetWaypoint = null;

        enum State
    {
        IDLE, PATROL, ATTACK, CHASE
    }

    State currentState = State.IDLE;
    float stateTimer = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        Vector2 direction = Vector2.zero;

        switch(currentState)
        {
            case State.IDLE:
            {
                if(CanseePlayer()) currentState = State.CHASE;

                stateTimer += Time.deltaTime;
                if(stateTimer >= 2)
                {
                    SetNewWaypointTarget();
                    currentState = State.PATROL;
                }
                break;
            }
            case State.ATTACK:
            case State.CHASE:
            case State.PATROL:
            {
                if(CanseePlayer()) currentState = State.CHASE;

                direction.x = Mathf.Sign(targetWaypoint.position.x - transform.position.x);
                Physics2D.Raycast(transform.position, Vector2.right * direction.x * rayDistance);
                Debug.DrawRay(-transform.position, Vector2.right * direction.x * rayDistance);
                float dx = Mathf.Abs(targetWaypoint.position.x - transform.position.x);
                if(dx <= 0.25f)
                {
                    currentState = State.IDLE;
                    stateTimer = 0;
                }
                // if(transform.position.x < targetWaypoint.position.x)
                // {
                //     direction.x = 1;
                // }
                // else
                // {
                //     direction.x = -1;
                // }
                break;
            }
        }

        // check if the character is on the ground
        bool onGround = Physics2D.OverlapCircle(groundTransform.position, 0.02f, groundLayerMask) != null;
        // get direction input

        // set velocity
        velocity.x = direction.x * speed;

        if (onGround)
        {
            if (velocity.y < 0) velocity.y = 0;
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y += Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y);
                StartCoroutine(DoubleJump());
                animator.SetTrigger("Jump");
            }
        }
        // adjust gravity for jump
        float gravityMultiplier = 1;
        if (!onGround && velocity.y < 0) gravityMultiplier = fallRateMultiplier;
        if (!onGround && velocity.y > 0 && !Input.GetButton("Jump")) gravityMultiplier = lowJumpRateMultiplier; 
        velocity.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;

        // move character
        rb.velocity = velocity;

        // flip character
        if (velocity.x > 0 && !faceRight) Flip();
        if (velocity.x < 0 &&  faceRight) Flip();

        // rotate character to face direction of movement (velocity)

        // Update Animator
        animator.SetFloat("Speed", Mathf.Abs(velocity.x));

    }

    IEnumerator DoubleJump()
    {
        // wait a little after the jump to allow a double jump
        yield return new WaitForSeconds(0.01f);
        // allow a double jump while moving up
        while (velocity.y > 0)
        {
            // if "jump" pressed add jump velocity
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y += Mathf.Sqrt(doubleJumpHeight * -2 * Physics.gravity.y);
                break;
            }
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundTransform.position, groundRadius);
    }

    private void Flip()
    {
        faceRight = !faceRight;
        spriteRenderer.flipX = !faceRight;
    }

    private void SetNewWaypointTarget()
    {
        Transform waypoint = null;
        do
        {
            waypoint = waypoints[Random.Range(0, waypoints.Length)];
        } while(waypoint == targetWaypoint);
        targetWaypoint = waypoint;
    }

    private bool CanseePlayer()
    {
        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, ((faceRight) ? Vector2.right : Vector2.left) * rayDistance);
        Debug.DrawRay(transform.position, ((faceRight) ? Vector2.right : Vector2.left) * rayDistance);

        return raycastHit.collider != null && gameObject.CompareTag("Player");
    }
}