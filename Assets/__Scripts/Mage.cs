using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : Enemy
{   
    //GameObject boss instance
    public GameObject mage;
    // Animator
    public Animator anim;
    // Movement variables
    private float input_x;
    private float input_y;
    // Private state booleans
    private bool isMoving;
    private bool isLaunching;
    private bool isFrozen;
    // Projectile 
    public GameObject projectile;
    
    //Start method
    new void Start(){
        // Superclass start method call
        base.Start();
        // Start mage health at 6
        base.setHealth(6); 
        // Set state variables
        isMoving = true;
        isLaunching = false;
        isFrozen = false;
        // Set animator
        anim = gameObject.GetComponent<Animator>();
        // Start walk controller
        StartCoroutine(walkController());
    }

    // Overriden Update method
    public override void Update()
    {
        // Ensure object is not affected by gravity
        rbe.isKinematic = true;
        // If movement is enabled
        if (isMoving && knockback == false){
            // Move toward the player
            Vector3 direction = playerTransform.position - transform.position;
            // Left 
            if(direction.x < -0.4f && System.Math.Abs(direction.y) < 0.4f) {input_x = -1f; input_y = 0f;}
            // Right
            else if(direction.x > 0.4f && System.Math.Abs(direction.y) < 0.4f) {input_x = 1f; input_y = 0f;}
            // Up
            else if(direction.y > 0.4f && System.Math.Abs(direction.x) < 0.4f){input_x = 0f; input_y =-1f;}
            // Down
            else {input_x = 0f; input_y = 1f;}
            direction.Normalize();
            // Set movement
            movement = direction;
            // Set the sprite direction
            anim.SetFloat("x", input_x);
            anim.SetFloat("y", input_y);
            // Set rotation
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,lockPos,lockPos);
        } else if (isLaunching == false){
            // If the mage isn't currently launching fireballs, stop movement and begin launching fireballs
            rbe.velocity = new Vector2(0.0f,0.0f);
            StartCoroutine(launchFireball());
        }  
        else {
            // Stop movement
            rbe.velocity = new Vector2(0.0f,0.0f);
        } 
            // Destroy instance if health is zero and grant player experience
            if (health <=0){
                Destroy(this.gameObject);
                playerScript.updateXP(0.4f);
        } 
    }

    protected override void FixedUpdate()
    {
        // Move enemy
        if (isMoving && isFrozen == false){
            moveEnemy(movement);
        }
    }

    // Launch fireballs
    private IEnumerator launchFireball(){
        // Set state booleans 
        isLaunching = true;
        isMoving = false;
        // Fire 3 fireballs in a row
        for( int i = 0; i < 3; i++){
            // If mage is not frozen
            if (isFrozen == false){
                // Wait half a second
                yield return new WaitForSeconds(0.5f);
                // Create direction and rotation using player transform
                Vector3 dir3 = playerTransform.position - transform.position;
                Vector2 dir2 = new Vector3(dir3.x, dir3.y);
                Vector2 temp = new Vector2(input_x, input_y); 
                // Instantiate fireball
                Projectile fireball = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Projectile>(); 
                // Set up rotation and direction
                fireball.Setup(dir2, ChooseProjDirection());  
            }
        }
        // Wait for 1.5 second
        yield return new WaitForSeconds(1.5f);
        // Reset state booleans 
        isMoving = true;
        isLaunching = false;
        // Reset the walk controller
        StartCoroutine(walkController());
    }

    // Use the player's transform to get fireball rotation
    Vector3 ChooseProjDirection()
    {
        Vector3 dir3 = playerTransform.position - transform.position;
        float temp = Mathf.Atan2(dir3.y, dir3.x) * Mathf.Rad2Deg;
        return new Vector3(0, 0, temp);
    }

    // Freeze the mage in place
    public IEnumerator freeze(){
        // Set state booleans and disable movement
        isFrozen = true;
        isMoving = false;
        isLaunching = true;
        // Set frozen sprite 
        anim.SetBool("frozen", true);
        yield return new WaitForSeconds(3f);
        // Reset and enable walk controller
        isMoving = true;
        isLaunching = false;
        anim.SetBool("frozen", false);
        isFrozen = false;
        StartCoroutine(walkController());
    }

    // Controls walk timer
    private IEnumerator walkController(){
        // Wait for two seconds
        yield return new WaitForSeconds(2f);
        // Disable movement
        isMoving = false;
    }

    // Trigger events
    public new void OnTriggerEnter2D(Collider2D col){
        if (col.gameObject.tag == "energyball" && isFrozen == false){
            // When hit by the eneryball, take damage and freeze the mage
            takeDamage();
            StartCoroutine(freeze());
        }
        // Hit by arrow
        if (col.gameObject.tag == "projectile"){
            // If mage isn't taking damage
            if (!takingDamage){
                // Take damage
                takeDamage();
            }
        }
        if (col.gameObject.tag == "Player"){
            // If player's melee attack is active and mage isn't taking damage
            if (playerScript.getMeleeState() && !takingDamage){
                // Take damage
                takeDamage();
                // If gold armour is active
                if (col.gameObject.GetComponent<Rigidbody2D>() != null && playerScript.IsGold())
                {
                    // Initiate enemy knockback
                    StartCoroutine(Knockback());
                }
            }
        }
    }

    // Collision events
    public new void OnCollisionEnter2D(Collision2D col){
        // Collision with arrow
        if (col.gameObject.tag == "projectile"){
            // If enemy isn't taking damage
            if (!takingDamage){
                // Take damage
                takeDamage();
            }
        }
        // Collision with player
        if (col.gameObject.tag == "Player"){
            // If player's melee attack is active and enemy isn't taking damage
            if (playerScript.getMeleeState() && !takingDamage){
                // Take damage
                takeDamage();
                // If gold armour is active
                if (col.gameObject.GetComponent<Rigidbody2D>() != null && playerScript.IsGold())
                {
                    // Initiate enemy knockback
                   StartCoroutine(Knockback());
                }
            }
        }
    }  
}

