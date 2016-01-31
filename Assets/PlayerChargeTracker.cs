using UnityEngine;
using System.Collections;

public class PlayerChargeTracker : MonoBehaviour {

    public float m_MaxCharge = 100f;
    public float m_CurrentCharge = 100f;

	public void UseCharge( float chargeUsage )
    {
        m_CurrentCharge -= chargeUsage;
    }

    public bool IsMaxed()
    {
        return m_CurrentCharge == m_MaxCharge;
    }

    public float GetCurrentCharge()
    {
        return m_CurrentCharge;
    }

    public void RefillCharge()
    {
        m_CurrentCharge = m_MaxCharge;
    }
}
