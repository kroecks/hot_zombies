using UnityEngine;
using System.Collections;

public class SuctionGrabbable : MonoBehaviour {

    public bool m_grabbed = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetGrabbed( bool grabbed )
    {
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
    }
}
