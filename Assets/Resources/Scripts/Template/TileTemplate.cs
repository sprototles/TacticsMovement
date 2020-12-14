using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileTemplate : MonoBehaviour
{
    public List<TacticsMove> unitsOnTile = new List<TacticsMove>();

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
    /// 
    /// </summary>
    public bool isPath = false;

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
    public List<TileTemplate> adjacencyList = new List<TileTemplate>();

    // BFS (breadth first search)

    /// <summary>
    /// true if tile was searched in BFS
    /// </summary>
    public bool visited = false;

    /// <summary>
    /// req. for generating path
    /// </summary>
    public TileTemplate parent = null;




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

    public virtual void Init_Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        Init_Start();

    }

    /// <summary>
    /// SetTileEnum, FindNeighbours
    /// </summary>
    public virtual void Init_Start()
    {

        if (!TileManager_Template.list_Tile.Contains(this))
        {
            TileManager_Template.list_Tile.Add(this);
        }

        SetTileEnum();

        SetTileColor_Enum();

        FindNeighbors(1.0f);
    }


    #region Tile color

    /// <summary>
    /// set Enum based on tile Y position
    /// </summary>
    public virtual void SetTileEnum()
    {
    }

    /// <summary>
    /// set color based on enum
    /// </summary>
    public virtual void SetTileColor_Enum()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetTileColor_Move()
    {
        if(unitsOnTile.Count > 0)
        {
            if (TileManager_Template.selectedUnit != null && TileManager_Template.selectedUnit.currentTile == this)
            {
                material.color = Color.yellow;
            }
            else
            {
                material.color = Color.cyan;
            }
        }
        else if (!walkable)
        {
            material.color = Color.grey;
        }
        else if (target || isPath)  // fix
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

        TileTemplate tile;

        foreach (Collider item in colliders)
        {
            if(( tile = item.GetComponent<TileTemplate>()) != null)
            {
                if (!adjacencyList.Contains(tile))
                {
                    adjacencyList.Add(tile);
                }
            }
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="unit"></param>
    public void UnitsOnTile_Add(TacticsMove unit)
    {
        // Debug.Log(" UnitsOnTile_Add \n 01 \n unit: " + unit, gameObject);
        unitsOnTile.Add(unit);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="unit"></param>
    public void UnitsOnTile_Remove(TacticsMove unit)
    {
        if(unitsOnTile != null)
        {
            unitsOnTile.Remove(unit);
        }
    }


    #endregion

    #region Mouse Events

    private void OnMouseEnter()
    {
        OnMouseEnterTile();
    }

    public virtual void OnMouseEnterTile(){}

    private void OnMouseExit()
    {
        OnMouseExitTile();
    }

    public virtual void OnMouseExitTile(){}

    private void OnMouseOver()
    {
        OnMouseOverTile();

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

    public virtual void OnMouseOverTile()
    {

    }

    public virtual void OnLeftMouseClick()
    {
        Debug.Log("<color=yellow> OnLeftMouseClick </color>\n Tile " + this + " \n", gameObject);
    }

    public virtual void OnRightMouseClick()
    {
        Debug.Log("<color=yellow> OnRightMouseClick </color>\n Tile " + this +" \n", gameObject);
    }

    #endregion


}
