using UnityEngine;
using System.Collections;

public class AutoDestruct : MonoBehaviour {

    float m_Lifetime = 2f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        m_Lifetime -= Time.deltaTime;
        if( m_Lifetime <= 0f)
        {
            Destroy(gameObject);
        }

    }
}
