using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCollision : MonoBehaviour
{
    // Start is called before the first frame update
    public GameManager gameManager;

    PlayerController playerScript;


    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerScript = gameManager.player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        if(this.tag =="Wall" && other.tag == "Player")
        {
            //The player is out of bounds: put logic here

            //1.tell player they're gonna die
            playerScript.outOfBounds = true;
            //2.player tells UI they're gonna die
            //3.Player/UI starts countdown til death
            //4.Within player script, if the countdown reaches 0 destroy the player
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(this.tag=="Wall" && other.tag == "Player")
        {
            //Player returns into the map: put logic here
            //1.End countdown
            playerScript.outOfBounds = false;
            //2.Player stops warning, tells UI script to stop warning

        }
    }
}
