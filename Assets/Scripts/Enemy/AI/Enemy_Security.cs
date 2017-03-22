using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Security : EnemyScript {


	void Awake()
    {
		playerObject = GameObject.FindObjectOfType<Player> ();

        // TODO!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // 부모, 자식의 생성자 호출에 대해서 더 공부해야할것
        // 만약 이게 깔끔하게 된다면 gameobject를 여기서만 보내주고 두고두고 사용할 수 있음
		istate = new IState[2];
		istate [0] = new IdleState (gameObject);
		istate [1] = new PatrolState (gameObject);
	}

	public float stateChange = 2;
	private float stateChangeTimer = 0;

    // Update is called once per frame
    void Update()
    {
        switch (enemyState)
        {
            case EnemyState.Idle:
                if (stateChangeTimer <= stateChange)
                {
                    // 함수 내부에서 형변환을 해주어서 조금 더 깔끔하게 짜기 위해 함수를 구현함
                    // 단 성능이 더 중요하다면 함수를 호출하는 overload를 줄이기 위해 함수를 사용하지 않아도 됨
                    GetState(EnemyState.Idle).OnStateStay(gameObject);
                    stateChangeTimer += Time.deltaTime;
                }
                else
                {

                }
                break;
            case EnemyState.Patrol:
                GetState(EnemyState.Patrol).OnStateStay(gameObject);
                break;
        }
    }
}
