using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public enum EnumTileType_Battleground { None, Grass, Sand, Tree, Wall, Swamp, Stone }

public class TileTemplate_Battleground : TileTemplate
{

    public EnumTileType_Battleground enumTileType_Battleground;

    public TileManager_Battleground tileManager_Battleground;

    #region Unity Start & Awake

    public override void Init_Awake()
    {
        base.Init_Awake();

        if (tileManager_Battleground == null)
        {
            if ((tileManager_Battleground = GameObject.Find("TileManager_Battleground").GetComponent<TileManager_Battleground>()) == null)
                Debug.LogError("tileManager_Battleground", gameObject);
        }


    }

    #endregion

    #region Enum, Color

    /// <summary>
    /// randomly generate Enum
    /// </summary>
    public override void SetTileEnum()
    {
        base.SetTileEnum();

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
        base.SetTileColor_Enum();


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

        if (TileManager_Battleground.selectedUnit != null)
        {
            if (!TileManager_Battleground.someUnitIsMoving)
            {
                TileManager_Battleground.UpdateTileColor(true);

                // pick only 
                TileManager_Battleground.selectedUnit.MoveToTileGhost(this);
            }
        }

    }

    public override void OnLeftMouseClick()
    {
        base.OnLeftMouseClick();

        // check if some unit was selected 
        if (TileManager_Battleground.selectedUnit != null)
        {
            if (!TileManager_Battleground.someUnitIsMoving)
            {
                // check if some tile was selected before and reset its status
                if (TileManager_Battleground.selectedTile != null)
                {
                    // check if selected tile is THIS or not
                    if (TileManager_Battleground.selectedTile != this)
                    {
                        TileManager_Battleground.selectedTile.target = false;
                    }
                }

                TileManager_Battleground.selectedTile = this;

                // can unit move to this tile ?
                if (selectable)
                {
                    target = true;

                    TileManager_Battleground.UpdateTileColor(true);

                    // selected unit move to this tile

                    TileManager_Battleground.selectedUnit.MoveToTile(this);

                }
            }

        }
    }


    public override void OnRightMouseClick()
    {
        base.OnRightMouseClick();


        // TileManager.selectedUnit = null;
        TileManager_Battleground.SelectedUnit(null);

        TileManager_Battleground.selectedTile = null;
        TileManager_Battleground.UpdateTileColor(false);

        // all units are deselected
        TileManager_Battleground.DeselectUnits();
    }


    #endregion
}
