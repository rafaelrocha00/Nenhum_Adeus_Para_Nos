using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface BattleUnit
{
    void StartBattle(bool byTrigger = false);

    void EndBattle();

    bool CanFight();

    bool IsInBattle();

    void ReceiveDamage(float damage);

    void Die();
}
