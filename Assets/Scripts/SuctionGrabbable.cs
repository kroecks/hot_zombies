using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SuctionGrabbable : MonoBehaviour {

    public bool m_grabbed = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if( m_grabbed )
        {
            bool isAnyHeld = false;
            foreach( KeyValuePair<int, PlayerObject> playerPair in GameController.sActivePlayers)
            {
                if (playerPair.Value.GetComponent<PlayerFireController>().IsSecondaryHeld())
                {
                    isAnyHeld = true;
                }
            }

            if(!isAnyHeld)
            {
                SetGrabbed(false);

                BoxCollider boxCol = GetComponent<BoxCollider>();
                if (boxCol)
                {
                    boxCol.enabled = true;
                }
            }
        }
	
	}

    public bool CanGrab()
    {
        BaseMonsterBrain monster = GetComponent<BaseMonsterBrain>();
        if( monster && !monster.IsKnockedOut())
        {
            return false;
        }

        return true;
    }

    public void ThrowObject()
    {
        BoxCollider boxCol = GetComponent<BoxCollider>();
        if (boxCol)
        {
            boxCol.enabled = true;
        }
    }

    public void SetGrabbed( bool grabbed )
    {
        if(m_grabbed == grabbed )
        {
            return;
        }
        m_grabbed = grabbed;
        if( m_grabbed )
        {
            PlayerObject player = GetComponent<PlayerObject>();
            if( player )
            {
                player.SetMovementEnabled(false);
            }
        }
        else
        {
            PlayerObject player = GetComponent<PlayerObject>();
            if (player)
            {
                player.SetMovementEnabled(true);
            }
        }

        BoxCollider boxCol = GetComponent<BoxCollider>();
        if( boxCol )
        {
            boxCol.enabled = false;
        }
    }

    public void OnThrowComplete( Vector3 throwDir )
    {
        BaseMonsterBrain monster = GetComponent<BaseMonsterBrain>();
        if( monster )
        {
            monster.OnThrownDeath(throwDir);
        }
    }
}
