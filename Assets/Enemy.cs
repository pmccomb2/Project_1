using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 0.01f;

    private Rigidbody2D rbe;

    public Transform player;

    private Vector2 movement;

    public float lockPos;

    
    // Start is called before the first frame update
    void Start()
    {
        rbe  = this.GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
       Vector3 direction = player.position - transform.position;
       float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
       rbe.rotation = angle;
       direction.Normalize();
       movement = direction;

       transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,lockPos,lockPos);
    }
    void moveCharacter(Vector2 direction)
    {
        rbe.MovePosition((Vector2)transform.position + (direction * moveSpeed * Time.deltaTime));
    }

    void FixedUpdate()
    {
        moveCharacter(movement);
    }
}
