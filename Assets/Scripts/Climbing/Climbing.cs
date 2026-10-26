using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Rigidbody rb;
    public PlayerController pm;
    public LayerMask whatIsWall;

    [Header("Climbing")]
    public float climbingSpeed;
    public float maxClimbingSpeed;
    public float climbTimer;

    private bool climbing;

    [Header("Detection")]
    public float detectionLength;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    private float wallLookAngle;


    private RaycastHit frontWallHit;
    private bool wallFront;
    
    private void Update()
    {
        WallCheck();
        StateMachine();

        if (climbing) ClimbingMovement(); 
    }

    private void StateMachine()
    {
        // Climbing
        if (wallFront && Input.GetKey(KeyCode.W) && wallLookAngle < maxWallLookAngle)
        {
            if (!climbing &&  climbTimer > 0) StartClimbing();
            
            // Climbing Timer
            if (climbTimer > 0) climbTimer -= Time.deltaTime;
            if (climbTimer < 0) StopClimbing();
        }
        else
        {
            // stops the Climbing 
            if (climbing) StopClimbing();
        }
    }       
    private void WallCheck() // Wall dectection using spherecast to check if there is a wall in front of the player
    {
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLength, whatIsWall);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);

        if (pm.grounded)
        { 
            climbTimer = maxClimbingTime;
        }
    }

    private void StartClimbing()
    {
        climbing = true;

        // Camdra fov changing for a speedin look
    }

    private void ClimbingMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, climbingSpeed, rb.velocity.z);

        // sound effects for climbing ** later

    }

    private void StopClimbing()
    {
        climbing = false;

        // particle effects for climbing
    }
}

