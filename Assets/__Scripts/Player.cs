using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // Variable declarations

    // Coordinate vectors
    public Vector2 TeleportPosition;
    public Vector2 CaveExit;
    public Vector2 HutEnter;
    public Vector2 HutExit;
    public Vector2 bossSpawn;
    // Rigidbody component reference
    public Rigidbody2D rb;
    // GUI Text GameObject references
    public Text healthText;
    public Text movementText;
    public Text shieldText;
    public Text teleportationText;
    public Text infoText;
    public Text coinText;
    public Text gameLevelText;
    public Text LevelText;
    // XP Bar GameObject references
    public Image XPBar;
    private bool increasingXP;
    private float xp;
    private float XPincrease;
    // Private control variables
    private int health;
    private int movement;
    private int shield;
    private int level;
    private int _coins;
    private float teleportationCooldown;
    private int gameLevel = 1;
    // Animators
    Animator anim;
    Animator energyBallAnimator;
    // Keyboard input variables
    private float input_x;
    private float input_y;
    // Private state booleans
    private bool isWalking;
    private bool isMelee = false;
    private bool isShoot = false;
    private bool isCharge = false;
    private bool chargeReady = false;
    private bool canMove = true;
    private bool castSpells = false;
    private bool teleportationEnabled;
    private bool takingDamage;
    private bool isBlue;
    private bool isGold;
    // GameObject references
    public GameObject boss;
    public GameObject projectile;
    public GameObject energyBall;
    // Energyball script reference
    private EnergyBall ball;
    // Animator override controllers
    public AnimatorOverrideController blue;
    public AnimatorOverrideController gold;

    void Start()
    {
        // Initilize control variables
        health = 15;
        movement = 3;
        shield = 3;
        level = 1;
        xp = 0.0f;
        coins = 0;
        teleportationCooldown = 0;
        teleportationEnabled = true;
        anim = GetComponent<Animator>();
        this.takingDamage = false;
        isBlue = false;
        isGold = false;
        chargeReady = false;
        // Initialize GUI and experience bar
        infoText.text = "";
        XPincrease = 0f;
        LevelText.text = level.ToString();
        XPBar.fillAmount = 0.0f;
        updateXP(0.0f);
        updateCoinText();
    }

    void Update()
    {
        // Set GUI text every frame
        setGUIText();
        // Reduce teleportation cooldown
        if (teleportationCooldown > 0){
            teleportationCooldown -= Time.deltaTime;
        }
        // If the player can move, move the player and check for teleporation commands
        if (canMove){
            MovePlayer();
            CheckTeleport();
        }
        // Get keyboard input
        GetInput();
        //if players health hits zero the level will restart
        if (health <= 0){
            SceneManager.LoadScene("TitleScreen");
        }
        // Fill the experience bar
        IncreaseFillAmount();
        // Check for unused energyball instances
        DestroyOldEnergyballs();
    }

    // Updates GUI text to display live values
    void setGUIText(){
        healthText.text = health.ToString() + " / 20";
        movementText.text = movement.ToString() + " / 20";
        shieldText.text = shield.ToString() + " / 20";
        teleportationText.text = ((int) teleportationCooldown).ToString();
        gameLevelText.text = "Level " + gameLevel.ToString();
    }

    // Update the GUI to display the correct number of coins
    public void updateCoinText(){
        if (coins > 9){
            coinText.text = "0" + coins.ToString();
        }
        if (coins > 99){
            coinText.text = coins.ToString();
        } else {
            coinText.text = "00" + coins.ToString();
        }
    }

    // Update players experience value
    public void updateXP(float newXP){
        increasingXP = true;
        XPincrease += newXP;
    }
    // Fill the experience bar
    void IncreaseFillAmount(){
        // If experience is increasing
        if (increasingXP == true){
            // Gradually fill the experience bar
            XPBar.fillAmount += XPincrease * Time.deltaTime;
            // Check if the experience bar is full
            if (XPBar.fillAmount >= 1.0f && XPBar.fillAmount <= xp + XPincrease){
                    // Reset the bar
                    XPincrease = xp + XPincrease - 1.0f;
                    xp = 0f;
                    level += 1;
                    UpdateLevelText();
                    XPBar.fillAmount = 0f;
            }
            // Ensure the bar has reached the correct value
            if (XPBar.fillAmount >= xp + XPincrease){
                xp = xp + XPincrease;
                XPincrease = 0f;
                // Stop increasing
                increasingXP = false;
            }
        }
    }

    // Update the players experience level
    void UpdateLevelText(){
        LevelText.text = level.ToString();
    }

    // Returns experience level
    public int GetExperienceLevel(){
        return level;
    }

    // Get keyboard input 
    private void GetInput()
    {
        // Left and right arrow key input
        input_x = Input.GetAxisRaw("Horizontal");
        input_y = Input.GetAxisRaw("Vertical");
        // Return key triggers a melee attack
        if (Input.GetKeyDown("return") && !isMelee)
        {
            StartCoroutine(AttackCo());
        }
        // 'm' key triggers the arrow projectile
        else if(Input.GetButtonDown("Second Weapon") && !isShoot)
        {
            StartCoroutine(SecondAttackCo());
        }
        // If spells are active, check for spell casting input
        if (castSpells){
            checkSpellCast();
        }
        // Update the isWalking state boolean
        isWalking = (Mathf.Abs(input_x) + Mathf.Abs(input_y)) > 0;
    }

    // Move the player
    private void MovePlayer()
    // Player movement and speed is affected by the movement stat
    {  
        // Set the walking animation to the value of the isWalking boolean
        anim.SetBool("isWalking", isWalking);
        // If the player is walking
        if (isWalking)
        {
            // Move the player based on the current movement value
            float multiplier = (float) movement / 6f;
            anim.SetFloat("x", input_x);
            anim.SetFloat("y", input_y);
            Vector3 pos = transform.position;
            pos.x += input_x * Time.deltaTime * multiplier;
            pos.y += input_y * Time.deltaTime * multiplier;
            transform.position = pos;
        }
        else
        {
            // Set the players velocity to zero if they are not walking
            rb.velocity = new Vector2(0.0f,0.0f);
        }
    }

    // Checks if teleport is being activated
    private void CheckTeleport(){
        // If teleportation cooldown is less than one second, enable teleportation
        if (teleportationCooldown < 1f){
            teleportationEnabled = true;
        }
        else {teleportationEnabled = false;}
        // Save the players position
        Vector3 pos = transform.position;
        // If the player presses space bar and teleportation is enabled, move one unit in the direction they are facing
        if ((Input.GetKey(KeyCode.A) || Input.GetKey("left")) && Input.GetKeyDown("space") && teleportationEnabled)
        {
            teleportationCooldown = 15;
            pos.x += -1;
            transform.position = pos;
        }
        else if ((Input.GetKey(KeyCode.D) || Input.GetKey("right")) && Input.GetKeyDown("space") && teleportationEnabled)
        {
            teleportationCooldown = 15;               
            pos.x += 1;
            transform.position = pos;
        }
        else if ((Input.GetKey(KeyCode.W) || Input.GetKey("up")) && Input.GetKeyDown("space") && teleportationEnabled)
        {
            teleportationCooldown = 15;
            pos.y += 1;
            transform.position = pos;
        }
        else if ((Input.GetKey(KeyCode.S) || Input.GetKey("down")) && Input.GetKeyDown("space") && teleportationEnabled)
        {
            teleportationCooldown = 15;
            pos.y += -1;
            transform.position = pos;
        }
    }

    // Launch a melee attack
    private IEnumerator AttackCo(){
        // Trigger the players melee animation
        anim.SetBool("melee", true);
        // Set the isMelee state boolean to disable the player from more melee attacks
        isMelee = true;
        yield return null;
        // Disable the melee animation
        anim.SetBool("melee", false);
        // Wait for a moment
        yield return new WaitForSeconds(.05f);
        // Enable melee attacks
        isMelee = false;
    }

    // Launch an arrow projectile
    private IEnumerator SecondAttackCo()
    {
        // Fires arrows
        isShoot = true;
        yield return null;
        MakeArrow();
        // If the blue armor is equipped fire 3 arrows instead
        if (isBlue){
            MultiArrow();
        }
        yield return new WaitForSeconds(.33f);
        isShoot = false;
    }

    // Fire a single arrow
    private void MakeArrow()
    {   
        Vector2 temp = new Vector2(anim.GetFloat("x"), anim.GetFloat("y")); // Use movement to get arrow direction
        Arrow arrow = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Arrow>(); // Creating arrow and reference to script
        arrow.Setup(temp, ChooseProjDirection());  // Set up arrow with direction and rotation
    }

    // Fire multiple arrows
    private void MultiArrow(){
        Vector2 temp1 = new Vector2(anim.GetFloat("x"), anim.GetFloat("y")); // Use movement to get arrow direction 
        Arrow arrow1 = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Arrow>(); // Creating arrow and reference to script
        float tempFloat1 = Mathf.Atan2(anim.GetFloat("y"), anim.GetFloat("x")) * Mathf.Rad2Deg;
        Vector3 upDirection = new Vector3(20, 0, tempFloat1);
        arrow1.Setup(temp1, upDirection); // Set up arrow with direction and rotation
        Vector2 temp2 = new Vector2(anim.GetFloat("x"), anim.GetFloat("y")); // Use movement to do arrow direction
        Arrow arrow2 = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Arrow>(); //creating arrow and ref to script
        float tempFloat2 = Mathf.Atan2(anim.GetFloat("y"), anim.GetFloat("x")) * Mathf.Rad2Deg;
        Vector3 downDirection = new Vector3(-20, 0, tempFloat2);
        arrow2.Setup(temp2, downDirection); // Set up arrow with direction and rotation
    }
    
    // Use the player's movement direction to get arrow rotation
    Vector3 ChooseProjDirection()
    {
        float temp = Mathf.Atan2(anim.GetFloat("y"), anim.GetFloat("x")) * Mathf.Rad2Deg;
        return new Vector3(0, 0, temp);
    }
    
    // Return if the player is currently using a melee attack
    public bool getMeleeState(){
        return isMelee;
    }

    // If the player isn't charging a spell and is holding down the right shift key, begin charging a spell
    private void checkSpellCast(){
        if(Input.GetKeyDown(KeyCode.RightShift) && isCharge == false){
                // Set the state variable
                isCharge = true;
                StartCoroutine(chargeEnergyball());
        }
        // The the player releases the shift key
        if(Input.GetKeyUp(KeyCode.RightShift)){
            // Enable movement
            canMove = true;
            // If a charged ball is ready to be released
            if (chargeReady == true && isCharge == true && ball != null){
                    // Set the energyball to the release animation
                    energyBallAnimator = ball.GetComponent<Animator>();
                    energyBallAnimator.SetBool("release", true);
                    // Launch the energy ball
                    StartCoroutine(launchEnergyball());
            } else {
                    // Destroy the energy ball if it is not fully charged
                    GameObject obj1 = GameObject.FindGameObjectWithTag("energyball");
                    Destroy(obj1);
                    isCharge = false;
            }
            // Update the charge state boolean
            chargeReady = false;
        }
        // Check if there are unused energyball instances
        if (ball != null && Input.GetKey(KeyCode.RightShift) == false){
            // Destroy them
            GameObject obj1 = GameObject.FindGameObjectWithTag("energyball");
            Destroy(obj1);
            isCharge = false;
        }
    }

    // Begin charging an energyball
    private IEnumerator chargeEnergyball(){
        // Disable movement
        canMove = false;
        // Set coordinates to slightly infront of the player
        float tempx = transform.position.x + anim.GetFloat("x") * 0.25f;
        float tempy = transform.position.y + anim.GetFloat("y") * 0.25f;
        Vector3 temp = new Vector3(tempx, tempy, transform.position.z);
        // Instantiate the energyball
        ball = Instantiate(energyBall, temp, Quaternion.identity).GetComponent<EnergyBall>();
        // Check if the player has released shift
        if (ball != null && Input.GetKey(KeyCode.RightShift) == false){
            // Destroy the energyball if shift key is not being pressed
            GameObject obj1 = GameObject.FindGameObjectWithTag("energyball");
                    Destroy(obj1);
                    // Update the isCharge state boolean
                    isCharge = false;
        }
        // Wait for the energyball to fully charge
        yield return new WaitForSeconds(0.32f);
        // Check if the player has released shift
        if (ball != null && Input.GetKey(KeyCode.RightShift) == false){
            // Destroy the energyball if shift key is not being pressed
            GameObject obj1 = GameObject.FindGameObjectWithTag("energyball");
                    Destroy(obj1);
                    // Update the isCharge state boolean
                    isCharge = false;
        }
        // Check if the player has released shift
        if (ball != null && Input.GetKey(KeyCode.RightShift)){
            // Reset the animator grow boolean
            energyBallAnimator = ball.GetComponent<Animator>();
            energyBallAnimator.SetBool("grow", false);
            // Set the charge ready state boolean to true
            chargeReady = true;
        }
    }

    // Launch a charged energyball
    private IEnumerator launchEnergyball(){
        // Enable player movement
        canMove = true;
        // Set a vector based on the direction the player is facing
        Vector2 temp = new Vector2(anim.GetFloat("x"), anim.GetFloat("y"));
        // Setup the direction and velocity of the energyball
        ball.Setup(temp, ChooseProjDirection());
        // Reset the ball reference object and animator reference object
        ball = null;
        energyBallAnimator = null;
        // Set the charge ready state boolean to false for a 2 second cooldown
        chargeReady = false;
        yield return new WaitForSeconds(2f);
        isCharge = false;
    }

    // Destroy unused energyballs
    void DestroyOldEnergyballs(){
        // Find energyball using tag
        GameObject eball = GameObject.FindGameObjectWithTag("energyball");
        if (eball != null && ball == null ){
            // Set reference to its rigid body
            Rigidbody2D body = eball.GetComponent<Rigidbody2D>();
                // Destroy it if it is not moving
                if (body.velocity.magnitude < 1.0f){
                       Destroy(eball);
                }
        }
    }

    // Inflict damage to the player
    public void takeDamage(){
        float multiplier = (float) shield / 2f; // Takes damage in respect to the value of the shield variable
        float damage = 6f - multiplier;
        Debug.Log(damage);
        health -= (int) damage;
        // Make the players opacity flash to indicate damage has been taken
        StartCoroutine(Flasher());
    }

    // After the player takes damage, flash opacity and disable more damage temporarily
    private IEnumerator Flasher() 
         {
             takingDamage = true;
             // Flash opacity in a loop three times
             for (int i = 0; i < 3; i++)
             {
              GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 0.3f);
              yield return new WaitForSeconds(.1f);
              GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 1f);
              yield return new WaitForSeconds(.1f);
              GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 0.3f);
              yield return new WaitForSeconds(.1f);
              GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 1f);
              yield return new WaitForSeconds(.1f);
             }
             takingDamage = false;
          }

    // Return the current game level the player is on
    public int getGameLevel(){
        return gameLevel;
    }

    // Public property coins sets and gets the number of coins the player has collected
    public int coins {
        get {
            return _coins;
        }
        set {
            _coins = value;
        }
    }
    
    // Displaying an on-screen message
    public IEnumerator displayMessage(string message){
        infoText.text = message;
        // Wait for a moment
        yield return new WaitForSeconds(1.3f);
        // Remove the message
        infoText.text = "";
    }

    // Return a boolean indicating if the player is wearing gold armor
    public bool IsGold(){
        return isGold;
    }

    // Trigger events
    private void OnTriggerEnter2D(Collider2D collider)
    {   
        // Teleportation events

        // If the player is level 2, the door silo teleports them to the second level
        if (collider.tag == "DoorSilo" && isWalking && level >= 2)
        {
            transform.position = TeleportPosition;
            gameLevel = 2;
        } else if (collider.tag == "DoorSilo"){
            StartCoroutine(displayMessage("Must be level 2 to enter!"));
            infoText.color = Color.red;
        }
        // Entering the armor hut
        if (collider.tag == "Hut_Enter" && isWalking)
        {
            transform.position = HutEnter;
        }
        // Leaving the armor hut
        if (collider.tag == "Hut_exit" && isWalking)
        {
            transform.position = HutExit;
        }
        // Going back to the first level from the cave
        if (collider.tag == "CaveDoor" && isWalking)
        {
            transform.position = CaveExit;
        }

        // Picking up the wand 
        if (collider.tag == "wand"){
            // Destroy the wand
            GameObject wand = GameObject.FindGameObjectWithTag("wand");
            Destroy(wand);
            // Enable the player to cast spells
            castSpells = true;
        }
        // Player is approaching the dragon
        if (collider.tag == "SpawnDragon" && isWalking)
        {   
            // Instantiate the dragon
            Instantiate(boss, bossSpawn, Quaternion.identity);
        }
        // Colecting coins
        if (collider.tag == "coin"){
            Destroy(collider.gameObject);
            coins += 1;
            updateCoinText();
        }
    }

    // Collision events
    public void OnCollisionEnter2D(Collision2D col){
        if (col.gameObject.tag == "enemy" && !isMelee && !takingDamage){
            // Player will take damage on collision with enemy if not attacking or flashing
            takeDamage();
        }
        if (col.gameObject.tag == "BlueArmor" && isWalking)
        // Colliding with blue armor will change stats and animations
        {
            GetComponent<Animator>().runtimeAnimatorController = blue;
            isBlue = true;
            isGold = false;
            movement = 8;
            shield = 6;
            // Display a message indicating stat upgrades
            StartCoroutine(displayMessage("Armor +3\nSpeed +5"));
            infoText.color = Color.blue;
        }
        if (col.gameObject.tag == "GoldArmor" && isWalking)
        // Colliding with gold armor will change stats and animations
        {
            GetComponent<Animator>().runtimeAnimatorController = gold;
            movement = 6;
            shield = 8;
            isGold = true;
            isBlue = false;
            // Display a message indicating stat upgrades
            StartCoroutine(displayMessage("Armor +5\nSpeed +3"));
            infoText.color = Color.yellow;
            Debug.Log("Armor " + GetComponent<Collider>().ToString());
            Debug.Log("Collision Detected!");
        }
        // Collision with chest
        if (col.gameObject.tag == "Chest")
        {
            // If the player has five coins, deduct five coins from the total and heal player health by 5
            if(coins >= 5 )
            {
                if(health <= 15){
                    health += 5;
                    coins -= 5;
                    updateCoinText();
            } else { 
                health = 20;
                coins -= 5;
                updateCoinText();
                }
            StartCoroutine(displayMessage("Health +5"));
            infoText.color = Color.magenta;
            }
        }
    }
}
