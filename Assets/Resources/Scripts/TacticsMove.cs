using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Movement logic for player and also NPC
/// </summary>
public class TacticsMove : MonoBehaviour
{
    [Header("Internal data")]
    public TileManager tileManager = null;

    /// <summary>
    /// identifier must be unique for every TacticsMove object
    /// </summary>
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
    /// destination tile for THIS if moving
    /// </summary>
    public Tile targetTile;

    /// <summary>
    /// 
    /// </summary>
    public List<Tile> path = new List<Tile>();

    /// <summary>
    /// 
    /// </summary>
    public List<Tile> pathGhost = new List<Tile>();

    /// <summary>
    /// max distance unit can move
    /// </summary>
    public int move = 8;

    /// <summary>
    /// <= move ; reset in NewTurn
    /// </summary>
    public int remainingMoves;

    /// <summary>
    /// 
    /// </summary>
    public float moveSpeed = 2.0f;

    /// <summary>
    /// 
    /// </summary>
    public float jumpVelocity = 4.5f;

    /// <summary>
    /// 
    /// </summary>
    public bool isMoving = false;

    /// <summary>
    /// distance from top of tile to center of the player
    /// </summary>
    float halfHeight = 0;

    /// <summary>
    /// up/down tile distance for jumping
    /// </summary>
    public float jumpHeight = 2;

    public bool fallingDown = false;
    public bool jumpingUp = false;
    public bool movingEdge = false;

    public Vector3 velocity = new Vector3();
    public Vector3 heading = new Vector3();
    public Vector3 jumpTarget = new Vector3();

    /// <summary>
    /// when player select THIS unit or tile on THIS unit = TRUE
    /// </summary>
    public bool unitIsSelected = false;

    [Header("Graphics")]
    public Renderer render;
    public Material material;

    private void Start()
    {
        // add to static list
        if (!TileManager.list_Units.Contains(this))
        {
            TileManager.list_Units.Add(this);
            TileManager.UnitAwake();
        }

        // INIT variables
        targetTile = null;
        unitIsSelected = false;

        // assign tileManager
        if (tileManager == null)
        {
            if ((tileManager = GameObject.Find("TileManager").GetComponent<TileManager>()) == null)
                Debug.LogError("tileManager", gameObject);
        }


        Init_start();

        // reset remaining moves to Max
        NewTurn();

        Late_Start();
    }

    /// <summary>
    /// get currentTile and add THIS unit on current tile
    /// </summary>
    public virtual void Init_start()
    {
        if(currentTile == null)
        {
            GetCurrentTile();
        }

        if(currentTile.unitsOnTile != null)
        {
            if (!currentTile.unitsOnTile.Contains(this))
            {
                currentTile.UnitsOnTile_Add(this);
            }
        }
        
        // FIX 
        // transform.position = new Vector3(currentTile.transform.position.x, halfHeight + currentTile.GetComponent<Collider>().bounds.extents.y,currentTile.transform.position.z);

    }

    /// <summary>
    /// executed during Start()
    /// </summary>
    public virtual void Late_Start(){}

