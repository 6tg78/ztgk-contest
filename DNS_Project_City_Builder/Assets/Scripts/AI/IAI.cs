using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IAI
{
    void UpdateActions();

    void ToIdle();

    //void ToEscape();

}
