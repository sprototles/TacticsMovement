using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// </summary>
public enum EnumTileType_Campaign { None, Grass, Sand, Forest, Mountain, River }


public class TileTemplate_Campaign : TileTemplate
{
    public EnumTileType_Campaign enumTileType_Campaign;

    public TileManager_Campaign tileManager_Campaign;

    #region Unity Start & Awake

    public override void Init_Awake()
    {
        base.Init_Awake();

        if (tileManager_Campaign == null)
        {
            if ((tileManager_Campaign = GameObject.Find("TileManager_Campaign").GetComponent<TileManager_Campaign>()) == null)
                Debug.LogError("TileManager_Campaign", gameObject);
        }
    }

    #endregion

    #region Enum, Color

    public override void SetTileEnum()
    {
        base.SetTileEnum();
    }

    public override void SetTileColor_Enum()
    {
        base.SetTileColor_Enum();
    }

    #endregion


    #region Mouse Events

    public override void OnMouseOverTile()
    {
        base.OnMouseOverTile();
    }

    public override void OnLeftMouseClick()
    {
        base.OnLeftMouseClick();
    }


    public override void OnRightMouseClick()
    {
        base.OnRightMouseClick();
    }


    #endregion
}
