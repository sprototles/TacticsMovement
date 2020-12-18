using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public enum EnumTileType_Battleground { None, Grass, Sand, Tree, Wall, Swamp, Stone }

public class Tile_Battleground : Tile_Template
{
    public EnumTileType_Battleground enumTileType_Battleground;

    public TileManager_Battleground tileManager_Battleground;

    #region Unity Start & Awake

    public override void Init_Awake()
    {
        if(!CompareTag("Tile_Battleground"))
        {
            tag = "Tile_Battleground";
        }

        if (tileManager_Battleground == null)
        {
            if ((tileManager_Battleground = GameObject.Find("TileManager_Battleground").GetComponent<TileManager_Battleground>()) == null)
                Debug.LogError("TileManager_Battleground", gameObject);
        }

    }

    public override void Init_Start()
    {
        if (!tileManager_Battleground.list_TileBattleground.Contains(this))
        {
            tileManager_Battleground.list_TileBattleground.Add(this);
        }

        base.Init_Start();
    }

    #endregion

    #region Enum, Color

    /// <summary>
    /// randomly generate Enum
    /// </summary>
    public override void SetTileEnum()
    {
        if (transform.localPosition.y < -0.2)
        {
            enumTileType_Battleground = EnumTileType_Battleground.Swamp;
            walkable = false;
            tileWalkingDistance = 99;
        }
        else if (transform.localPosition.y >= -0.2 && transform.localPosition.y < 0)
        {
            enumTileType_Battleground = EnumTileType_Battleground.Sand;
            walkable = true;
            tileWalkingDistance = 2;
        }
        else if (transform.localPosition.y >= 0 && transform.localPosition.y <= 0.5)
        {
            enumTileType_Battleground = EnumTileType_Battleground.Grass;
            walkable = true;
            tileWalkingDistance = 1;
        }
        else if (transform.localPosition.y > 0.5 && transform.localPosition.y < 1.5)
        {
            enumTileType_Battleground = EnumTileType_Battleground.Tree;
            walkable = true;
            tileWalkingDistance = 3;
        }
        else if (transform.localPosition.y >= 1.5)
        {
            enumTileType_Battleground = EnumTileType_Battleground.Wall;
            walkable = true;
            tileWalkingDistance = 10;
        }
        else
        {
            enumTileType_Battleground = EnumTileType_Battleground.None;
            walkable = false;
            tileWalkingDistance = 99;
        }
    }

    public override void SetTileColor_Enum()
    {
        if (material == null)
        {
            render = GetComponent<Renderer>();
            material = render.material;
        }

        switch (enumTileType_Battleground)
        {
            case EnumTileType_Battleground.Swamp:
                material.color = Color.blue;
                break;
            case EnumTileType_Battleground.Grass:
                material.color = Color.green;
                break;
            case EnumTileType_Battleground.Tree:
                material.color = new Color(0.125f, 0.5f, 0.2f, 1);
                break;
            case EnumTileType_Battleground.Wall:
                material.color = Color.grey;
                break;
            case EnumTileType_Battleground.Sand:
                material.color = Color.yellow;
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

        if (tileManager_Battleground.selectedUnit_Battleground != null)
        {
            if (!tileManager_Battleground.someUnitIsMoving_Battleground)
            {
                tileManager_Battleground.UpdateTileColor(true);

                // pick only 
                tileManager_Battleground.selectedUnit_Battleground.MoveToTileGhost(this);
            }
        }

    }

    public override void OnLeftMouseClick()
    {
        base.OnLeftMouseClick();

        // check if some unit was selected 
        if (tileManager_Battleground.selectedUnit_Battleground != null)
        {
            if (!tileManager_Battleground.someUnitIsMoving_Battleground)
            {
                // check if some tile was selected before and reset its status
                if (tileManager_Battleground.selectedTile_Battleground != null)
                {
                    // check if selected tile is THIS or not
                    if (tileManager_Battleground.selectedTile_Battleground != this)
                    {
                        tileManager_Battleground.selectedTile_Battleground.target = false;
                    }
                }

                tileManager_Battleground.selectedTile_Battleground = this;

                // can unit move to this tile ?
                if (selectable)
                {
                    target = true;

                    tileManager_Battleground.UpdateTileColor(true);

                    // selected unit move to this tile

                    tileManager_Battleground.selectedUnit_Battleground.MoveToTile(this);

                }
            }

        }
        else
        {
            tileManager_Battleground.selectedTile_Battleground = this;
        }
    }


    public override void OnRightMouseClick()
    {
        base.OnRightMouseClick();


        // TileManager.selectedUnit = null;
        tileManager_Battleground.selectedUnit_Battleground = null;

        tileManager_Battleground.selectedTile_Battleground = null;
        tileManager_Battleground.UpdateTileColor(false);

        // all units are deselected
        tileManager_Battleground.DeselectUnits();
    }


    #endregion
}
