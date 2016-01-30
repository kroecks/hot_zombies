using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainCameraController : MonoBehaviour {

    public BoundsBox cameraBounds = null;
    public static MainCameraController sInstance = null;

    public float lowestCamDistance = 10f;
    public float highestCamDistance = 50f;

    public float cameraHeightDampening = 1f;
    public float cameraMoveDampening = 1f;

	// Use this for initialization
	void Start () {
        cameraBounds = new BoundsBox(transform.position);
        sInstance = this;
        m_desiredCamPoint = transform.position;
    }
	
	// Update is called once per frame
	void Update () {

        TrackPlayers();
        MoveCamera();
    }

    void MoveCamera()
    {
        transform.position = Vector3.Lerp(transform.position, m_desiredCamPoint, Time.deltaTime * cameraMoveDampening);
    }

    public Vector3 m_desiredCamPoint = Vector3.zero;

    // This is actually how close they need to be within for the camera to be at its lowest point
    public float m_minPlayerDistance = 10f;
    public float m_maxPlayerDistance = 25f;

    public float m_cameraBoundsRadius = 10f;

    void TrackPlayers()
    {
        Vector3 startingPoint = transform.position;
        startingPoint.z = 0f;
        cameraBounds = new BoundsBox(startingPoint);
        foreach ( KeyValuePair<int,PlayerObject> playerPair in GameController.sActivePlayers)
        {
            PlayerObject player = playerPair.Value;
            int playerNum = playerPair.Key;
            cameraBounds.GrowTo(player.transform.position);
        }

        Vector3 centerPoint = cameraBounds.GetCenter();
        float camHeight = 0f;


        float heightRatio = 0f;
        float maxDistance = cameraBounds.MaxDistance();
        if (maxDistance > m_minPlayerDistance)
        {
            float distanceFromMin = maxDistance - m_minPlayerDistance;
            float maxDelta = m_maxPlayerDistance - m_minPlayerDistance;
            heightRatio = (distanceFromMin / maxDelta);
        }

        camHeight = Mathf.Lerp(lowestCamDistance, highestCamDistance, heightRatio);
        centerPoint.z = Mathf.Clamp(camHeight, lowestCamDistance, highestCamDistance);
        centerPoint.z *= -1f;

        m_desiredCamPoint = centerPoint;
        
    }

    static Vector3 GetCameraPosition()
    {
        if( sInstance )
        {
            return sInstance.transform.position;
        }
        return Vector3.zero;
    }
}
