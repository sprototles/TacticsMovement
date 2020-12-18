using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// </summary>
public enum EnumTileType_Campaign { None, Grass, Sand, Forest, City, Mountain, River }


public class Tile_Campaign : Tile_Template
{
    public EnumTileType_Campaign enumTileType_Campaign;

    public TileManager_Campaign tileManager_Campaign;

    #region Unity Start & Awake

    public override void Init_Awake()
    {
        if (!CompareTag("Tile_Campaign"))
        {
            tag = "Tile_Campaign";
        }

        if (tileManager_Campaign == null)
        {
            if ((tileManager_Campaign = GameObject.Find("TileManager_Campaign").GetComponent<TileManager_Campaign>()) == null)
                Debug.LogError("TileManager_Campaign", gameObject);
        }

    }

    public override void Init_Start()
    {

        if (!tileManager_Campaign.list_TileCampaign.Contains(this))
        {
            tileManager_Campaign.list_TileCampaign.Add(this);
        }
        base.Init_Start();
    }

    #endregion

    #region Enum, Color

    public override void SetTileEnum()
    {

    }

    public override void SetTileColor_Enum()
    {
        if (material == null)
        {
            render = GetComponent<Renderer>();
            material = render.material;
        }

        switch (enumTileType_Campaign)
        {
            case EnumTileType_Campaign.River:
                material.color = Color.blue;
                walkable = false;
                tileWalkingDistance = 99;
                break;
            case EnumTileType_Campaign.Grass:
                material.color = Color.green;
                walkable = true;
                tileWalkingDistance = 1;
                break;
            case EnumTileType_Campaign.Forest:
                material.color = new Color(0.125f, 0.5f, 0.2f, 1);
                walkable = true;
                tileWalkingDistance = 2;
                break;
            case EnumTileType_Campaign.Mountain:
                material.color = Color.grey;
                walkable = false;
                tileWalkingDistance = 99;
                break;
            case EnumTileType_Campaign.City:
                material.color = Color.yellow;
                walkable = true;
                tileWalkingDistance = 1;
                break;
            case EnumTileType_Campaign.Sand:
                material.color = Color.red;
                walkable = true;
                tileWalkingDistance = 3;
                break;
            default:
                walkable = false;
                tileWalkingDistance = 99;
                material.color = Color.white;
                break;
        }
    }

    #endregion

    #region Mouse Events

    public override void OnMouseOverTile()
    {
        base.OnMouseOverTile();

        // check if some unit/units is selected
        if (tileManager_Campaign.selectedUnits_Campaign != null)
        {
            // check if its not moving
            if (!tileManager_Campaign.someUnitIsMoving_Battleground)
            {
                // some unit is selected and ready to move
                tileManager_Campaign.UpdateTileColor(true);

                // pick one with smallest range
                // tileManager_Campaign.selectedUnit_Battleground.MoveToTileGhost(this);
            }
        }

    }

    public override void OnLeftMouseClick()
    {
        base.OnLeftMouseClick();

        // check if some unit was selected 
        if (tileManager_Campaign.selectedUnits_Campaign != null)
        {
            if (!tileManager_Campaign.someUnitIsMoving_Battleground)
            {
                // check if some tile was selected before and reset its status
                if (tileManager_Campaign.selectedTile_Campaign != null)
                {
                    // check if selected tile is THIS or not
                    if (tileManager_Campaign.selectedTile_Campaign != this)
                    {
                        tileManager_Campaign.selectedTile_Campaign.target = false;
                    }
                }

                tileManager_Campaign.selectedTile_Campaign = this;

                // can unit move to this tile ?
                if (selectable)
                {
                    target = true;

                    tileManager_Campaign.UpdateTileColor(true);

                    // selected unit move to this tile
                    // find one with smallest range
                    // tileManager_Campaign.selectedUnit_Battleground.MoveToTile(this);

                }
            }

        }
        else
        {
            tileManager_Campaign.selectedTile_Campaign = this;
        }
    }


    public override void OnRightMouseClick()
    {
        base.OnRightMouseClick();

        // TileManager.selectedUnit = null;
        tileManager_Campaign.selectedUnits_Campaign = null;

        tileManager_Campaign.selectedTile_Campaign = null;
        tileManager_Campaign.UpdateTileColor(false);

        // all units are deselected
        tileManager_Campaign.DeselectUnits();
    }


    #endregion
}
