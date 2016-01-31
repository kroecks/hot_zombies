using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMoveController : MonoBehaviour {

    public AimDirection m_defaultDirection;
    public AimDirection m_currentDirection;

    public Animator m_movementAnim = null;

    public float momentumSpeedUpRate = 1.0f;
    public float momentumSlowDownRate = 1.0f;

    public float currentSpeed = 0f;
    public float maxSpeed = 20f;

    public string moveHorizontalStr;
    public string moveVerticalStr;

    public float moveHorizontal = 0f;
    public float moveVertical = 0f;

    public float momentumPercent = 0f;

    Vector3 moveVector = Vector3.zero;

    // Update is called once per frame when our player object tells us to
    public void UpdateMovement () {
        moveVector = Vector3.zero;

        moveHorizontal = Input.GetAxis(moveHorizontalStr);
        moveVertical = Input.GetAxis(moveVerticalStr);

        bool bMoving = (moveHorizontal != 0f || moveVertical != 0f);

        if(momentumPercent > 0f || bMoving)
        {
            momentumPercent += (bMoving) ? (momentumSpeedUpRate * Time.deltaTime) : -(momentumSlowDownRate * Time.deltaTime);
        }
        
        momentumPercent = Mathf.Clamp01(momentumPercent);

        currentSpeed = momentumPercent * maxSpeed;

        moveVector.x = moveHorizontal;
        moveVector.y = moveVertical;

        moveVector *= currentSpeed;

        if( MainCameraController.sInstance != null )
        {
            BoundsBox playingBounds = MainCameraController.sInstance.cameraBounds;
            Vector3 playSpace = playingBounds.GetSize();
            Vector3 localPosition = transform.position;
            for ( int axis = 0; axis < 3; axis++ )
            {
                if(playSpace[axis] >= MainCameraController.sInstance.m_maxPlayerDistance)
                {
                    // Find out if we're trying to expand it further and block the effort
                    if( localPosition[axis] == playingBounds.GetMin()[axis] )
                    {
                        // If we're equal to min, make sure our movement is positive
                        if(moveVector[axis] < 0f)
                        {
                            moveVector[axis] = 0f;
                        }
                    }
                    else // Since the bounds are absolutely our player positions, check if we're moving out
                    {
                        if (moveVector[axis] > 0f)
                        {
                            moveVector[axis] = 0f;
                        }
                    }
                }
            }
        }

        if( currentSpeed == 0f)
        {
            m_currentDirection = m_defaultDirection;
            if(m_movementAnim)
            {
                m_movementAnim.SetBool("IsMoving", false);
            }
        }
        else
        {
            m_currentDirection = GetAimDirection();
            if (m_movementAnim)
            {
                m_movementAnim.SetBool("IsMoving", true);
            }
        }

        GetComponent<CharacterController>().Move(moveVector);
	
	}

    public float GetDirection()
    {
        Vector3 referenceForward = Vector3.up;

        Vector3 referenceRight = Vector3.Cross(Vector3.forward, referenceForward);
        Vector3 newDirection = moveVector;

        float angle = Vector3.Angle(newDirection, referenceForward);
        float sign = Mathf.Sign(Vector3.Dot(newDirection, referenceRight));
        float finalAngle = sign * angle;

        return finalAngle;
    }

    public AimDirection GetAimDirection()
    {
        float timeBetween = (360f / (float)(AimDirection.eAimDirectionCount));
        float spaceForEachHalf = timeBetween / 2f;

        float angle = GetDirection();

        for (int i = 0; i < (int)AimDirection.eAimDirectionCount; i++)
        {
            float directionAngle = GetAngleForDirection((AimDirection)i);
            if (angle > (directionAngle - spaceForEachHalf) && angle < (directionAngle + spaceForEachHalf))
            {
                return (AimDirection)(i);
            }
        }

        return AimDirection.eAimDirectionN;
    }

    public float GetAngleForDirection(AimDirection checkDir)
    {
        switch (checkDir)
        {
            case AimDirection.eAimDirectionN:
                {
                    return 0f;
                }
            case AimDirection.eAimDirectionNE:
                {
                    return -45f;
                }
            case AimDirection.eAimDirectionNW:
                {
                    return 45;
                }
            case AimDirection.eAimDirectionS:
                {
                    return 180f;
                }
            case AimDirection.eAimDirectionSE:
                {
                    return -135f;
                }
            case AimDirection.eAimDirectionSW:
                {
                    return 135f;
                }
            case AimDirection.eAimDirectionE:
                {
                    return -90f;
                }
            case AimDirection.eAimDirectionW:
                {
                    return 90f;
                }
            default:
                return 0f;
        }
    }
}
