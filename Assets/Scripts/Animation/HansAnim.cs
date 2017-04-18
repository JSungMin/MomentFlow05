using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HansAnim : AnimationBase
{
    private new void Awake()
    {
        base.Awake();
    }

    public void ReadPaper()
    {
        // TODO!!!!!
        // ReSearcher는 Hans로 바꿔야함
        // ReadPaper 애니메이션 만들어야 함
        // setAnimation(0, charAnimName + "_ReadPaper", true, 1.0f);
        Debug.Log("hans is reading paper!");
    }

    public void ClosePaper()
    {
        setAnimation(0, charAnimName + "_ClosePaper", true, 1.0f);
    }
}
