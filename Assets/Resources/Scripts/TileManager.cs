using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    /// <summary>
    /// list of all tiles presenting map
    /// </summary>
    public static List<Tile> list_Tile = new List<Tile>();

    private void Awake()
    {
        list_Tile.Clear();
    }

}
