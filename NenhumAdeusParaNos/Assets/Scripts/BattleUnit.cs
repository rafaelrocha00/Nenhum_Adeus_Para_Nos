using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface BattleUnit
{
    void StartBattle();

    void EndBattle();

    bool CanFight();
}
