using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
//manages which team the player will be on
public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager instance;
    //counts the players
    static int playerCount;

    void Awake()
    {
        //makes sure there is only one player manager
        if (instance == null)
        {
            instance = this;
            //makes sure this one is not destoyed
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //destroys duplicates
            Destroy(gameObject);
            return;
        }
    }
    //code only runs on server
    [Server]
    public static void SetTeam(GameObject newPlayer)
    {
        PlayerScript player = newPlayer.GetComponent<PlayerScript>();
        //determines which team player will be on
        player.teamNum = (int)Mathf.Repeat(playerCount, 2);
        playerCount++;
    }


}