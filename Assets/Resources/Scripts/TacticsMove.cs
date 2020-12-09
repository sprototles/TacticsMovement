using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Movement logic for player and also NPC
/// </summary>
public class TacticsMove : MonoBehaviour
{
    /// <summary>
    /// identifier must be unique for every TacticsMove object
    /// </summary>
    [Header("Internal data")]
    public int identifier;

    internal readonly List<Color> list_Color = new List<Color> {Color.yellow, Color.cyan, Color.red, Color.green, Color.blue };

    /// <summary>
    /// units with same ID are controlled with same player
    /// </summary>
    public int playerID;

    /// <summary>
    /// list of tiles accesible to THIS object
    /// </summary>
    [Header("Movement")]
    public List<Tile> selectableTiles = new List<Tile>();

    /// <summary>
    /// tile under THIS unit
    /// </summary>
    public Tile currentTile;

    /// <summary>
    /// path of tiles from current to target
    /// </summary>
    public Stack<Tile> path = new Stack<Tile>();

    /// <summary>
    /// 
    /// </summary>
    public int move = 5;

    public float moveSpeed = 2.0f;

    public bool isMoving = false;

    /// <summary>
    /// distance from top of tile to center of the player
    /// </summary>
    float halfHeight = 0;

    /// <summary>
    /// up/down tile distance for jumping
    /// </summary>
    public float jumpHeight = 2;

    Vector3 velocity = new Vector3();
    Vector3 heading = new Vector3();


    [Header("Graphics")]
    public Renderer render;
    public Material material;

    private void Start()
    {
        if (!TileManager.list_Units.Contains(this))
        {
            TileManager.list_Units.Add(this);
        }

        Late_Start();
    }

    /// <summary>
    /// executed during Start()
    /// </summary>
    public virtual void Late_Start(){}

    public void Update()
    {
        Update_TileMove();
    }

    /// <summary>
    /// 
    /// </summary>
    protected void Init_TacticsMove()
    {
        halfHeight = GetComponent<Collider>().bounds.extents.y;
    }

    #region BFS

    /// <summary>
    /// a.k.a selected tile
    /// </summary>
    public void GetCurrentTile()
    {
        Debug.Log("<color=yellow> GetCurrentTile </color>\n\n", gameObject);

        currentTile = GetTargetTile(gameObject);
        currentTile.current = true;

        Debug.Log("<color=yellow> GetCurrentTile </color>\n currentTile = " + currentTile +"\n", gameObject);

    }

    public Tile GetTargetTile(GameObject target)
    {
        // Debug.Log("<color=yellow> GetTargetTile </color>\n target = " + target + "\n", gameObject);

        RaycastHit hit;
        Tile tile = null;

        if(Physics.Raycast(target.transform.position, -Vector3.up, out hit, 3))
        {
            tile = hit.collider.GetComponent<Tile>();
        }

        return tile;
    }

    /// <summary>
    /// obsolete, already done
    /// </summary>
    public void ComputeAdjacencyLists()
    {
        Debug.Log("<color=yellow> ComputeAdjacencyLists </color>\n\n", gameObject);

        foreach (Tile tile in TileManager.list_Tile)
        {
            tile.Reset();
        }
    }

    public void FindSelectableTiles()
    {
        Debug.Log("<color=yellow> FindSelectableTiles </color>\n\n",gameObject);

        ComputeAdjacencyLists();
        GetCurrentTile();

        Queue<Tile> process = new Queue<Tile>();

        process.Enqueue(currentTile);

        currentTile.visited = true;

        while(process.Count > 0)
        {
            Tile t = process.Dequeue();

            selectableTiles.Add(t);
            t.selectable = true;

            if(t.distance < move)
            {
                foreach (Tile tile in t.adjacencyList)
                {
                    if (!tile.visited && tile.walkable)
                    {
                        tile.parent = t;
                        tile.visited = true;
                        tile.distance = tile.tileWalkingDistance + t.distance;
                        process.Enqueue(tile);
                    }
                }
            }
        }
    }

    public void MoveToTile(Tile tile)
    {
        Debug.Log("<color=yellow> GetTargetTile </color>\n tile = " + tile + "\n", gameObject);

        path.Clear();

        // moving = true;

        Tile next = tile;

        while(next != null)
        {
            path.Push(next);
            next = next.parent;
        }

        // MOVE !!!
        isMoving = true;

    }

    /// <summary>
    /// 
    /// </summary>
    public void Update_TileMove()
    {
        if (isMoving)
        {
            Move();
        }
    }

    /// <summary>
    /// use in Update or FixedUpdate ???
    /// </summary>
    public void Move()
    {
        if(path.Count > 0)
        {
            // is moving to target tile
            Tile t = path.Peek();

            Vector3 target = t.transform.position;

            // calculate the units poisiton on top of target tile
            target.y += halfHeight + t.GetComponent<Collider>().bounds.extents.y;

            if (Vector3.Distance(transform.position, target) >= 0.05f)
            {
                CalculateHeading(target);
                SetHorizontalVelocity();

                transform.forward = heading;
                transform.position += velocity * Time.deltaTime;
            }
            else
            {
                // tile center reached
                transform.position = target;
                path.Pop();
            }

        }
        else
        {
            // target has been reached
            // moving = false;

            RemoveSelectableTiles();


        }
    }

    private void CalculateHeading(Vector3 target)
    {
        heading = target - transform.position;
        heading.Normalize();
    }

    private void SetHorizontalVelocity()
    {
        velocity = heading * moveSpeed;
    }


    /// <summary>
    /// 
    /// </summary>
    protected void RemoveSelectableTiles()
    {
        if(currentTile != null)
        {
            currentTile.current = false;
            currentTile = null;
        }

        foreach(Tile tile in selectableTiles)
        {
            tile.Reset();
        }

        selectableTiles.Clear();
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
        FindSelectableTiles();
        TileManager.UpdateTileColor(true);
        TileManager.selectedUnit = this;

    }
    private void OnRightMouseClick()
    {
        TileManager.UpdateTileColor(false);
        TileManager.selectedUnit = null;
    }

    #endregion
}
