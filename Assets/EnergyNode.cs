using UnityEngine;
using System.Collections;

public class EnergyNode : MonoBehaviour {

    public float m_RechargeTime = 20f;
    public float m_TimeTilRecharge = 0f;
    public bool m_IsCharged = false;

    public GameObject m_PowerDownParticle = null;

    public Animator m_Animator = null;

    // Use this for initialization
    void Start () {
        if (!m_Animator)
            m_Animator = GetComponent<Animator>();
	
	}
	
	// Update is called once per frame
	void Update () {
	    if( !m_IsCharged )
        {
            m_TimeTilRecharge -= Time.deltaTime;
            if( m_TimeTilRecharge <= 0f )
            {
                m_IsCharged = true;
                OnCharged();
            }
        }
	}

    void OnCharged()
    {
        if(m_Animator)
        {
            m_Animator.SetBool("FullCharge", true);
        }
    }

    void AbsorbCharge()
    {
        if( m_Animator)
        {
            m_Animator.SetBool("FullCharge", false);
        }
        m_IsCharged = false;
        m_TimeTilRecharge = m_RechargeTime;
        if( m_PowerDownParticle)
        {
            Instantiate(m_PowerDownParticle, transform.position, transform.rotation);
        }
    }

    public void OnTriggerEnter( Collider other )
    {
        if( !m_IsCharged )
        {
            return;
        }

        PlayerChargeTracker charger = other.GetComponent<PlayerChargeTracker>();
        if( charger && !charger.IsMaxed() )
        {
            charger.RefillCharge();
            AbsorbCharge();
        }
    }

    public void OnTriggerStay( Collider other)
    {
        if( !m_IsCharged )
        {
            return;
        }

        PlayerChargeTracker charger = other.GetComponent<PlayerChargeTracker>();
        if (charger && !charger.IsMaxed())
        {
            charger.RefillCharge();
            AbsorbCharge();
        }
    }
}
