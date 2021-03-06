﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : Bullet
{
    void Start()
    {
        bc = GetComponent<Collider>();
        ps = GetComponent<ParticleSystem>();
        if (ps == null)
            ps = transform.GetComponentInChildren<ParticleSystem>();
    }
    
    void Update()
    {
        RaycastHorizontal();
        RaycastVertical();
        Move();
    }

    public void DestroyBullet()
    {
        bc.enabled = false;
        ps.Stop();
    }

    protected override void ProcessCollide(GameObject obj)
    {
        if (obj.layer == LayerMask.NameToLayer("Player"))
        {
            damage = 1000;
            if (transform.position.x < obj.transform.position.x)
                obj.transform.GetComponent<Player>().Damaged(damage, true);
            else
                obj.transform.GetComponent<Player>().Damaged(damage, false);
            DestroyBullet();
        }
    }
}