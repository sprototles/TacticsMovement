using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileManager_Template : MonoBehaviour
{
    /// <summary>
    /// list of all tiles presenting map
    /// </summary>
    public static List<Tile_Template> list_Tile = new List<Tile_Template>();

    /// <summary>
    /// target tile for selected unit
    /// </summary>
    public static Tile_Template selectedTile;

    /// <summary>
    /// selected unit for movement
    /// </summary>
    public static TacticsMove selectedUnit;

    /// <summary>
    /// 
    /// </summary>
    public static bool someUnitIsMoving = false;

    /// <summary>
    /// list of all units on this map fighting together
    /// </summary>
    public static List<TacticsMove> list_Units = new List<TacticsMove>();

    private void Awake()
    {
        list_Tile.Clear();
        list_Units.Clear();
        Init_Awake();
    }

    public virtual void Init_Awake()
    {

    }


    private void Start()
    {
        Debug.Log("<color=green> TileManager \n Start()  </color>\n ",gameObject);
        Init_Start();
    }

    public virtual void Init_Start()
    {

    }

    public static void UnitAwake()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(" " + this + "\ntile_List + " + list_Tile.Count + "\n",gameObject);
        }
    }

    /// <summary>
    /// foreach Tile isPath = false
    /// </summary>
    public static void ResetTilePath()
    {
        foreach (Tile_Template tile in list_Tile)
        {
            tile.isPath = false;
        }
    }

    public virtual void DeselectUnits()
    {
        foreach(TacticsMove unit in list_Units)
        {
            unit.unitIsSelected = false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="unit"></param>
    public static void SelectedUnit(TacticsMove unit)
    {
        selectedUnit = unit;
    }

    /// <summary>
    /// TRUE = movement, FALSE = enum color
    /// </summary>
    /// <param name="status"></param>
    public void UpdateTileColor(bool status)
    {
        foreach (Tile_Template tile in list_Tile)
        {
            if (status)
            {
                tile.SetTileColor_Move();
            }
            else
            {
                tile.SetTileColor_Enum();
            }
        }
    }


    #region Editor

    /// <summary>
    /// split for Tile_Campaign adn Tile_Battleground
    /// </summary>
    [ContextMenu("01 - UpdateTile")]
    public void UpdateTile()
    {
        GameObject[] arrayGo = GameObject.FindGameObjectsWithTag("Tile");

        foreach(GameObject go in arrayGo)
        {
            go.GetComponent<Tile_Template>().SetTileEnum();
            go.GetComponent<Tile_Template>().SetTileColor_Enum();
        }
    }
    #endregion

}
