using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerObject : MonoBehaviour
{
    public int playerNum = 0;

    public bool m_MovementEnabled = true;

    PlayerMoveController moveController = null;
    PlayerFireController fireController = null;

    public void OnStart()
    {
        moveController = GetComponent<PlayerMoveController>();
        fireController = GetComponent<PlayerFireController>();
    }

    public void Update()
    {
        if( m_MovementEnabled )
        {
            if( moveController)
                moveController.UpdateMovement();

            if (fireController)
                fireController.UpdateFireController();
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
