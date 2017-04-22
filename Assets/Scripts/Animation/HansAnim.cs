using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HansAnim : AnimationBase
{
    private new void Awake()
    {
        base.Awake();
    }

	public void Sit(){
		setAnimation (0, "Sit",true,1.0f);
	}

	public void OpenPaper(){
		setAnimation (1, "OpenPaper",false,1.0f);
	}

    public void ReadPaper()
    {
        // TODO!!!!!
        // ReSearcher는 Hans로 바꿔야함
        // ReadPaper 애니메이션 만들어야 함
        setAnimation(1, "ReadPaper", true, 1.0f);
    }

    public void ClosePaper()
    {
		setAnimation(1, "ClosePaper", false, 1.0f);
		Debug.Log ("Hans Close Paper");
    }
}
