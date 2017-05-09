using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunState : IState
{
    public StunState(GameObject obj) : base(obj) { }
    
    private float deadVanishTimer = 0.0f;
    private const float deadVanish = 3.0f;
    private const float deadVanishComplete = 4.0f;
    
    public override void OnStateEnter()
    {
    }

    public override void OnStateStay()
    {
        deadVanishTimer += Time.deltaTime;
        if (deadVanishTimer >= deadVanishComplete)
        {
            // 아예 사라지는 루틴
            enemyScript.SetMaterialAlpha(0.0f);
            enemyScript.DisableGhosting();
        }
        else if (deadVanishTimer >= deadVanish)
        {
            // 서서히 사라지는 루틴
            enemyScript.SetMaterialAlpha((deadVanishComplete - deadVanishTimer) / (deadVanishComplete - deadVanish));
        }
        else
        {
            // 스턴 애니메이션하는 루틴
            // Stun(enemyScript.transform, Vector3.zero);
        }
    }

    public override void OnStateExit()
    {
    }

    //현재 State가 Patrol인지를 따진 후, Stay or Enter를 호출한다.
    public override State ChangeState(State nowState)
    {
        if (nowState == State.Stun)
        {
            OnStateStay();
        }
        else
        {
            if (CheckState())
                OnStateEnter();
        }
        return State.Stun;
    }

    public override bool CheckState()
    {
        return true;
    }
}