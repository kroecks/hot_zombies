using UnityEngine;
using System.Collections;

public class ThrowCatcher : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnTriggerEnter( Collider other )
    {
        CatchableObject catchable = other.GetComponent<CatchableObject>();
        if( catchable )
        {
            Destroy(other.gameObject);
        }
    }
}
