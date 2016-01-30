using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Projectile
{
    public GameObject m_prefab = null;
    public float m_projectileSpeed = 10f;

    public Texture m_uiTexture = null;
}

public class PlayerFireController : MonoBehaviour {

    public Projectile m_primaryFire = new Projectile();

    public Projectile[] m_projectiles = new Projectile[0];

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void UpdateFireController()
    {
        ProcessInput();
    }

    public string m_fireButtonStr;
    public string m_secondaryFireButtonStr;

    bool m_secondaryHeld = false;
    bool m_primaryHeld = false;

    bool m_secondaryStatusAchieved = false;

    void ProcessInput()
    {
        m_fireCooldown -= Time.deltaTime;

        bool secondInitialHeld = m_secondaryHeld;
        bool firstInitialHeld = m_primaryHeld;
        m_secondaryHeld = false;
        m_primaryHeld = false;

        if ( Input.GetButton( m_fireButtonStr ) )
        {
            m_primaryHeld = true;
        }
        else if ( Input.GetButton(m_secondaryFireButtonStr))
        {
            m_secondaryHeld = true;
        }

        if (m_primaryHeld)
        {
            OnPrimaryHeld();
            // Can't hold secondary while primary
            m_secondaryHeld = false;
        }

        // If we're letting go of secondary, eject our passenger
        if (secondInitialHeld != m_secondaryHeld)
        {
            OnSecondaryStateChange();
        }
        // If we don't have a passenger continue trying to grab one
        else if( m_secondaryHeld )
        {
            OnSecondaryHeld();
        }

        


    }

    public float m_fireRate = 1f;
    public float m_fireCooldown = 0f;

    public float m_secondarySuckUpRange = 5f;

    public Transform m_suckHoldObject = null;
    public Transform m_suckHoldPosition = null;

    void OnSecondaryHeld()
    {
        if( !m_suckHoldObject)
        {
            Collider[] foundObjects = Physics.OverlapSphere(transform.position, m_secondarySuckUpRange);

            foreach( Collider suckedObj in foundObjects )
            {
                if( suckedObj.gameObject == this.gameObject )
                {
                    continue;
                }

                PlayerObject player = suckedObj.GetComponent<PlayerObject>();
                if( player )
                {
                    // Let's try it, fuck it.
                }

                SuctionGrabbable suckComp = suckedObj.GetComponent<SuctionGrabbable>();
                if( !suckComp)
                {
                    continue;
                }

                suckComp.SetGrabbed(true);
                m_suckHoldObject = suckComp.transform;
            }
        }
        else if(m_suckHoldObject && m_suckHoldPosition)
        {
            m_suckHoldObject.position = m_suckHoldPosition.position;
        }
    }

    void OnSecondaryStateChange()
    {
        if( !m_secondaryHeld && m_suckHoldObject)
        {
            OnSeconaryReleased();
        }
    }

    IEnumerator ThrowingObject( Transform throwObj, Vector3 throwTo, float throwTime )
    {
        float timeInThrow = 0f;
        Vector3 startingPosition = throwObj.position;
        while( timeInThrow < throwTime )
        {
            float percThrow = Mathf.Clamp01(timeInThrow / throwTime);
            Vector3 newPositon = Vector3.Lerp(throwObj.position, throwTo, percThrow);
            timeInThrow += Time.deltaTime;

            throwObj.position = newPositon;

            yield return new WaitForFixedUpdate();
        }
    }

    public float m_throwTime = 0.3f;
    public float m_throwDistance = 10f;

    void OnSeconaryReleased()
    {
        SuctionGrabbable suckComp = m_suckHoldObject.GetComponent<SuctionGrabbable>();
        if(suckComp)
        {
            suckComp.SetGrabbed(false);
        }

        Vector3 throwTo = (m_suckHoldPosition.position - transform.position) * m_throwDistance;

        StartCoroutine(ThrowingObject(m_suckHoldObject, throwTo, m_throwTime));
        
        m_suckHoldObject = null;
    }

    void OnPrimaryHeld()
    {
        if(m_fireCooldown > 0f)
        {
            return;
        }

        FirePrimary();
    }

    void FirePrimary()
    {
        m_fireCooldown = m_fireRate;
    }
}
