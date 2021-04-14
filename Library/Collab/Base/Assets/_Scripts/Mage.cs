using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : Enemy
{   
    //GameObject boss instance
    public GameObject mage;

    public Animator anim;

    private float input_x;
    private float input_y;
    private bool isMoving;
    private bool isLaunching;
    private bool isFrozen;

    public GameObject projectile;
    //Start method
    new void Start(){
        
        // Superclass start method call
        base.Start();
        //start boss health at 10 HP
        base.setHealth(6); 
        isMoving = true;
        isLaunching = false;
        isFrozen = false;

        anim = gameObject.GetComponent<Animator>();
        StartCoroutine(walkController());
    }

    //Overriden Update method
    public override void Update()
    {
        rbe.isKinematic = true;
        if (isMoving && knockback == false){
            Vector3 direction = playerTransform.position - transform.position;
            // Left 
            if(direction.x < -0.4f && System.Math.Abs(direction.y) < 0.4f) {input_x = -1f; input_y = 0f;}
            // Right
            else if(direction.x > 0.4f && System.Math.Abs(direction.y) < 0.4f) {input_x = 1f; input_y = 0f;}
            // Up
            else if(direction.y > 0.4f && System.Math.Abs(direction.x) < 0.4f){input_x = 0f; input_y =-1f;}
            // DOwn
            else {input_x = 0f; input_y = 1f;}
            direction.Normalize();
            // Set movement
            movement = direction;
            anim.SetFloat("x", input_x);
            anim.SetFloat("y", input_y);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,lockPos,lockPos);
        } else if (isLaunching == false){
            rbe.velocity = new Vector2(0.0f,0.0f);
            StartCoroutine(launchFireball());
        }  
        else {rbe.velocity = new Vector2(0.0f,0.0f);} 
             // Destroy instance if health is zero
       if (health <=0)
       {
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

    private IEnumerator launchFireball(){
        isLaunching = true;
        isMoving = false;
        for( int i = 0; i < 3; i++){
            if (isFrozen == false){
            yield return new WaitForSeconds(0.5f);
            Vector3 dir3 = playerTransform.position - transform.position;
            Vector2 dir2 = new Vector3(dir3.x, dir3.y);
            Vector2 temp = new Vector2(input_x, input_y); //use movement to do arrow direction
            Projectile fireball = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Projectile>(); //creating arrow and ref to script
            fireball.Setup(dir2, ChooseProjDirection());  //set up arrow with direction
            }
        }
        yield return new WaitForSeconds(1.5f);
        isMoving = true;
        isLaunching = false;
        StartCoroutine(walkController());
    }

        //use the directions to shoot where facing
    Vector3 ChooseProjDirection()
    {
        Vector3 dir3 = playerTransform.position - transform.position;
        float temp = Mathf.Atan2(dir3.y, dir3.x) * Mathf.Rad2Deg;
        return new Vector3(0, 0, temp);
    }

    public IEnumerator freeze(){
        isFrozen = true;
        isMoving = false;
        isLaunching = true;
        anim.SetBool("frozen", true);
        yield return new WaitForSeconds(3f);
        isMoving = true;
        isLaunching = false;
        anim.SetBool("frozen", false);
        isFrozen = false;
        StartCoroutine(walkController());
    }

    private IEnumerator walkController(){
        yield return new WaitForSeconds(2f);
        isMoving = false;
    }

    public new void OnTriggerEnter2D(Collider2D col){
        if (col.gameObject.tag == "energyball" && isFrozen == false){
            takeDamage();
            StartCoroutine(freeze());
        }
       if (col.gameObject.tag == "projectile"){
            // If enemy isn't taking damage
            if (!takingDamage){
                // Take damage
                takeDamage();
            }
        }
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

    public new void OnCollisionEnter2D(Collision2D col){

        if(col.gameObject.tag == "wall"){
            rbe.velocity = new Vector2(0.0f,0.0f);
        }
        if (col.gameObject.tag == "projectile"){
            // If enemy isn't taking damage
            if (!takingDamage){
                // Take damage
                takeDamage();
            }
        }
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

