using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IState
{
    protected GameObject enemyObj;
    protected EnemyScript enemyScript;

    // TODO!!!!!!!!!!!!!!!!!!!!!!!!!!
    // 부모에서 awake를 구현하고 자식에서 따로 오버라이드 하지 않으면 아마 부모의 awake가 불릴걸? 시험해봐야함
    public IState() { }


    public IState(GameObject obj)
    {
        enemyObj = obj;
    }

    public void Hold()
    {

    }

    public void Walk(Transform t)
    {
        t.Translate(Vector3.right * 0.5f * Time.deltaTime);
    }

    public abstract void OnStateEnter(GameObject obj);
    public abstract void OnStateStay(GameObject obj);
    public abstract void OnStateExit(GameObject obj);
    public abstract EnemyState ChangeState(GameObject obj);
}