using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinChestInstantiation : MonoBehaviour
{
    public GameObject Coin;
    public GameObject TreasureChest;
    private Vector3[] coinSpawnPos;
    private Vector3[] chestSpawnPos;

    // Start is called before the first frame update
    void Start()
    {
        coinSpawnPos = new Vector3[19];
        coinSpawnPos[0] = new Vector3(3,5,0);
        coinSpawnPos[1] = new Vector3(4,2,0);
        coinSpawnPos[2] = new Vector3(6,8,0);
        coinSpawnPos[3] = new Vector3(5,2,0);
        coinSpawnPos[4] = new Vector3(6,8,0);
        coinSpawnPos[5] = new Vector3(11,3,0);
        coinSpawnPos[6] = new Vector3(9,8,0);
        coinSpawnPos[7] = new Vector3(11,9,0);
        coinSpawnPos[8] = new Vector3(12,5,0);
        coinSpawnPos[9] = new Vector3(7,9,0);
        coinSpawnPos[10] = new Vector3(24,4,0);
        coinSpawnPos[11] = new Vector3(24,5,0);
        coinSpawnPos[12] = new Vector3(24,7,0);
        coinSpawnPos[13] = new Vector3(28,7,0);
        coinSpawnPos[14] = new Vector3(27,7,0);
        coinSpawnPos[15] = new Vector3(32,7,0);
        coinSpawnPos[16] = new Vector3(33,7,0);
        coinSpawnPos[17] = new Vector3(33,6,0);
        coinSpawnPos[18] = new Vector3(31,6,0);
        

        chestSpawnPos = new Vector3[4];

        chestSpawnPos[0] = new Vector3(9,6,0);
        chestSpawnPos[1] = new Vector3(24,8,0);
        chestSpawnPos[2] = new Vector3(27,8,0);
        chestSpawnPos[3] = new Vector3(27,3,0);


        
        for(int i=0; i<chestSpawnPos.Length; i++){
            Instantiate(TreasureChest,chestSpawnPos[i],Quaternion.identity);
             
            }
        for(int i = 0; i<coinSpawnPos.Length; i++){
            Instantiate(Coin,coinSpawnPos[i],Quaternion.identity);
        }
     }

   
}
