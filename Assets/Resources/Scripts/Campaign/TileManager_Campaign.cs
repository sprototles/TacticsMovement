using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager_Campaign : TileManager_Template
{
    public static List<TacticsMove> list_selectedUnits = new List<TacticsMove>();


    public override void Init_Awake()
    {
        base.Init_Awake();
        name = "TileManager_Campaign";
    }


}
