﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveManager : MonoBehaviour
{
    public LayerMask masking;

    public List<Collider> nearNpcList = new List<Collider>();
    public List<Collider> nearObjList = new List<Collider>();
    private List<Collider> nearEneList = new List<Collider>();

    // 현재 플레이어가 인터렉트를 하고 있는 지
    public bool nowInteract = false;
    // 현재 플레이어가 인터렉트 하고 있는 오브젝트
    private InteractInterface interactingObj;
    
    public void OnTriggerEnter(Collider col)
    {
        if (!TimeLayer.EqualTimeLayer(col.gameObject, gameObject))
            return;

        if (col.gameObject.layer == LayerMask.NameToLayer("NPC"))
        {
            if (!nearNpcList.Contains(col))
                nearNpcList.Add(col);
        }
        else if (col.gameObject.layer == LayerMask.NameToLayer("Object"))
        {
            if (!nearObjList.Contains(col))
                nearObjList.Add(col);
        }
        else if(col.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (!nearEneList.Contains(col))
                nearEneList.Add(col);
        }
    }

    public void OnTriggerExit(Collider col)
    {
        //TimeLayer.EqualTimeLayer();
        if (col.gameObject.layer == LayerMask.NameToLayer("NPC"))
        {
            col.gameObject.GetComponentInChildren<cakeslice.Outline>().eraseRenderer = true;
            col.GetComponent<NPC>().StopInteract();
            nearNpcList.Remove(col);
        }
        else if (col.gameObject.layer == LayerMask.NameToLayer("Object"))
        {
            var outlines = col.gameObject.GetComponentsInChildren<cakeslice.Outline>();
            for (int i = 0; i < outlines.Length; i++)
                outlines[i].eraseRenderer = true;
            nearObjList.Remove(col);
        }
        else if (col.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            nearEneList.Remove(col);
        }
    }

    void Update()
    {
        // 인터렉팅 중이 아니라면
        // 가장 가까운 오브젝트 아웃라인 활성화
        if (!nowInteract)
        {
            //Set nearest NPC Outline
            if (nearNpcList.Count >= 1)
            {
                int minIndex = FindNearestColIndex(nearNpcList);
                EnableOutLinesOnly(nearNpcList, minIndex);
            }

            //Set nearest Object Outline
            if (nearObjList.Count >= 1)
            {
                int minIndex = FindNearestColIndex(nearObjList);
                EnableOutLinesOnly(nearObjList, minIndex);
            }
        }

        //Interact Part
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (nearNpcList.Count >= 1)
            {
                int minIndex = FindNearestColIndex(nearNpcList);
                if (!nowInteract)
                {
                    nearNpcList[minIndex].GetComponent<NPC>().Interact();
                    nowInteract = true;
                }
            }

            // 
            if (nearObjList.Count >= 1)
            {
                if (nowInteract)
                {
                    interactingObj.StopInteract();
                }
                else
                {
                    int minIndex = FindNearestColIndex(nearObjList);
                    if (nearObjList[minIndex].GetComponent<InteractInterface>() != null)
                    {
                        interactingObj = nearObjList[minIndex].GetComponent<InteractInterface>();
                        nearObjList[minIndex].GetComponent<InteractInterface>().Interact();
                    }
                }
            }
        }
    }

    // 가장 가까이 있는 적의 충돌체를 찾는다
    public EnemyScript FindNearestEnemy()
    {
        if (nearEneList.Count > 0)
            return nearEneList[FindNearestColIndex(nearEneList)].GetComponent<EnemyScript>();
        else
            return null;
    }
    
    private int FindNearestColIndex(List<Collider> cols)
    {
        float minDis = float.MaxValue;
        int minIndex = -1;

        for (int i = 0; i < cols.Count; i++)
        {
            float dis = Vector3.Distance(new Vector3(transform.position.x, transform.position.y, 0),
                    new Vector3(cols[i].transform.position.x, cols[i].transform.position.y, 0));
            
            if (minDis >= dis)
            {
                minDis = dis;
                minIndex = i;
            }
        }

        return minIndex;
    }

    // cols 중에서 id와 인덱스가 같은 col에 아웃라인을 켠다
    private void EnableOutLinesOnly(List<Collider> cols, int id)
    {
        for (int i = 0; i < cols.Count; i++)
        {
            var outlines = cols[i].GetComponentsInChildren<cakeslice.Outline>();

            for (int j = 0; j < outlines.Length; j++)
            {
                outlines[j].eraseRenderer = (i == id) ? false : true;
            }
        }
    }
}