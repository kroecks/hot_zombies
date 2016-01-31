using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GameController : MonoBehaviour
{
    public static int sLastObjectId = 0;
    public static int GetNextObjectId()
    {
        return ++sLastObjectId;
    }

    public static Dictionary<int, PlayerObject> sActivePlayers = new Dictionary<int, PlayerObject>();
    public static Dictionary<int, PlayerTeamObjectiveObject> sActiveObjects = new Dictionary<int, PlayerTeamObjectiveObject>();
    public static Dictionary<int, BaseMonsterBrain> sSpawnedMonsters = new Dictionary<int, BaseMonsterBrain>();

    public static BaseMonsterBrain GetClosestKOMonster( Vector3 fromPoint, ref float outDistance )
    {
        BaseMonsterBrain retVal = null;
        outDistance = 0f;
        foreach (KeyValuePair<int, BaseMonsterBrain> monster in sSpawnedMonsters)
        {
            if(!monster.Value.IsKnockedOut())
            {
                continue;
            }
            float curDistance = Vector3.Distance(monster.Value.transform.position, fromPoint);
            if (!retVal || curDistance < outDistance)
            {
                retVal = monster.Value;
                outDistance = curDistance;
            }
        }

        return retVal;
    }

    public static GameObject GetClosestAttackableObject( Vector3 fromPath, ref float outDistance )
    {
        float playerDistance = 0f;
        PlayerObject playObj = GetClosestPlayer(fromPath, ref playerDistance);

        float objectiveDistance = 0f;
        PlayerTeamObjectiveObject objvObj = GetClosestGameObjective(fromPath, ref objectiveDistance);

        if(playObj && !objvObj)
        {
            return playObj.gameObject;
        }
        else if( objvObj && !playObj)
        {
            return objvObj.gameObject;
        }
        else if( playerDistance < objectiveDistance )
        {
            return playObj.gameObject;
        }
        else
        {
            return objvObj.gameObject;
        }
    }

    public static PlayerTeamObjectiveObject GetClosestGameObjective( Vector3 fromPoint, ref float outDistance )
    {
        PlayerTeamObjectiveObject retVal = null;
        outDistance = 0f;
        foreach (KeyValuePair<int, PlayerTeamObjectiveObject> objective in sActiveObjects)
        {
            float curDistance = Vector3.Distance(objective.Value.transform.position, fromPoint);
            if (!retVal || curDistance < outDistance)
            {
                retVal = objective.Value;
                outDistance = curDistance;
            }
        }

        return retVal;
        
    }


    public static PlayerObject GetClosestPlayer( Vector3 fromPoint, ref float outDistance )
    {
        PlayerObject retVal = null;
        outDistance = 0f;
        foreach ( KeyValuePair<int,PlayerObject> player in sActivePlayers )
        {
            float curDistance = Vector3.Distance(player.Value.transform.position, fromPoint);
            if ( !retVal || curDistance < outDistance)
            {
                retVal = player.Value;
                outDistance = curDistance;
            }
        }

        return retVal;
    }

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
    }

    public ProgressRenderer m_playerOneBar = null;

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

        if( m_playerOneBar )
        {
            m_playerOneBar.m_TrackedResource = playerObj.GetComponent<PlayerChargeTracker>();
        }
    }
}
