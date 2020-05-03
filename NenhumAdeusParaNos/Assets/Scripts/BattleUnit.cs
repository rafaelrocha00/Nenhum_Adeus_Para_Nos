using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface BattleUnit
{
    void StartBattle(bool byTrigger = true);

    void EndBattle();

    bool CanFight();

    bool IsInBattle();

    bool ReceiveDamage(float damage);

    void Die();

    Vector3 GetPos();

    Transform GetItemSpawnTransf();

    void Knockback(float dis);
}
