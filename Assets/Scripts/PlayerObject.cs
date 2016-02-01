using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Rewired;

public class PlayerObject : MonoBehaviour
{
    public int mObjectId = 0;
    public int mPlayerId = 0;

    public Player mRewiredPlayer = null;

    public bool m_MovementEnabled = true;

    PlayerMoveController moveController = null;
    PlayerFireController fireController = null;
    PlayerChargeTracker chargeController = null;

    public void Awake()
    {
       // mObjectId = GameController.GetNextObjectId();
    }

    public void OnStart( int playerId )
    {
        mObjectId = playerId;
        moveController = GetComponent<PlayerMoveController>();
        fireController = GetComponent<PlayerFireController>();
        chargeController = GetComponent<PlayerChargeTracker>();
    }

    public void Update()
    {
        if (chargeController.GetCurrentCharge() <= 0f)
        {
            if( fireController )
            {
                fireController.Disable();
            }
            return;
        }

        if( m_MovementEnabled )
        {
            if( moveController)
                moveController.UpdateMovement();

            if (fireController)
                fireController.UpdateFireController();
        }
        else
        {
            // We should verify that we're actually associated with someone
            // we're a player, so just check the other player for secondary held
            int otherPlayerId = (mPlayerId != 0) ? 0 : 1;
            PlayerObject otherPlayer = GameController.sActivePlayers.ContainsKey(otherPlayerId) ? GameController.sActivePlayers[otherPlayerId] : null;
            if( otherPlayer && otherPlayer.GetComponent<PlayerFireController>())
            {
                if( !otherPlayer.GetComponent<PlayerFireController>().IsSecondaryHeld() )
                {
                    SetMovementEnabled(true);
                }
            }
            else
            {
                SetMovementEnabled(true);
            }
        }
        
    }

    public void SetMovementEnabled( bool movementEnabled )
    {
        m_MovementEnabled = movementEnabled;
        BoxCollider box = GetComponent<BoxCollider>();
        if (box)
        {
            box.enabled = m_MovementEnabled;
        }

        CharacterController charControl = GetComponent<CharacterController>();
        if( charControl )
        {
            charControl.enabled = m_MovementEnabled;
        }
    }
}
