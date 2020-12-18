using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsMove_Campaign : TacticsMove
{
    public override void Late_Start()
    {
        base.Late_Start();

        material.color = list_Color[playerID];
        Init_TacticsMove();
    }
}
