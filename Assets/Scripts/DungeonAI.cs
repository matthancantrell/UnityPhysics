using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class DungeonAI : MonoBehaviour
{
    [Header("Sprites & Animations")]
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    [Header("Movement")]
    [SerializeField] float speed;
    [SerializeField] float jumpHeight;
    [SerializeField, Range(1, 5)] float fallRateMultiplier;
    [Header("Ground")]
    [SerializeField] Transform groundTransform;
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] float groundRadius;
    [Header("AI")]
    [SerializeField] Transform[] waypoints;
    [SerializeField] float rayDistance = 1;
    [SerializeField] string enemyTag;
    [SerializeField] LayerMask raycastLayerMask;
    
    GameObject enemy;
    Transform enemyPos;

    Rigidbody2D rb;
    Vector2 velocity = Vector3.zero;
    bool faceRight = true;
    float groundAngle = 0;
    Transform targetWaypoint = null;
        enum State
    {
        IDLE, PATROL, ATTACK, CHASE
    }

    [SerializeField] State currentState = State.IDLE;
    float stateTimer = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        Vector2 direction = Vector2.zero;

        if(enemy.transform.position.y <= transform.position.y + 0.5f)
        {
            Debug.Log("Moving To Enemy!");
        }


        switch(currentState)
        {
            case State.IDLE:
            {
                if(CheckEnemySeen()) currentState = State.CHASE; // If Enemy Is At Or Below Current Y-Level, Chase Them

                stateTimer -= Time.deltaTime; // Decrement The State Timer
                if(stateTimer <= 0) // If At Or Below 0
                {
                    SetNewWaypointTarget(); // Find A New Target Waypoint
                    currentState = State.PATROL; // Switch To Patrol
                }
                break;
            }

            case State.PATROL:
            {
                if(CheckEnemySeen()) currentState = State.CHASE; // If Enemy Is At Or Below Current Y-Level, Chase Them

                direction.x = Mathf.Sign(targetWaypoint.position.x - transform.position.x); // Walk Towards The Waypoint
                float dx = Mathf.Abs(targetWaypoint.position.x - transform.position.x); // Calculate Distance From Target

                if(dx <= 0.25f) // If Close
                {
                    currentState = State.IDLE; // Switch To Idle
                    stateTimer = 1; // Reset State Timer
                }
                break;
            }

            case State.CHASE:
            {
                if(!CheckEnemySeen())
                {
                    currentState = State.IDLE; // Switch To Idle
                    stateTimer = 1; // Reset State Timer
                    break;
                }
                float dx = Mathf.Abs(enemy.transform.position.x - transform.position.x); // Calculate Distance From Enemy
                if(dx <= 1f) // If Close
                {
                    currentState = State.ATTACK; // Switch To Attack
                    animator.SetTrigger("Attack"); // Trigger Attack Animation
                }else // If Not Close
                {
                    direction.x = Mathf.Sign(enemy.transform.position.x - transform.position.x); // Move Towards Enemy
                }
                break;
            }

            case State.ATTACK:
            {
                if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0)) // If Not Attacking
                {
                    currentState = State.CHASE; // Switch To Chase
                }
                break;
            }
        }

        // check if the character is on the ground
        bool onGround = Physics2D.OverlapCircle(groundTransform.position, 0.02f, groundLayerMask) != null;

        // set velocity
        velocity.x = direction.x * speed;

        if (onGround)
        {
            if (velocity.y < 0) velocity.y = 0;
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y += Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y);
                animator.SetTrigger("Jump");
            }
        }
        // adjust gravity for jump
        float gravityMultiplier = 1;
        if (!onGround && velocity.y < 0) gravityMultiplier = fallRateMultiplier;
        velocity.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;

        // move character
        rb.velocity = velocity;

        // flip character
        if (velocity.x > 0 && !faceRight) Flip();
        if (velocity.x < 0 &&  faceRight) Flip();

        // Update Animator
        animator.SetFloat("Speed", Mathf.Abs(velocity.x));
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

    private bool CheckEnemySeen()
    {
        if(enemy.transform.position.y <= transform.position.y + 2f) return true;
        return false;
    }
}