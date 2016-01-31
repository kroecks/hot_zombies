using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseMonsterBrain : MonoBehaviour {

    public int mObjectId = 0;

    public bool m_BrainActive = false;

    public SpriteRenderer m_SpriteComp = null;

    public GameObject m_StunPrefab = null;
    public GameObject m_RevivePrefab = null;
    public GameObject m_deathParticlePrefab = null;

    public float m_MoveSpeed = 1f;
    public float m_MonsterAttackDistance = 2f;

    public float m_KnockOutHealth = 2f;

    public float m_MonsterHealthCurrent = 5f;
    public float m_MonsterHealthMax = 5f;

    public Vector3 m_DesiredMovePosition = Vector3.zero;

    public Animator m_AnimComponent = null;

    public void Awake()
    {
        mObjectId = GameController.GetNextObjectId();
        GameController.sSpawnedMonsters.Add(mObjectId, this);
    }

	// Update is called once per frame
	void Update () {
        if (m_BrainActive)
        {
            UpdateMonsterBrain();   
        }
	}

    public virtual void UpdateMonsterBrain()
    {
        UpdateDesiredMovePoint();
        UpdateMonsterMove();
        UpdateAttack();
    }

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

        if (m_AnimComponent)
        {
            m_AnimComponent.SetBool("IsStunned", true);
        }
    }

    public void OnRevive()
    {
        m_MonsterHealthCurrent = m_MonsterHealthMax;
        m_BrainActive = true;

        if(m_RevivePrefab)
        {
            Instantiate(m_RevivePrefab, transform.position, Quaternion.identity);
        }
    }

    public Vector3 GetMoveDirection()
    {
        return Vector3.Normalize(m_DesiredMovePosition - transform.position);
    }

    public virtual void UpdateDesiredMovePoint()
    {
        float distanceTo = 0f;
        GameObject closest = GameController.GetClosestAttackableObject(transform.position, ref distanceTo);
        if (!closest)
        {
            return;
        }

        Vector3 dir = (closest.transform.position - transform.position);
        Vector3 desiredPos = (closest.transform.position) + (m_MonsterAttackDistance * dir);
        m_DesiredMovePosition = desiredPos;
    }

    public virtual Vector3 GetDesiredMovePoint()
    {
        return m_DesiredMovePosition;
    }

    public void UpdateMonsterMove()
    {
        Vector3 moveTo = GetDesiredMovePoint();
        Vector3 moveDir = GetMoveDirection();

        float distanceTo = Vector3.Distance(moveTo, transform.position);

        if (moveDir.x != 0f && m_SpriteComp != null)
        {
            Vector3 spriteScale = Vector3.one;
            spriteScale.x = (moveDir.x > 0f) ? 1f : -1f;
            m_SpriteComp.transform.localScale = spriteScale;
        }

        // If we haven't caught up, keep following
        if ( m_MonsterAttackDistance < distanceTo)
        {            
            Vector3 newPosition = Vector3.MoveTowards(transform.position, moveTo, m_MoveSpeed * Time.deltaTime);
            transform.position = newPosition;
            if (m_AnimComponent)
            {
                m_AnimComponent.SetBool("IsAttacking", false);
            }
        }
        else
        {
            if (m_AnimComponent)
            {
                m_AnimComponent.SetBool("IsAttacking", true);
            }
        }
    }

    public void UpdateAttack()
    {

    }

    public void OnDeath()
    {
        GameController.sSpawnedMonsters.Remove(mObjectId);
    }

    public void OnTriggerEnter( Collider other )
    {
        if( other.GetComponent<PlayerObject>() )
        {
            m_BrainActive = true;
        }
    }
}
