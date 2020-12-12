using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// OneByOne - player end turn after moving one unit
/// AllByOne - player end turn after moving all units
/// </summary>
public enum EnumGameMode { OneByOne, AllByOne };

public class TileManager : MonoBehaviour
{
    [Tooltip("OneByOne - player end turn after moving one unit, only one unit on one tile, tile2tile interaction\nAllByOne - player end turn after moving all units, multiple units on one tile")]
    public static EnumGameMode GameMode;

    /// <summary>
    /// list of all tiles presenting map
    /// </summary>
    public static List<Tile> list_Tile = new List<Tile>();

    /// <summary>
    /// target tile for selected unit
    /// </summary>
    public static Tile selectedTile;

    /// <summary>
    /// selected unit for movement
    /// </summary>
    public static List<TacticsMove> list_SelectedUnits =  new List<TacticsMove>();

    /// <summary>
    /// 
    /// </summary>
    public List<TacticsMove> list_publicSelectedUnits = new List<TacticsMove>();

    /// <summary>
    /// 
    /// </summary>
    public static bool someUnitIsMoving = false;

    /// <summary>
    /// 
    /// </summary>
    public static List<TacticsMove> list_Units = new List<TacticsMove>();

    private void Awake()
    {
        GameMode = EnumGameMode.AllByOne;

        list_Tile.Clear();
        list_Units.Clear();
    }


    private void Start()
    {
        Debug.Log("<color=green> TileManager \n Start()  </color>\n GameMode = " + GameMode,gameObject);
    }

    public static void UnitAwake()
    {

    }

    /// <summary>
    /// foreach Tile isPath = false
    /// </summary>
    public static void ResetTilePath()
    {
        foreach (Tile tile in list_Tile)
        {
            tile.isPath = false;
        }
    }

    public static void DeselectUnits()
    {
        foreach(TacticsMove unit in list_Units)
        {
            unit.unitIsSelected = false;
        }
    }


    /// <summary>
    /// TRUE = movement, FALSE = enum color
    /// </summary>
    /// <param name="status"></param>
    public static void UpdateTileColor(bool status)
    {
        foreach (Tile tile in list_Tile)
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="unit"></param>
    public static void SelectedUnit_Add(TacticsMove unit)
    {
        list_SelectedUnits.Add(unit);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="unit"></param>
    public static void SelectedUnit_Remove(TacticsMove unit)
    {
        list_SelectedUnits.Remove(unit);
    }

    #region Editor

    [ContextMenu("01 - UpdateTile")]
    public void UpdateTile()
    {
        GameObject[] arrayGo = GameObject.FindGameObjectsWithTag("Tile");

        foreach(GameObject go in arrayGo)
        {
            go.GetComponent<Tile>().SetTileEnum();
        }
    }
    #endregion

}
