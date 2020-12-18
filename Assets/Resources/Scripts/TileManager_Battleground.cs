using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager_Battleground : TileManager_Template
{
    public TacticsMove selectedUnit_Battleground;

    public bool someUnitIsMoving_Battleground;

    public Tile_Battleground selectedTile_Battleground;

    public List<Tile_Battleground> list_TileBattleground = new List<Tile_Battleground>();

    public override void Init_Awake()
    {
        base.Init_Awake();

        name = "TileManager_Battleground";

        list_TileBattleground.Clear();


    }

    public override void DeselectUnits()
    {
    }


}
