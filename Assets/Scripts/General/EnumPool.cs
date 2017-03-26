using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Idle = 0,    // 0
    Patrol,      // 1
    Suspicious,   // 2
	Detection,  //3
	Alert,  //4
	Attack,  //5
	Escape,  //6
	Sturn,  //7 기절 상태이다.
	Dead,  //8
	Sit,  //9
}

public enum EnemyAttackType
{
	Gun = 0,
	Sword
}

public class EnumPool
{

}
