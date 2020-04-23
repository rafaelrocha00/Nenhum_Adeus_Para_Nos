using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour, BattleUnit
{
    float life = 100;

    private void Update()
    {
        life -= Time.deltaTime * 20;
        life = Mathf.Clamp(life, 0, 100);
        if (life == 0) Die();
        Debug.Log(life);
    }

    public bool CanFight()
    {
        return false;
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }

    public void EndBattle()
    {
        
    }

    public Vector3 GetPos()
    {
        return transform.position;
    }

    public bool IsInBattle()
    {
        return true;
    }

    public bool ReceiveDamage(float damage)
    {
        life -= damage;
        return false;  
    }

    public void StartBattle(bool byTrigger = true)
    {
        
    }
}
