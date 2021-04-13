using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyAI
{
    void UpdateActions();

    void ToAttack();

    void ToDestroy();

    void ToFight();
}
