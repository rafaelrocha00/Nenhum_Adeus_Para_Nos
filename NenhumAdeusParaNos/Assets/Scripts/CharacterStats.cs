using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats
{
    [SerializeField] bool canFight = true;
    public bool CanFight { get { return canFight; } set { canFight = value; } }

    BattleUnit myBattleUnit;

    float life = 100;

    public CharacterStats(BattleUnit bu)
    {
        myBattleUnit = bu;
    }

    public void ReceiveDamage(float damage)
    {
        life -= damage;
        life = Mathf.Clamp(life, 0, life + damage);

        if (life == 0) Die();
        Debug.Log(life);
    }

    public void Die()
    {
        Debug.Log("died");
        canFight = false;
        GameManager.gameManager.battleController.CheckForBattleEnd();
    }
}
