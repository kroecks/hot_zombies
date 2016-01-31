using UnityEngine;
using System.Collections;


// this class is to represent what the enemies want to attack
public class PlayerTeamObjectiveObject : MonoBehaviour {

    public AudioClip[] m_TakeDamageClips = new AudioClip[0];

    public float m_ObjectiveHealth = 100f;

    public int mObjectId = 0;

    public void Awake()
    {
        mObjectId = GameController.GetNextObjectId();
        GameController.sActiveObjects.Add(mObjectId, this);
    }

    public void Update()
    {
        if( m_TimeTilNextDamageSound > 0f)
            m_TimeTilNextDamageSound -= Time.deltaTime;
    }

    public bool IsDestroyed()
    {
        return m_ObjectiveHealth <= 0f;
    }

    public float m_TimeTilNextDamageSound = 0f;
    public float m_DamageSoundRate = 0.5f;

    public void TakeDamage( Vector3 fromDir, float damageAmt )
    {
        if(IsDestroyed())
        {
            return;
        }
        m_ObjectiveHealth -= damageAmt;

        if( m_TimeTilNextDamageSound <= 0f && m_TakeDamageClips.Length > 0)
        {
            m_TimeTilNextDamageSound = m_DamageSoundRate;
            AudioClip playClip = m_TakeDamageClips[Random.Range(0, m_TakeDamageClips.Length)];
            AudioSource aud = GetComponent<AudioSource>();
            if(playClip && aud )
            {
                aud.PlayOneShot(playClip);
            }
        }

        if( m_ObjectiveHealth <= 0f)
        {
            OnDestroyed();
        }
    }

    public void OnDestroyed()
    {

    }

}
