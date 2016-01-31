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

    public float m_desiredCamHeight = 8f;
    public float m_currentCamHeight = 8f;

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

        Camera camComp = GetComponent<Camera>();
        if (camComp)
        {
            m_currentCamHeight = Mathf.Lerp(m_currentCamHeight, m_desiredCamHeight, Time.deltaTime * cameraHeightDampening);
            camComp.orthographicSize = Mathf.Clamp(m_currentCamHeight, lowestCamDistance, highestCamDistance);
        }
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

        BoundsBox priorityBounds = new BoundsBox(startingPoint);

        foreach ( KeyValuePair<int,PlayerObject> playerPair in GameController.sActivePlayers)
        {
            PlayerObject player = playerPair.Value;
            cameraBounds.GrowTo(player.transform.position);
            priorityBounds.GrowTo(player.transform.position);
        }

        foreach( KeyValuePair<int, PlayerTeamObjectiveObject> objPair in GameController.sActiveObjects )
        {
            PlayerTeamObjectiveObject player = objPair.Value;
            cameraBounds.GrowTo(player.transform.position);
            priorityBounds.GrowTo(player.transform.position);
        }

        foreach (KeyValuePair<int, BaseMonsterBrain> monstPair in GameController.sSpawnedMonsters)
        {
            BaseMonsterBrain monster = monstPair.Value;
            cameraBounds.GrowTo(monster.transform.position);
        }

        cameraBounds.GrowBy(m_cameraBoundsRadius);

        Vector3 centerPoint = priorityBounds.GetCenter();

        float heightRatio = 0f;
        float maxDistance = cameraBounds.MaxDistance();
        if (maxDistance > m_minPlayerDistance)
        {
            float distanceFromMin = maxDistance - m_minPlayerDistance;
            float maxDelta = m_maxPlayerDistance - m_minPlayerDistance;
            heightRatio = (distanceFromMin / maxDelta);
        }

        m_desiredCamHeight = Mathf.Lerp(lowestCamDistance, highestCamDistance, heightRatio);

        m_desiredCamPoint = centerPoint;
        m_desiredCamPoint.z = -10f;

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
