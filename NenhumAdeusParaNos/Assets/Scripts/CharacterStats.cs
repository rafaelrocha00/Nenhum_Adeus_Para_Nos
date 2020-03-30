using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats
{
    [SerializeField] bool canFight = true;
    public bool CanFight { get { return canFight; } set { canFight = value; } }

    BattleUnit myBattleUnit;

    [SerializeField] float maxLife = 100;
    public float MaxLife { get { return maxLife; } set { maxLife = value; } }
    [SerializeField] float life;
    public float Life { get { return Life; } set { Life = value; } }

    public CharacterStats(BattleUnit bu)
    {
        myBattleUnit = bu;
        life = maxLife;
    }

    public void ReceiveDamage(float damage)
    {

        if (myBattleUnit is Player)
        {
            Player aux = (Player)myBattleUnit;
            if (aux.Dashing) return;
            else if (aux.Defending)
            {
                damage -= damage * aux.Defense_Strength / 100;
                aux.UpdateDefense(-(damage * aux.Defense_Strength / 100));
            }
        }
        else if (!GameManager.gameManager.battleController.ActiveBattle)
        {
            INPC aux = (INPC)myBattleUnit;
            GameManager.gameManager.battleController.TriggerHostileNPCs(aux.transform.position);
            return;
        }
        //life -= damage;
        //life = Mathf.Clamp(life, 0, maxLife);

        //if (life == 0) Die();
        //Debug.Log(life);
        life -= damage;
        life = Mathf.Clamp(life, 0, maxLife);
        if (life == 0) Die();
        Debug.Log(life);
    }

    public float LifePercentage()
    {
        return life / maxLife;
    }

    public void Die()
    {
        Debug.Log("died");
        canFight = false;
        myBattleUnit.Die();
        GameManager.gameManager.battleController.CheckForBattleEnd();
    }

    //public bool Alive()
    //{
    //    if (life > 0) return true;
    //    else return false;
    //}
}
