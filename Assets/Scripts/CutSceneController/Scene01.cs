using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene01 : MonoBehaviour
{
    private CutSceneUnit[] busAndWheel;

    private void Awake()
    {
        busAndWheel = GameObject.FindObjectsOfType<CutSceneUnit>();
    }

    public void MakeBusComes()
    {
        for (int i = 0; i < busAndWheel.Length; i++)
        {
            busAndWheel[i].StartAction();
        }
    }
}
