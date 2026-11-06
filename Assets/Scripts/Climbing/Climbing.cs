    using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Climbing : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Rigidbody rb;
    public PlayerController pm;
    public LayerMask whatIsWall;

    [Header("Climbing")]
    public float climbSpeed;
    public float maxClimbTime;
    private float climbTimer;

    private bool climbing;

    [Header("ClimbJumping")]
    public float climbJumpUpForce;
    public float climbJumpBackForce;

    public KeyCode jumpKey = KeyCode.Space;
    public int climbJumps;
    public int climbJumpsLeft;

    [Header("Detection")]
    public float detectionLength;
    public float sphereCastRadius;
    public float maxWallLookAngle; 
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool wallFront;
    private Transform lastWall;
    private Vector3 lastWallNormal;
    public float minWallNormalAngleChange;

    public bool isClimbing => climbing;

    [Header("Exiting")]
    public float exitWallTime;
    private float exitWallTimer;

    private void Update()
    {
        WallCheck();
        StateMachine();

        if (climbing) ClimbingMovement();
    }

    private void StateMachine()
    {

        bool wPressed = Keyboard.current != null && Keyboard.current.wKey.isPressed;
        bool spacePressed = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;

        if (exitWallTimer > 0)
        { 
            exitWallTimer -= Time.deltaTime;
            if (climbing) StopClimbing();
            return;
        }

        // sue wPressed instead of Input.GetKey(KeyCode.W)
        if (wallFront && wPressed && wallLookAngle < maxWallLookAngle)
        {
            if (!climbing && climbTimer > 0) StartClimbing();

            if (climbTimer > 0) climbTimer -= Time.deltaTime;
            if (climbTimer <= 0) StopClimbing();
        }
        // none
        else
        {
            if (climbing) StopClimbing();
        }

        if (wallFront && spacePressed && climbJumpsLeft > 0) ClimbJump();

    }
    // wall checking working
    private void WallCheck()
    {
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLength, whatIsWall);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);

        bool NewWall = frontWallHit.transform != lastWall || Mathf.Abs(Vector3.Angle(lastWallNormal, frontWallHit.normal)) > minWallNormalAngleChange;
       
        if ((wallFront && NewWall) || pm.isGrounded())
        {
            
            climbTimer = maxClimbTime;
            climbJumpsLeft = climbJumps;
        }

        // Debugging
        Color rayColor = (wallFront && wallLookAngle < maxWallLookAngle) ? Color.green : Color.red;
        Debug.DrawRay(transform.position, orientation.forward * detectionLength, rayColor);

        if (wallFront)
        {
            Debug.Log($"Wall Hit: {frontWallHit.collider.name} | Angle: {wallLookAngle}°");
        }
    }


    // debugging adds an ShpereCast gizmo to the scene view to visualize the wall detection 
    private void OnDrawGizmosSelected()
    {
        if (orientation == null) return;

        Gizmos.color = wallFront ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position + orientation.forward * detectionLength, sphereCastRadius);
    }

private void StartClimbing()
    {
        climbing = true;

        lastWall = frontWallHit.transform;
        lastWallNormal = frontWallHit.normal;
        
        // camera fov
    }

    private void ClimbingMovement()
    {   
        rb.useGravity = false;
        transform.position += Vector3.up * climbSpeed * Time.deltaTime;

        //sound effect
    }

    private void StopClimbing()
    {
        rb.useGravity = true;
        climbing = false;

        //particle effect 
    }

    private void ClimbJump()
    {
        StopClimbing();
        exitWallTimer = exitWallTime;
        Vector3 launch = Vector3.up * climbJumpUpForce + frontWallHit.normal * climbJumpBackForce;
        pm.LaunchFromWall(launch);
        climbJumpsLeft--;
    }
}

