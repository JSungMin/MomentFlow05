using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Idle = 0,    // 0
    Patrol,      
    Suspicious,   
	Detection,  
	Alert,  
	Attack,  
	Escape,  
	Stun,  // 기절 상태이다.
	Dead,  
	Sit,  
}

public enum PlayerSkillState
{
	None,
	Replay,
	Stop
}

public enum EnemyAttackType
{
	Gun = 0,
	Sword
}

public class EnumPool
{

}
