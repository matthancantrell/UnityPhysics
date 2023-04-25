using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class AIController2D1 : MonoBehaviour
{
    [Header("Sprites & Animations")]
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    [Header("Movement")]
    [SerializeField] float speed;
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
        enemy = FindObjectOfType<ControllerCharacter2D>().gameObject; // Enemies are aware of the player's position at all times
    }
    void Update()
    {
        //CheckEnemySeen(); // Sets 'enemy' If Raycast Detects Viable Target
        if(enemy != null) Debug.Log(enemy);
        Vector2 direction = Vector2.zero;

        switch(currentState)
        {
            case State.IDLE:
            {
                if(enemy.transform.position.y <= transform.position.y) currentState = State.CHASE; // If An Enemy Is Specified / Seen, Switch To Chase

                stateTimer -= Time.deltaTime; // Decrement The State Timer
                if(stateTimer <= 0) // If At Or Below 0
                {
                    SetNewWaypointTarget(); // Find A New Target Waypoint
                    currentState = State.PATROL; // Switch To Patrol
                }
                break;
            }
            case State.ATTACK:
            {
            //     if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0)) // If Not Attacking
            //     {
            //         currentState = State.CHASE; // Switch To Chase
            //     }
                break;
            }
            case State.CHASE:
            {
                if(enemy.transform.position.y > transform.position.y) // If Enemy Is Too High Up
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
                    Debug.Log("Attack!"); // Let's Me Know That This Is Being Triggered
                }else // If Not Close
                {
                    direction.x = Mathf.Sign(enemy.transform.position.x - transform.position.x); // Move Towards Enemy
                }
                break;
            }
            case State.PATROL:
            {
                if(enemy.transform.position.y <= transform.position.y) currentState = State.CHASE; // If An Enemy Is On Or Below Curreny Y-Level, Chase After Them

                direction.x = Mathf.Sign(targetWaypoint.position.x - transform.position.x); // Walk Towards The Waypoint
                // Physics2D.Raycast(transform.position, Vector2.right * direction.x * rayDistance);  // Send Out A Raycast To Detect An Enemy In Sight
                // Debug.DrawRay(transform.position, Vector2.right * direction.x * rayDistance); // Draw The Raycast For Debugging Purposes
                float dx = Mathf.Abs(targetWaypoint.position.x - transform.position.x); // Calculate Distance From Target

                if(dx <= 0.25f) // If Close
                {
                    currentState = State.IDLE; // Switch To Idle
                    stateTimer = 1; // Reset State Timer
                }
                break;
            }
        }

        // check if the character is on the ground
        bool onGround = Physics2D.OverlapCircle(groundTransform.position, 0.02f, groundLayerMask) != null;

        // set velocity
        velocity.x = direction.x * speed;

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

    // private void CheckEnemySeen()
    // {
    //     enemy = null;
	// 	RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, ((faceRight) ? Vector2.right : Vector2.left), rayDistance, raycastLayerMask);
	// 	if (raycastHit.collider != null && raycastHit.collider.gameObject.CompareTag(enemyTag))
	// 	{
	// 		enemy = raycastHit.collider.gameObject;
	// 		Debug.DrawRay(transform.position, ((faceRight) ? Vector2.right : Vector2.left) * rayDistance, Color.red);
	// 	}
    // }
}