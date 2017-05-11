using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : IState
{
    public DeadState(GameObject obj) : base(obj) { }
    
    public override void OnStateEnter()
    {
    }

    public override void OnStateStay()
    {
    }

    public override void OnStateExit()
    {
    }
    
    public override State ChangeState(State nowState)
    {
        return nowState;
    }

    public override bool CheckState()
    {
        return true;
    }
}
