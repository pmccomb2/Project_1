using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    public Player playerScript;
    public int playerCoinCount;

    void OnCollision(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            // do something
            
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        playerCoinCount = playerScript.coins;
    }
}
