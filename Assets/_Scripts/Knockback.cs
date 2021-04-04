using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    // Public variables
    public float thrust;
    public float knockTime;
    public float damage;

    // Collision with object
    private void OnTriggerEnter2D(Collider2D other)
    {
        // If other object is an enemy
        if (other.gameObject.CompareTag("enemy"))
        {
            Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
            // If rigidbody exists
            if (hit != null)
            {
                // Set direction
                Vector2 difference =  transform.position - hit.transform.position;
                difference = difference.normalized * thrust;
                // Add force
                hit.AddForce(difference, ForceMode2D.Impulse);
            
            }
        }
    }
    

}

