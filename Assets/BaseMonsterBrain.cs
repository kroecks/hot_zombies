using UnityEngine;
using System.Collections;

public class BaseMonsterBrain : MonoBehaviour {

    public bool m_BrainActive = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (m_BrainActive)
        {
            UpdateMonsterMove();   
        }
	
	}

    public float m_MoveSpeed = 1f;
    public float m_MonsterAttackDistance = 2f;

    public void UpdateMonsterMove()
    {
        float distanceTo = 0f;
        PlayerObject closest = GameController.GetClosestPlayer(transform.position, ref distanceTo);
        if( !closest )
        {
            return;
        }

        // If we haven't caught up, keep following
        if( m_MonsterAttackDistance < distanceTo)
        {
            Vector3 dir = (transform.position - closest.transform.position);
            dir = Vector3.Normalize(dir);
            Vector3 desiredPos = (closest.transform.position) + (m_MonsterAttackDistance * dir);
            Vector3 newPosition = Vector3.MoveTowards(transform.position, desiredPos, m_MoveSpeed * Time.deltaTime);
            transform.position = newPosition;
        }
        else
        {
            UpdateAttack();
        }
    }

    public void UpdateAttack()
    {

    }

    public void OnTriggerEnter( Collider other )
    {
        if( other.GetComponent<PlayerObject>() )
        {
            m_BrainActive = true;
        }
    }
}
