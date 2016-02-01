using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GameController : MonoBehaviour
{
    public static GameController sInstance = null;
    public static int sLastObjectId = 32; // arbitrary inflation for static keys
    public static int GetNextObjectId()
    {
        return ++sLastObjectId;
    }

    public static bool sGameStarted = false;
    public static bool sGameEnded = false;

    public static Dictionary<int, PlayerObject> sActivePlayers = new Dictionary<int, PlayerObject>();
    public static Dictionary<int, PlayerTeamObjectiveObject> sActiveObjects = new Dictionary<int, PlayerTeamObjectiveObject>();
    public static Dictionary<int, BaseMonsterBrain> sSpawnedMonsters = new Dictionary<int, BaseMonsterBrain>();
    public static Dictionary<int, ReviveSpawns> sReviveSpawns = new Dictionary<int, ReviveSpawns>();

    public void ClearStatics()
    {
        sActivePlayers.Clear();
        sActiveObjects.Clear();
        sSpawnedMonsters.Clear();
        sReviveSpawns.Clear();
        sGameStarted = false;
        sGameEnded = false;
    }

    public static ReviveSpawns GetClosestReviveSpawn(Vector3 fromPoint, ref float outDistance)
    {
        ReviveSpawns retVal = null;
        outDistance = 0f;
        foreach (KeyValuePair<int, ReviveSpawns> monster in sReviveSpawns)
        {
            float curDistance = Vector3.Distance(monster.Value.transform.position, fromPoint);
            if (!retVal || curDistance < outDistance)
            {
                retVal = monster.Value;
                outDistance = curDistance;
            }
        }

        return retVal;
    }

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
            outDistance = playerDistance;
            return playObj.gameObject;
        }
        else if( objvObj && !playObj)
        {
            outDistance = objectiveDistance;
            return objvObj.gameObject;
        }
        else if( playerDistance < objectiveDistance )
        {
            outDistance = playerDistance;
            return playObj.gameObject;
        }
        else
        {
            outDistance = objectiveDistance;
            return objvObj.gameObject;
        }
    }

    public static PlayerTeamObjectiveObject GetClosestGameObjective( Vector3 fromPoint, ref float outDistance )
    {
        PlayerTeamObjectiveObject retVal = null;
        outDistance = 0f;
        foreach (KeyValuePair<int, PlayerTeamObjectiveObject> objective in sActiveObjects)
        {
            if(objective.Value.IsDestroyed() )
            {
                continue;
            }
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

    public void Start()
    {
        sInstance = this;
    }

    public void OnPlayerDepleted( int objectId )
    {
        if( !sActivePlayers.ContainsKey(objectId))
        {
            return;
        }

        if( sActivePlayers.Count == 1)
        {
            GameOver();
        }
        else
        {
            bool anyAlive = false;
            foreach( KeyValuePair<int, PlayerObject> pairObj in sActivePlayers )
            {
                PlayerObject player = pairObj.Value;
                if( !player  )
                {
                    Debug.LogError("Huge problmen, bro");
                    continue;
                }

                PlayerChargeTracker charger = player.GetComponent<PlayerChargeTracker>();
                if( !charger.IsDepleted())
                {
                    anyAlive = true;
                }
            }

            if( !anyAlive)
            {
                GameOver();
            }
        }

    }

    bool playerOneSpawned = false;
    bool playerTwoSpawned = false;

    public void Update()
    {
        if( sGameEnded)
        {
            if(HotInputManager.sInstance)
            {
                if(HotInputManager.sInstance.GetStart(0) || HotInputManager.sInstance.GetStart(1))
                {
                    ClearStatics();
                    Application.LoadLevel(Application.loadedLevelName);
                }
            }

            return;
        }

        bool bStartGame = false;
        if( !playerOneSpawned && HotInputManager.sInstance && HotInputManager.sInstance.GetStart(0))
        {
            playerOneSpawned = true;
            AddPlayerOne();
            bStartGame = true;
        }

        if( !playerTwoSpawned && HotInputManager.sInstance && HotInputManager.sInstance.GetStart(1))
        {
            playerTwoSpawned = true;
            AddPlayerTwo();
            bStartGame = true;
        }

        if(bStartGame)
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        if(sGameStarted)
        {
            return;
        }
        
        sGameStarted = true;
    }

    public void OnObjectiveDestroyed( int objectId )
    {
        if( !sActiveObjects.ContainsKey(objectId) )
        {
            Debug.LogError("Object destroyed that we don't know about");
            return;
        }

        sActiveObjects.Remove(objectId);
        if( sActiveObjects.Count == 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        sGameEnded = true;
        sGameStarted = false;
        // Show some game over UI
    }

    public ProgressRenderer m_playerOneBar = null;
    public ProgressRenderer m_playerTwoBar = null;

    public GameObject player1Prefab = null;
    public GameObject player2Prefab = null;

    public Transform startLocation = null;

    public void AddPlayerTwo()
    {
        Debug.Log("Attempting to spawn player two when we have : " +sActivePlayers.Count.ToString());

        if( sActivePlayers.ContainsKey(1))
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

        playerObj.OnStart(1);

        PlayerChargeTracker chargeTrack = playerObj.GetComponent<PlayerChargeTracker>();
        if (m_playerTwoBar && chargeTrack)
        {
            Debug.Log("Go into the water");
            m_playerTwoBar.m_TrackedResource = chargeTrack;
            chargeTrack.m_HealthBar = m_playerTwoBar;
        }

        GameController.sActivePlayers.Add(1, playerObj);
    }

    public void AddPlayerOne()
    {
        Debug.Log("Attempting to spawn player one when we have : " + sActivePlayers.Count.ToString());
        if (sActivePlayers.ContainsKey(0))
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

        playerObj.OnStart(0);

        PlayerChargeTracker chargeTrack = playerObj.GetComponent<PlayerChargeTracker>();
        if ( m_playerOneBar && chargeTrack)
        {
            Debug.Log("Live there die there");
            m_playerOneBar.m_TrackedResource = chargeTrack;
            chargeTrack.m_HealthBar = m_playerOneBar;
        }
        GameController.sActivePlayers.Add(0, playerObj);
    }
}
