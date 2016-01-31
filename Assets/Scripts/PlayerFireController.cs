using UnityEngine;
using System.Collections;
using System;

public class PlayerFireController : MonoBehaviour {

    public LineRenderer m_beamRenderer = new LineRenderer();

    public AudioClip m_fireSoundLoop = null;

    public string m_fireButtonStr;
    public string m_secondaryFireButtonStr;

    bool m_secondaryHeld = false;
    bool m_primaryHeld = false;

    bool m_secondaryStatusAchieved = false;

    public float m_secondarySuckUpRange = 5f;

    public float m_EnergyUsePerSec = 3f;

    public Transform m_suckHoldObject = null;
    public Transform m_suckHoldPosition = null;

    public float m_throwTime = 0.3f;
    public float m_throwDistance = 10f;

    public float m_laserDamagePerSec = 1f;
    public float m_maxFireDistance = 100f;
    public float m_fireRadiusLeniency = 10f;

    // Use this for initialization
    void Awake () {

        if( !m_beamRenderer)
        {
            gameObject.AddComponent<LineRenderer>();
            m_beamRenderer = GetComponent<LineRenderer>();
        }
	
	}
	
    public void UpdateFireController()
    {
        ProcessInput();
    }

    void ProcessInput()
    {
        bool secondInitialHeld = m_secondaryHeld;
        bool firstInitialHeld = m_primaryHeld;
        m_secondaryHeld = false;
        m_primaryHeld = false;

        if ( Input.GetButton( m_fireButtonStr ) || Input.GetAxisRaw( m_fireButtonStr ) != 0f )
        {
            m_primaryHeld = true;
        }
        else if ( Input.GetButton(m_secondaryFireButtonStr) || Input.GetAxisRaw(m_secondaryFireButtonStr) != 0f)
        {
            m_secondaryHeld = true;
        }

        if( m_primaryHeld != firstInitialHeld)
        {
            AudioSource snSrc = GetComponent<AudioSource>();
            if( snSrc )
            {
                if(m_primaryHeld)
                {
                    snSrc.loop = true;
                    snSrc.clip = m_fireSoundLoop;
                    snSrc.Play();
                }
                else
                {
                    snSrc.Stop();
                }
            }
        }

        if (m_primaryHeld)
        {
            m_beamRenderer.enabled = true;
            OnPrimaryHeld();
            // Can't hold secondary while primary
            m_secondaryHeld = false;
        }
        else
        {
            m_beamRenderer.enabled = false;
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

    void OnSecondaryHeld()
    {
        PlayerAimController aimCon = GetComponent<PlayerAimController>();
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
                if( !suckComp || !suckComp.CanGrab())
                {
                    continue;
                }

                suckComp.SetGrabbed(true);
                m_suckHoldObject = suckComp.transform;
            }
        }
        else if(m_suckHoldObject && aimCon)
        {
            m_suckHoldObject.position = aimCon.GetAimOrigin();
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
        Vector3 throwDir = throwTo - throwObj.position;

        throwDir = Vector3.Normalize(throwDir);
        SuctionGrabbable suck = throwObj.GetComponent<SuctionGrabbable>();
        if( suck)
        {
            suck.ThrowObject();
        }

        float timeInThrow = 0f;
        Vector3 startingPosition = throwObj.position;
        while( timeInThrow <= throwTime && throwObj != null )
        {
            float percThrow = Mathf.Clamp01(timeInThrow / throwTime);

            Vector3 newPositon = Vector3.Lerp(startingPosition, throwTo, percThrow);
            throwObj.position = newPositon;

            timeInThrow += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        if( suck )
        {
            suck.OnThrowComplete(throwDir);
        }
    }

    void OnSeconaryReleased()
    {
        SuctionGrabbable suckComp = m_suckHoldObject.GetComponent<SuctionGrabbable>();
        if(suckComp)
        {
            suckComp.SetGrabbed(false);
        }

        PlayerAimController aimCon = GetComponent<PlayerAimController>();
        if( aimCon == null )
        {
            m_suckHoldObject = null;
            return;
        }

        Vector3 throwDir = aimCon.GetAimVector();


        Vector3 aimOrigin = aimCon.GetAimOrigin();
        Vector3 throwTo = aimOrigin + (throwDir * m_throwDistance);

        StartCoroutine(ThrowingObject(m_suckHoldObject, throwTo, m_throwTime));
        
        m_suckHoldObject = null;
    }
    

    void OnPrimaryHeld()
    {
        m_beamRenderer.enabled = true;
        Vector3 aimDir = Vector3.right;
        Vector3 aimOrigin = transform.position;
        PlayerAimController aimCont = GetComponent<PlayerAimController>();
        if( aimCont )
        {
            aimDir = aimCont.GetAimVector();
            aimOrigin = aimCont.GetAimOrigin();
        }

        PlayerChargeTracker chargeComp = GetComponent<PlayerChargeTracker>();
        if( chargeComp )
        {
            chargeComp.UseCharge(m_EnergyUsePerSec * Time.deltaTime);
        }

        Ray testRay = new Ray(aimOrigin, aimDir);
        RaycastHit rayHit = new RaycastHit();
        BaseMonsterBrain hitMonster = null;

        float distanceForLaser = m_maxFireDistance;

        if ( Physics.SphereCast(testRay, m_fireRadiusLeniency, out rayHit, m_maxFireDistance ))
        {
            hitMonster = rayHit.collider.GetComponent<BaseMonsterBrain>();
            distanceForLaser = Vector3.Distance(aimOrigin, rayHit.point);
        }

        Vector3 lastPos = aimOrigin + (aimDir * distanceForLaser);

        if (hitMonster)
        {
            hitMonster.DoDamage(m_laserDamagePerSec * Time.deltaTime, aimDir);
        }

        if( m_beamRenderer )
        {
            m_beamRenderer.SetVertexCount(2);
            m_beamRenderer.SetPosition(0, aimOrigin);
            m_beamRenderer.SetPosition(1, lastPos);
        }
    }

}
