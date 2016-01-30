using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMoveController : MonoBehaviour {

    public float momentumSpeedUpRate = 1.0f;
    public float momentumSlowDownRate = 1.0f;

    public float currentSpeed = 0f;
    public float maxSpeed = 20f;

    public string moveHorizontalStr;
    public string moveVerticalStr;

    public float moveHorizontal = 0f;
    public float moveVertical = 0f;

    public float momentumPercent = 0f;

    // Update is called once per frame when our player object tells us to
    public void UpdateMovement () {
        moveHorizontal = Input.GetAxis(moveHorizontalStr);
        moveVertical = Input.GetAxis(moveVerticalStr);

        if (moveHorizontal != 0f || moveVertical != 0f)
        {
            momentumPercent = Mathf.Lerp(momentumPercent, 1.0f, Time.deltaTime * momentumSpeedUpRate);
        }
        else
        {
            momentumPercent = Mathf.Lerp(momentumPercent, 0.0f, Time.deltaTime * momentumSpeedUpRate);
        }

        currentSpeed = Mathf.Clamp01(momentumPercent) * maxSpeed;

        Vector3 motion = new Vector3();

        motion.x = moveHorizontal;
        motion.y = moveVertical;

        motion *= currentSpeed;

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
                        if( motion[axis] < 0f)
                        {
                            motion[axis] = 0f;
                        }
                    }
                    else // Since the bounds are absolutely our player positions, check if we're moving out
                    {
                        if (motion[axis] > 0f)
                        {
                            motion[axis] = 0f;
                        }
                    }
                }
            }
        }

        GetComponent<CharacterController>().Move(motion);
	
	}
}
