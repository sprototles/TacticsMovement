using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : TacticsMove
{



    private void Awake()
    {
        render = GetComponent<Renderer>();
        material = render.material;
    }

    public override void Late_Start()
    {
        base.Late_Start();

        material.color = list_Color[playerID];
        Init_TacticsMove();
    }
}