    public void Update()
    {
        Update_TileMove();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(tileManager.list_publicSelectedUnits != null)
            {
                if (tileManager.list_publicSelectedUnits.Contains(this))
                {
                    NewTurn();
                }
            }
            /*
            if(TileManager.list_SelectedUnits != null)
            {
                if(TileManager.list_SelectedUnits.Contains(this))
                {
                    NewTurn();
                }
            }
            */
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [ContextMenu("01 - NewTurn")]
    public void NewTurn()
    {
        Debug.Log("<color=green> NewTurn </color>\n" + this,gameObject);
        remainingMoves = move;
    }

    /// <summary>
    /// 
    /// </summary>
    protected void Init_TacticsMove()
    {
        // halfHeight = GetComponent<Collider>().bounds.extents.y;
        halfHeight = 0;
    }

    #region BFS

    /// <summary>
    /// a.k.a selected tile
    /// </summary>
    public void GetCurrentTile()
    {
        // Debug.Log("<color=yellow> GetCurrentTile </color>\n\n", gameObject);

        currentTile = GetTargetTile(gameObject);
        currentTile.current = true;

        Debug.Log("<color=yellow> GetCurrentTile </color>\n currentTile = " + currentTile +"\n", gameObject);

    }

    public Tile GetTargetTile(GameObject target)
    {
        // Debug.Log("<color=yellow> GetTargetTile </color>\n target = " + target + "\n", gameObject);

        RaycastHit hit;
        Tile tile = null;

        if(Physics.Raycast(target.transform.position, -Vector3.up, out hit, 10))
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
        // Debug.Log("<color=yellow> ComputeAdjacencyLists </color>\n\n", gameObject);

        foreach (Tile tile in TileManager.list_Tile)
        {
            tile.Reset();
        }
    }

    public void FindSelectableTiles()
    {
        // Debug.Log("<color=yellow> FindSelectableTiles </color>\n\n",gameObject);

        ComputeAdjacencyLists();

        if(currentTile == null)
        {
            GetCurrentTile();
        }

        currentTile.walkable = true;

        Queue<Tile> process = new Queue<Tile>();

        process.Enqueue(currentTile);

        currentTile.visited = true;

        while(process.Count > 0)
        {
            Tile t = process.Dequeue();

            selectableTiles.Add(t);
            t.selectable = true;

            if(t.distance < remainingMoves)
            {
                foreach (Tile tile in t.adjacencyList)
                {
                    if (!tile.visited && tile.walkable)
                    {
                        if(TileManager.GameMode == EnumGameMode.OneByOne)
                        {
                            // if unit is on tile
                            if(tile.unitsOnTile.Count ==  0)
                            {
                                tile.parent = t;
                                tile.visited = true;
                                tile.distance = tile.tileWalkingDistance + t.distance;
                                process.Enqueue(tile);
                            }

                        }
                        else
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
    }

    public void MoveToTile(Tile tile)
    {
        Debug.Log("<color=yellow> MoveToTile </color>\n tile = " + tile + "\n", gameObject);

        path.Clear();

        // moving = true;

        Tile next = tile;

        while(next != null)
        {
            path.Add(next);
            next = next.parent;
        }

        targetTile = tile;

        // MOVE !!!
        isMoving = true;
        TileManager.someUnitIsMoving = true;
    }

    public void MoveToTileGhost(Tile tile)
    {
        pathGhost.Clear();

        TileManager.ResetTilePath();

        // moving = true;

        Tile next = tile;

        while (next != null)
        {
            pathGhost.Add(next);
            next.isPath = true;
            next = next.parent;
        }
    }


    /// <summary>
    /// Update()
    /// </summary>
    public void Update_TileMove()
    {
        if (isMoving && TileManager.someUnitIsMoving)
        {
            Move();
        }
    }

    /// <summary>
    /// use in Update or FixedUpdate ???
    /// </summary>
    public void Move()
    {
        // if (path.Count > 0)
        if (path.Count > 0)
        {
            // is moving to target tile
            //Tile t = path.Peek();
            Tile t = path[path.Count - 1];

            if (t.unitsOnTile != null)
            {
                if (!t.unitsOnTile.Contains(this))
                {
                    t.UnitsOnTile_Add(this);
                }
            }

            Vector3 target = t.transform.position;

            // calculate the units poisiton on top of target tile
            // if there can be more units on one tile, add them too
            float unitHeigth = 0;
            if(t.unitsOnTile != null)
            {
                foreach (TacticsMove units in t.unitsOnTile)
                {
                    unitHeigth += (units.GetComponent<Collider>().bounds.extents.y * 2);
                }
            }

            target.y += unitHeigth + halfHeight + t.GetComponent<Collider>().bounds.extents.y;

            if (Vector3.Distance(transform.position, target) >= 0.05f)
            {

                #region Jump
                /*
                bool jump = transform.position.y != target.y;

                if (jump)
                {
                    Jump(target);
                }
                else
                {
                    CalculateHeading(target);
                    SetHorizontalVelocity();
                }
                */
                #endregion

                CalculateHeading(target);
                SetHorizontalVelocity();

                // transform.forward = heading;
                transform.position += velocity * Time.deltaTime;
            }
            else
            {

                Debug.Log("<color=yellow> Move </color>\n Tile center reached\n Tile: " + t , gameObject);

                // tile center reached
                transform.position = target;

                currentTile = t;

                if(t.distance > 0)
                {
                    remainingMoves -= t.tileWalkingDistance;
                }

                t.UnitsOnTile_Remove(this);

                path.Remove(t);
            }
        }
        else
        {
            // target has been reached
            isMoving = false;
            TileManager.someUnitIsMoving = false;

            RemoveSelectableTiles();

            currentTile = targetTile;
            targetTile = null;

            TileManager.ResetTilePath();

            if (currentTile.unitsOnTile != null)
            {
                if (!currentTile.unitsOnTile.Contains(this))
                {
                    currentTile.UnitsOnTile_Add(this);
                }
            }

            // transform.forward = new Vector3( Mathf.Clamp(heading.x, 0, 1), 0 ,Mathf.Clamp(heading.z,0,1));

            // currentTile.walkable = false;


            TileManager.list_SelectedUnits = null;
            TileManager.selectedTile = null;

            // update map color
            TileManager.UpdateTileColor(false);

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

    private void Jump(Vector3 target)
    {
        if (fallingDown)
        {
            FallDownward(target);
        }
        else if (jumpingUp)
        {
            JumpUpward(target);
        }
        else if (movingEdge)
        {
            MoveToEdge();
        }
        else
        {
            PrepareJump(target);
        }
    }

    private void FallDownward(Vector3 target)
    {
        velocity += Physics.gravity * Time.deltaTime;

        if(transform.position.y <= target.y)
        {
            fallingDown = false;
            jumpingUp = false;
            movingEdge = false;

            Vector3 p = transform.position;
            p.y = target.y;
            transform.position = p;

            velocity = new Vector3();
        }
    }

    private void JumpUpward(Vector3 target)
    {
        velocity += Physics.gravity * Time.deltaTime;

        if(transform.position.y > target.y)
        {
            jumpingUp = false;
            fallingDown = true;
        }
    }

    /// <summary>
    /// moving to edge of tile and preparing for jump/fall
    /// </summary>
    private void MoveToEdge()
    {
        if(Vector3.Distance(transform.position,jumpTarget) >= 0.05f)
        {
            SetHorizontalVelocity();
        }
        else
        {
            movingEdge = false;
            fallingDown = true;

            velocity /= 5.0f;
            velocity.y = 1.5f;

        }
    }

    private void PrepareJump(Vector3 target)
    {
        float targetY = target.y;

        target.y = transform.position.y;

        CalculateHeading(target);

        if(transform.position.y > targetY)
        {
            fallingDown = false;
            jumpingUp = false;
            movingEdge = true;

            jumpTarget = transform.position + ((target - transform.position) / 2.0f);
        }
        else
        {
            fallingDown = false;
            jumpingUp = true;
            movingEdge = false;

            velocity = heading * moveSpeed / 3.0f;

            float difference = targetY - transform.position.y;

            velocity.y = jumpVelocity * (0.5f + difference / 2.0f);
        }

    }


    /// <summary>
    /// reset currentTile , tile.Reset and clear selectable tiles
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
        Debug.Log("<color=yellow> OnLeftMouseClick </color>\n TacticsMove " + this + "\n", gameObject);

        // check if last selected unit is moving

        if (TileManager.someUnitIsMoving)
        {
            return;
        }

        int shortestDistance =  remainingMoves;

        if (currentTile.unitsOnTile.Count == 1)
        {
            // only one unit is on tile
            TileManager.list_SelectedUnits.Clear();
            tileManager.list_publicSelectedUnits.Clear();
        }
        else
        {
            // more units are on tile, get shortest distance available for this group of units
            foreach(TacticsMove units in tileManager.list_publicSelectedUnits)
            {
                if(units.remainingMoves < shortestDistance)
                {
                    shortestDistance = units.remainingMoves;
                }
            }
        }

        // TODO: if multiple units are selected, find smallest distance to walk and apply
        FindSelectableTiles();

        // TODO: add THIS unit or all units in THIS group to tileManager.selectedUnits
        TileManager.SelectedUnit_Add(this);

        TileManager.UpdateTileColor(true);

        // TODO: do for all selected units
        unitIsSelected = true;

    }
    private void OnRightMouseClick()
    {
        Debug.Log("<color=yellow> OnRightMouseClick </color>\n TacticsMove " + this + "\n", gameObject);

        TileManager.UpdateTileColor(false);

        // TODO: make all selected units unselected
        // check if do for all or only THIS
        TileManager.list_SelectedUnits = null;

        TileManager.DeselectUnits();
    }

    #endregion
}
