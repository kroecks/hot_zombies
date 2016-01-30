using UnityEngine;
using System.Collections;

public class PlayerAimController : MonoBehaviour {

    public enum AimDirection
    {
        eAimDirectionN,
        eAimDirectionNE,
        eAimDirectionE,
        eAimDirectionSE,
        eAimDirectionS,
        eAimDirectionSW,
        eAimDirectionW,
        eAimDirectionNW,
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    float verticalAim = 0f;
    float horizontalAim = 0f;

    public string VerticalAimStr;
    public string HorizontalAimStr;

    public Transform AimTransform = null;

    public Vector3 CurrentAim = Vector3.zero;

    public void UpdateAim()
    {
        verticalAim = Input.GetAxis(VerticalAimStr);
        horizontalAim = Input.GetAxis(HorizontalAimStr);

        if(verticalAim == 0f && horizontalAim == 0f)
        {
            return;
        }

        CurrentAim.x = horizontalAim;
        CurrentAim.y = verticalAim;
    }

    public AimDirection GetAimDirection()
    {
        return AimDirection.eAimDirectionE;
    }
}
