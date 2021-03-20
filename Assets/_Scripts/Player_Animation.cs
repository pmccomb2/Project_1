using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Animation : MonoBehaviour
{
    public Vector2 TeleportPosition;
    public Rigidbody2D rb;
    
    Animator anim;
    // Start is called before the first frame update

    private float input_x;
    private float input_y;
    private bool isWalking;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        
    }
        
    // Update is called once per frame
    void Update()
    {
        
        // TODO ADD Get Input function
        GetInput();
        MovePlayer();
        
       
    }

    private void GetInput()
    {
        input_x = Input.GetAxisRaw("Horizontal");
        input_y = Input.GetAxisRaw("Vertical");


        isWalking = (Mathf.Abs(input_x) + Mathf.Abs(input_y)) > 0;
    }
    private void MovePlayer()
    {
        anim.SetBool("isWalking", isWalking);
        if (isWalking)
        {
            anim.SetFloat("x", input_x);
            anim.SetFloat("y", input_y);
           // rb.velocity = new Vector2(input_x, input_y).normalized;
           Vector3 pos = transform.position;
           pos.x += input_x * Time.deltaTime;
           pos.y += input_y * Time.deltaTime;
           transform.position = pos;
           if (Input.GetKey(KeyCode.A))
           {
               if (Input.GetKeyDown("space"))
               {
                   pos.x += -1;
                   transform.position = pos;
               }
           }
           else if (Input.GetKey(KeyCode.D))
           {
                   if (Input.GetKeyDown("space"))
                   {
                       pos.x += 1;
                       transform.position = pos;
                   }
           }
           else if (Input.GetKey(KeyCode.W))
           {
                   if (Input.GetKeyDown("space"))
                   {
                       pos.y += 1;
                       transform.position = pos;
                   }
           }
           else if (Input.GetKey(KeyCode.S))
           {
                   if (Input.GetKeyDown("space"))
                   {
                       pos.y += -1;
                       transform.position = pos;
                   }
            }

//transform.position += new Vector3(input_x, input_y, 0.0f).normalized * Time.deltaTime;
        }
        else
        {
            rb.velocity = new Vector2(0.0f,0.0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "DoorSilo" && isWalking)
        {
            transform.position = TeleportPosition;
            //collider.transform.position = TeleportPosition;
            Debug.Log("Entering " + collider.ToString());
        }
        //Debug.Log(collider.tag);
        Debug.Log("Collision Trigger Detected!");
    }
}
