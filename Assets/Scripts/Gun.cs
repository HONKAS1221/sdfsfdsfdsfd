using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    private CharacterInput controls;

    public float damage = 10f;
    public float range = 100f;

    public Camera fpsCam;

    void Awake()
    {
        controls = new CharacterInput();
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (controls.Player.Shoot.triggered)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit hit;

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {

            // We can visualise the ray by this, and it can be easier for you to debug

            Debug.DrawRay(fpsCam.transform.position, fpsCam.transform.forward * hit.distance, Color.red, 1f);
            Debug.Log(hit.transform.name);
            Target enemy = hit.transform.GetComponent<Target>();


            // If the target script exists, call the function in the target script to cause damage
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        // else show the lenth of the ray in the Colour red 
        else { Debug.DrawRay(fpsCam.transform.position, fpsCam.transform.forward * range, Color.red, 1f); }
    }



}
