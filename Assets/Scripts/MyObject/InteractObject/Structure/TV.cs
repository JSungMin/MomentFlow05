using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TV : InteractInterface
{
    private CellPhone cellPhone;

    private new void Awake()
    {
        base.Awake();
        interact = WatchTV;
        stopInteract = WatchTV;

        cellPhone = GameObject.FindObjectOfType<CellPhone>();
    }

    private void WatchTV()
    {
        if (!isInteracted)
        {
            isInteracted = true;
            Debug.Log("dialogue로 나오겠지 \n사고로 버스가 어쩌고 저쩌고...");
            cellPhone.RingCellPhone();
        }
    }
}
