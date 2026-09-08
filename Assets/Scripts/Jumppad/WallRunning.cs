using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")] // header for wallrunning settings
    public LayerMask whatIsWall; // layer mask for what is considered a wall
    public LayerMask whatIsGround; // layer mask for what is considered a floor/ground
    public float wallRunForce = 10f; // force applied to the player when wallrunning
    public float maxWallRunTime = 3f; // maximum time the player can wallrun
    public float wallRunTimer;// timer for how long the player has been wallrunning

    [Header("Input")]
    private CharacterInput controls; // input controls for the player
    private float horizontalInput; // horizontal input from the player
    private float verticalInput; // vertical input from the player

    [Header("Detection")]
    public float wallCheckDistance = 0.8f; // distance to check for walls
    public float minJumpHeight = 1.2f; // minimum height the player must be above the ground to wallrun
    private RaycastHit leftWallHit; // raycast hit for the left wall to check if there a wall or not 
    private RaycastHit rightWallHit;
    private bool wallLeft; // is there a wall to the left? 
    private bool wallRight;


    [Header("References")]
    public Transform orientation; // reference to the player's orientation
    public PlayerController pm;
    private CharacterController controller; // reference to the character controller

    public bool isWallrunning; // is the player wallrunning? 

    private void Start() // Start is called before the first frame update
    {
        controller = GetComponent<CharacterController>(); // get the character controller component
        if (pm == null) pm = GetComponent<PlayerController>(); // get the character controller component
    }
    void Update() 
    {
        CheckForWall();
        StateMachine();

        if (isWallrunning) // if the player is wallrunning, call the wallrunning movement function
        {
            WallRunningMovement();
        }
    }

    private void CheckForWall() // check if there is a wall to the left or right of the player
    {
        Vector3 rightDir = orientation != null ? orientation.right : transform.right; // if the orientation is not null use the orientation's right vector otherwise use the transform's right vector

        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall); // RightWallhit Stores hit information if the raycast hits a wall to the right of the player
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall); // Start -> point -> direction -> distance -> layermask
    }

    private bool AboveGround() // check if the player is above the ground
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround); 
    }

    private void StateMachine()
    {
        // getting inputs
        if (pm != null)
        {
            horizontalInput = pm.move.x; // get the horizontal input from the player
            verticalInput = pm.move.y; // get the vertical input from the player
        }

        // State 1 - Wallrunning
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround()) // if there is a wall to the left or right, the player is moving forward, and the player is above the ground
        {
            if (!isWallrunning)
            {
                Startwallrun();
            }

           if (wallRunTimer > 0) // if the wallrun timer is greater than 0, decrement the wallrun timer
            {
                wallRunTimer -= Time.deltaTime;
            }
            if (wallRunTimer <= 0 && isWallrunning) 
            {
                StopWallRun();
            }
        }
        // state 2 - stopping wallrun
        else // if the player is not wallrunning, stop wallrunning
        {
            if (isWallrunning)
            {
                StopWallRun();
            }
        }

    }
    private void WallRunningMovement()
    {
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal; // if there is a wall to the right, use the right wall normal, otherwise use the left wall normal
        // vector3.cross is a function that takes in right and upwords dirction and returns the forward direction of the wall
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up); // get the forward direction of the wall by taking the cross product of the wall normal and the up vector

        if ((transform.forward - wallForward).magnitude > (transform.forward - -wallForward).magnitude) // if the player is facing away from the wall it useses the opposite direction of the wall forward
        {
            wallForward = -wallForward;
        }

        // move the player character controllercontroller along the wall 
        if (controller != null)
        { 
            controller.Move(wallForward * wallRunForce * Time.deltaTime); // move the player in the direction of the wall forward
        }
    }
    private void Startwallrun()
    {
        isWallrunning = true; // set wallrunning to true
        wallRunTimer = maxWallRunTime; // set the wallrun timer to the max wallrun time
    }

    private void StopWallRun()
    {
        isWallrunning = false; // set wallrunning to false
    }

}

