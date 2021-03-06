﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 15.0f;
    public float lifeTime = 2.5f;

    Vector3 shotDirection;
    protected float damage;

    private void Start()
    {
        shotDirection = transform.forward;
        Invoke("OnLifeTimeOver", lifeTime);
    }

    public void InitialSet(/*Vector3 dir, */float dmg, int lay)
    {
        //shotDirection = dir;
        damage = dmg;
        gameObject.layer = lay;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            OnContact(other);
            //if (other.GetComponent<Player>() != null) other.GetComponent<Player>().CharStats.ReceiveDamage(damage);
            //else if (other.GetComponent<INPC>() != null) other.GetComponent<INPC>().CharStats.ReceiveDamage(damage);
            //bool aux = false;
            //try
            //{
            //    //if (other.GetComponent<BattleUnit>().IsInBattle())
            //    //{
            //    aux = other.GetComponent<BattleUnit>().ReceiveDamage(damage);
            //    //}
            //}
            //catch
            //{
            //}
            ///*if (!other.isTrigger) */
            //if (!aux) Destroy(this.gameObject);
        }
    }

    protected virtual void OnContact(Collider col)
    {
        bool aux = false;
        try
        {
            //if (other.GetComponent<BattleUnit>().IsInBattle())
            //{
            aux = col.GetComponent<BattleUnit>().ReceiveDamage(damage);
            //}
        }
        catch
        {
        }
        /*if (!other.isTrigger) */
        if (!aux) Destroy(this.gameObject);
    }

    protected virtual void OnLifeTimeOver()
    {
        Destroy(this.gameObject);
    }

    private void Update()
    {
        transform.position += shotDirection.normalized * speed * Time.deltaTime;
    }
}
