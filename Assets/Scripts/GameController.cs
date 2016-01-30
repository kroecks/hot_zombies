using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GameController : MonoBehaviour
{
    public static Dictionary<int, PlayerObject> sActivePlayers = new Dictionary<int, PlayerObject>();

    public void Update()
    {
        if( Input.GetButtonDown("StartGame"))
        {
            StartGame();
        }
    }

    public bool gameStarted = false;

    public void StartGame()
    {
        if( gameStarted )
        {
            return;
        }
        AddPlayerOne();
        AddPlayerTwo();
    }


    public GameObject player1Prefab = null;
    public GameObject player2Prefab = null;

    public Transform startLocation = null;

    public void AddPlayerTwo()
    {
        if( sActivePlayers.ContainsKey(2 ))
        {
            return;
        }

        GameObject newPlayer = Instantiate(player2Prefab, startLocation.transform.position, startLocation.transform.rotation) as GameObject;
        if (!newPlayer)
        {
            return;
        }

        PlayerObject playerObj = newPlayer.GetComponent<PlayerObject>();

        if (!playerObj)
        {
            return;
        }

        playerObj.OnStart();

        sActivePlayers.Add(2, playerObj);

        Debug.Log("There are now " + sActivePlayers.Count.ToString() + " players");
    }

    public void AddPlayerOne()
    {
        if (sActivePlayers.ContainsKey(1))
        {
            return;
        }

        GameObject newPlayer = Instantiate(player1Prefab, startLocation.transform.position, startLocation.transform.rotation) as GameObject;
        if( !newPlayer )
        {
            return;
        }

        PlayerObject playerObj = newPlayer.GetComponent<PlayerObject>();

        if( !playerObj )
        {
            return;
        }

        playerObj.OnStart();

        sActivePlayers.Add(1, playerObj);

        Debug.Log("There are now " + sActivePlayers.Count.ToString() + " players");
    }
}
