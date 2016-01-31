using UnityEngine;
using System.Collections;

public class BaseMonsterBrain : MonoBehaviour {

    public bool m_BrainActive = false;

    public SpriteRenderer m_SpriteComp = null;

    public GameObject m_StunPrefab = null;

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

    public GameObject m_deathParticlePrefab = null;

    public float m_MoveSpeed = 1f;
    public float m_MonsterAttackDistance = 2f;

    public float m_KnockOutHealth = 2f;

    public float m_MonsterHealthCurrent = 5f;
    public float m_MonsterHealthMax = 5f;

    public bool IsKnockedOut()
    {
        return m_MonsterHealthCurrent <= m_KnockOutHealth;
    }

    public void DoDamage( float incDamage, Vector3 damageDir )
    {
        if(m_MonsterHealthCurrent <= 0f )
        {
            return;
        }

        m_MonsterHealthCurrent -= incDamage;

        if(m_BrainActive && m_MonsterHealthCurrent <= m_KnockOutHealth )
        {
            OnKnockedOut(damageDir);
        }

        if( m_MonsterHealthCurrent <= 0f )
        {
            OnDeath(damageDir);
        }
    }

    public void OnDeath( Vector3 damageDir )
    {
        if(m_deathParticlePrefab)
        {
            Instantiate(m_deathParticlePrefab, transform.position, Quaternion.LookRotation(damageDir));
        }
        Destroy(gameObject);
    }

    public void OnKnockedOut(Vector3 damageDir)
    {
        if(m_StunPrefab)
        {
            Instantiate(m_StunPrefab, transform.position, Quaternion.LookRotation(damageDir));
        }
        m_BrainActive = false;
    }

    public void OnRevive()
    {
        m_MonsterHealthCurrent = m_MonsterHealthMax;
        m_BrainActive = true;
    }

    public void UpdateMonsterMove()
    {
        float distanceTo = 0f;
        PlayerObject closest = GameController.GetClosestPlayer(transform.position, ref distanceTo);
        if( !closest )
        {
            return;
        }

        Vector3 dir = (transform.position - closest.transform.position);

        if (dir.x != 0f)
        {
            Vector3 spriteScale = Vector3.one;
            spriteScale.x = (dir.x < 0f) ? 1f : -1f;
            m_SpriteComp.transform.localScale = spriteScale;
        }

        // If we haven't caught up, keep following
        if ( m_MonsterAttackDistance < distanceTo)
        {
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
