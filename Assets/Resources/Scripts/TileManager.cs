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
    [Tooltip("OneByOne - player end turn after moving one unit\nAllByOne - player end turn after moving all units")]
    public EnumGameMode GameMode;

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
    public static TacticsMove selectedUnit;

    /// <summary>
    /// 
    /// </summary>
    public static List<TacticsMove> list_Units = new List<TacticsMove>();

    private void Awake()
    {
        list_Tile.Clear();
        list_Units.Clear();
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
