using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem; // Added for the new Input System package

public class PickUpController : MonoBehaviour
{
    public Gun gun; // the gun script that is attached to the gun
    public Rigidbody rb; // the rigidbody used for the gun
    public Collider coll; // the hitbox/clolider of the gun
    public Transform player, gunContainer, fpsCam;   // Player, gunContainer, fpsCam

    public float pickUpRange; // The range at which the player can pick up the gun
    public float dropForwardForce, dropUpwardForce; // Used for force of the pick up

    public bool equipped; // Check if the gun is equipped
    public static bool slotFull; // Check if the player has a gun in their hand (static means it will be used every single pickup scripted)

    private void Start()
    {
        // set up for before dropping the gun, so the gun is not equipped and the slot is not full
        // If the gun is not equipped, enable the gun script
        if (!equipped)
        {
            gun.enabled = false;
            rb.isKinematic = false;
            coll.isTrigger = false;
        }
        // If the gun is equipped, disable the gun script
        if (equipped)
        {
            gun.enabled = true;
            rb.isKinematic = true;
            coll.isTrigger = true;
        }
    }

    private void Update()
    {
        // Ensure a keyboard is connected before reading key inputs
        if (Keyboard.current == null) return;

        Vector3 distanceToPlayer = player.position - transform.position; // Get the distance between the player and the gun

        // If the gun is not equipped, the player is in range, and the player presses E
        if (!equipped && distanceToPlayer.magnitude <= pickUpRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Pickup(); // Call the pickup function
        }

        // Drop if the gun is equipped and the player presses Q
        if (equipped && Keyboard.current.qKey.wasPressedThisFrame)
        {
            Drop(); // If the gun is equipped and the player presses Q
        }
    }

    private void Pickup()
    {
        equipped = true;
        slotFull = true;
       
        // Make the rigidbody kinematic and the collider
        rb.isKinematic = true;
        // make the collider a trigger 
        coll.isTrigger = true;

        //make weapon a child of the camera and move it to the default position
        transform.SetParent(gunContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;

        

        //enable the gun script
        gun.enabled = true;
    }

    private void Drop()
    {
        equipped = false;
        slotFull = false;

        // Set the parent to null, so the gun is no longer a child of the camera
        transform.SetParent(null);

        // Make the rigidbody kinematic and disable the collider
        rb.isKinematic = false;
        // disables the collider a trigger 
        coll.isTrigger = false;

        // gun momentum when dropped 
        rb.velocity = player.GetComponent<Rigidbody>().velocity; // makes the gun fly forward when dropped due to player velocity

        // Add force to the gun when dropped
        rb.AddForce(fpsCam.forward * dropForwardForce, ForceMode.Impulse); // adds forward force to the gun when dropped
        rb.AddForce(fpsCam.up * dropUpwardForce, ForceMode.Impulse); // adds upward force to the gun when dropped

        // adds sick random rotation when dropped 
        float random = Random.Range(-1f, 1f); // random number between -1 of 1
        rb.AddTorque(new Vector3(random, random, random) * 10); // adds random torque to the gun when dropped

        //disables the gun script
        gun.enabled = false;
    }
}