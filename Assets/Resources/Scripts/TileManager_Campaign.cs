using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager_Campaign : TileManager_Template
{
    public List<TacticsMove> selectedUnits_Campaign = new List<TacticsMove>();

    public bool someUnitIsMoving_Battleground;

    public Tile_Campaign selectedTile_Campaign;

    public List<Tile_Campaign> list_TileCampaign = new List<Tile_Campaign>();



    public override void Init_Awake()
    {
        base.Init_Awake();

        name = "TileManager_Campaign";

        list_TileCampaign.Clear();
    }


}
