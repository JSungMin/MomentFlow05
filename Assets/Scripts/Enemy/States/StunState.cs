using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunState : IState
{
    public StunState(GameObject obj) : base(obj) { }

    private float deadVanishTimer = 0.0f;
    private const float playerFinishAttack = 1.5f;
    private const float deadVanish = 2.0f;
    private const float deadVanishComplete = 3.0f;

    // 차라리 부모로 빼던가 할 것
    private Player player;

    public override void OnStateEnter()
    {
    }

    private bool isPlayerFinishedAttack = false;
    public override void OnStateStay()
    {
        deadVanishTimer += Time.deltaTime;
        if (deadVanishTimer >= deadVanishComplete)
        {
            // 아예 사라지는 루틴
            enemyScript.enemyState = State.Dead;
            enemyScript.SetMaterialAlpha(0.0f);
            enemyScript.DisableGhosting();
        }
        else if (deadVanishTimer >= deadVanish)
        {
            // 서서히 사라지는 루틴
            enemyScript.SetMaterialAlpha((deadVanishComplete - deadVanishTimer) / (deadVanishComplete - deadVanish));

        }
        else if (deadVanishTimer >= playerFinishAttack)
        {
            if (!isPlayerFinishedAttack)
            {
                player = GameObject.FindObjectOfType<Player>();
                player.state = MyObject.State.Idle;
                isPlayerFinishedAttack = true;
            }
        }
        else
        {
            // 스턴 애니메이션하는 루틴
            Stun(enemyScript.transform, Vector3.zero);
            isPlayerFinishedAttack = false;
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