using UnityEngine;
using System.Collections;

public class AnimationEndListener : MonoBehaviour {

	public void OnAnimEnd()
    {
        transform.root.SendMessage("OnAnimationEnd");
    }
}
