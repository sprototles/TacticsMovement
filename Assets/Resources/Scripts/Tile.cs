using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public enum EnumTileType { None, Grass, Sand, Forest, Mountain, River}

public class Tile : MonoBehaviour
{
    public EnumTileType enumTileType;

    #region Movement

    /// <summary>
    /// can unit walk on THIS tile ?
    /// </summary>
    public bool walkable;

    /// <summary>
    /// tile under selected player/npc
    /// </summary>
    public bool current = false;

    /// <summary>
    /// tile selected as target of selected moving unit
    /// </summary>
    public bool target = false;

    /// <summary>
    /// every tile player can select as target
    /// </summary>
    public bool selectable = false;

    /// <summary>
    /// [tileWalkingDistance] how much distance will it take to cross THIS tile
    /// </summary>
    public int tileWalkingDistance;

    /// <summary>
    /// distance from current tile to THIS tile
    /// </summary>
    public int distance = 0;

    /// <summary>
    /// list of (side, not edge) neighour tiles to THIS tile
    /// </summary>
    public List<Tile> adjacencyList = new List<Tile>();

    // BFS (breadth first search)

    /// <summary>
    /// true if tile was searched in BFS
    /// </summary>
    public bool visited = false;

    /// <summary>
    /// req. for generating path
    /// </summary>
    public Tile parent = null;




    #endregion

    [Header("Graphics")]
    public Renderer render;
    public Material material;


    private void Awake()
    {
        // set name for easier navigation in hierarchy
        name = "Tile_[" + Mathf.RoundToInt(transform.localPosition.x) + "," + Mathf.RoundToInt(transform.localPosition.z) + "]";

        if (render == null)
        {
            if ((render = GetComponent<Renderer>()) == null)
                Debug.LogError("render", gameObject);
        }

        if (material == null)
        {
            if ((material = render.material) == null)
                Debug.LogError("material", gameObject);
        }

        // clear list
        adjacencyList.Clear();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!TileManager.list_Tile.Contains(this))
        {
            TileManager.list_Tile.Add(this);
        }

        SetTileEnum();

        FindNeighbors(1.0f);
    }


    #region Tile color

    /// <summary>
    /// set Enum based on tile Y position
    /// </summary>
    public void SetTileEnum()
    {
        if (transform.localPosition.y < -0.2)
        {
            enumTileType = EnumTileType.River;
            walkable = false;
            tileWalkingDistance = 99;
        }
        else if (transform.localPosition.y >= -0.2 && transform.localPosition.y < 0)
        {
            enumTileType = EnumTileType.Sand;
            walkable = true;
            tileWalkingDistance = 2;
        }
        else if (transform.localPosition.y >= 0 && transform.localPosition.y <= 0.5)
        {
            enumTileType = EnumTileType.Grass;
            walkable = true;
            tileWalkingDistance = 1;
        }
        else if (transform.localPosition.y > 0.5 && transform.localPosition.y < 1.5)
        {
            enumTileType = EnumTileType.Forest;
            walkable = true;
            tileWalkingDistance = 3;
        }
        else if (transform.localPosition.y >= 1.5)
        {
            enumTileType = EnumTileType.Mountain;
            walkable = true;
            tileWalkingDistance = 10;
        }
        else
        {
            enumTileType = EnumTileType.None;
            walkable = false;
            tileWalkingDistance = 99;
        }

        SetTileColor_Enum();
    }

    /// <summary>
    /// set color based on enum
    /// </summary>
    public void SetTileColor_Enum()
    {
        
        if(material == null)
        {
            render = GetComponent<Renderer>();
            material = render.material;
        }
        
        switch (enumTileType)
        {
            case EnumTileType.River:
                material.color = Color.blue;
                break;
            case EnumTileType.Grass:
                material.color = Color.green;
                break;
            case EnumTileType.Forest:
                material.color = new Color(0.125f,0.5f,0.2f,1);
                break;
            case EnumTileType.Mountain:
                material.color = Color.grey;
                break;
            case EnumTileType.Sand:
                material.color = Color.yellow;
                break;
            default:
                walkable = false;
                tileWalkingDistance = 99;
                material.color = Color.white;
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetTileColor_Move()
    {
        if (!walkable)
        {
            material.color = Color.grey;
        }
        else if (target)
        {
            material.color = Color.green;
        }
        else if (selectable)
        {
            material.color = Color.red;
        }
        else if (current)
        {
            material.color = Color.magenta;
        }
        else
        {
            material.color = Color.white;
        }
    }

    #endregion

    #region Unit movement

    /// <summary>
    /// 
    /// </summary>
    public void Reset()
    {
        current = false;
        target = false;
        selectable = false;

        visited = false;
        parent = null;
        distance = 0;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jumpHeight">how high can unit jump and if its reachable</param>
    public void FindNeighbors(float jumpHeight)
    {
        Reset();

        CheckTile(Vector3.forward, jumpHeight);
        CheckTile(-Vector3.forward, jumpHeight);
        CheckTile(Vector3.right, jumpHeight);
        CheckTile(-Vector3.right, jumpHeight);
    }

    public void CheckTile(Vector3 direction,float jumpHeight)
    {
        Vector3 halfExtends = new Vector3(0.25f, (1 + jumpHeight) / 2.0f ,0.25f);

        Collider[] colliders = Physics.OverlapBox(transform.position + direction,halfExtends);

        Tile tile;

        foreach (Collider item in colliders)
        {
            if(( tile = item.GetComponent<Tile>()) != null)
            {
                if (!adjacencyList.Contains(tile))
                {
                    adjacencyList.Add(tile);
                }
            }
        }

    }


    #endregion

    #region Mouse Events

    private void OnMouseOver()
    {
        // Left mouse button
        if (Input.GetMouseButtonDown(0))
        {
            OnLeftMouseClick();
        }

        // Right mouse button
        if (Input.GetMouseButtonDown(1))
        {
            OnRightMouseClick();
        }
    }

    private void OnLeftMouseClick()
    {
        Debug.Log("<color=yellow> OnLeftMouseClick </color>\n Tile " + this + " \n", gameObject);

        // check if some unit was selected 
        if (TileManager.selectedUnit != null)
        {
            if (!TileManager.selectedUnit.isMoving)
            {
                // check if some tile was selected before and reset its status
                if (TileManager.selectedTile != null)
                {
                    // check if selected tile is THIS or not
                    if (TileManager.selectedTile != this)
                    {
                        TileManager.selectedTile.target = false;
                    }
                }

                TileManager.selectedTile = this;

                // can unit move to this tile ?
                if (selectable)
                {
                    target = true;

                    TileManager.UpdateTileColor(true);

                    // selected unit move to this tile

                    TileManager.selectedUnit.MoveToTile(this);

                }
            }

        }

    }

    private void OnRightMouseClick()
    {
        Debug.Log("<color=yellow> OnRightMouseClick </color>\n Tile " + this +" \n", gameObject);

        TileManager.selectedUnit = null;
        TileManager.selectedTile = null;
        TileManager.UpdateTileColor(false);
    }

    #endregion


}
