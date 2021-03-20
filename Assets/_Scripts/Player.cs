using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Text healthText;
    public Text movementText;
    public Text shieldText;
    public Text teleportationText;

    private int health;
    private int movement;
    private int shield;
    private float teleportationCooldown;

    // Start is called before the first frame update
    void Start()
    {
        health = 10;
        movement = 8;
        shield = 3;
        teleportationCooldown = 30;
    }

    // Update is called once per frame
    void Update()
    {
        setGUIText();
        if (teleportationCooldown > 0){
            teleportationCooldown -= Time.deltaTime;
        }
    }

    void FixedUpdate(){

    }

    void setGUIText(){
        healthText.text = health.ToString() + " / 10";
        movementText.text = movement.ToString() + " / 10";
        shieldText.text = shield.ToString() + " / 10";
        teleportationText.text = ((int) teleportationCooldown).ToString();
    }
}
