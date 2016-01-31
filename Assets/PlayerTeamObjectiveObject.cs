using UnityEngine;
using System.Collections;


// this class is to represent what the enemies want to attack
public class PlayerTeamObjectiveObject : MonoBehaviour {

    public int TeamBaseHealth = 100;

    public int mObjectId = 0;

    public void Awake()
    {
        mObjectId = GameController.GetNextObjectId();
        GameController.sActiveObjects.Add(mObjectId, this);
    }

}
