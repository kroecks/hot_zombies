using UnityEngine;
using System.Collections;

public class ReviverMonsterBrain : BaseMonsterBrain {

    public override void UpdateDesiredMovePoint()
    {
        float distanceTo = 0f;
        BaseMonsterBrain closest = GameController.GetClosestKOMonster(transform.position, ref distanceTo);
        if (closest)
        {
            Vector3 dir = (transform.position - closest.transform.position);
            Vector3 desiredPos = (closest.transform.position) + (m_MonsterAttackDistance * dir);
            m_DesiredMovePosition = desiredPos;
            return;
        }

        base.UpdateDesiredMovePoint();
    }

    public override void UpdateMonsterBrain()
    {
        UpdateMonsterMove();
    }


}
