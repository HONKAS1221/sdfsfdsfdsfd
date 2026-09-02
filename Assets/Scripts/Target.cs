using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    //health of the enemy 
    public float health = 50f;

    public void TakeDamage(float amount) 
    {
        // Remove the damage of the weapon from health
        health -= amount;
        Debug.Log(health);

        // If enemy health is too low, it dies
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
