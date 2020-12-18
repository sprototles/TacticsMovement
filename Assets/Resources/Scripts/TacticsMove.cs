using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Movement logic for player and also NPC
/// </summary>
public class TacticsMove : MonoBehaviour
{
    [Header("Internal data")]
    public TileManager_Battleground tileManager_Battleground = null;
    public TileManager_Campaign tileManager_Campaign = null;

    /// <summary>
    /// identifier must be unique for every TacticsMove object
    /// </summary>
    public int identifier;

    internal readonly List<Color> list_Color = new List<Color> {Color.yellow, Color.cyan, Color.red, Color.green, Color.blue };

    /// <summary>
    /// units with same ID are controlled with same player
    /// </summary>
    [Tooltip("OneByOne - player end turn after moving one unit, only one unit on one tile, tile2tile interaction\nAllByOne - player end turn after moving all units, multiple units on one tile")]
    public int playerID;

    /// <summary>
    /// list of tiles accesible to THIS object
    /// </summary>
    [Header("Movement")]
    public List<Tile_Template> selectableTiles = new List<Tile_Template>();

    /// <summary>
    /// tile under THIS unit
    /// </summary>
    public Tile_Template currentTile;

    /// <summary>
    /// destination tile for THIS if moving
    /// </summary>
    public Tile_Template targetTile;

    /// <summary>
    /// 
    /// </summary>
    public List<Tile_Template> path = new List<Tile_Template>();

    /// <summary>
    /// 
    /// </summary>
    public List<Tile_Template> pathGhost = new List<Tile_Template>();

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

    private void Awake()
    {
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

        // assign tileManager
        if (tileManager_Campaign == null)
        {
            if ((tileManager_Campaign = GameObject.FindGameObjectWithTag("TileManager_Campaign").GetComponent<TileManager_Campaign>()) == null)
                Debug.LogError("tileManager_Campaign", gameObject);
        }

        // assign tileManager
        if (tileManager_Battleground == null)
        {
            if ((tileManager_Battleground = GameObject.FindGameObjectWithTag("TileManager_Battleground").GetComponent<TileManager_Battleground>()) == null)
                Debug.LogError("tileManager_Battleground", gameObject);
        }

        Init_Awake();
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void Init_Awake() { }

    private void Start()
    {
        /*
        // add to static list
        if (!TileManager_Template.list_Units.Contains(this))
        {
            TileManager_Template.list_Units.Add(this);
            TileManager_Template.UnitAwake();
        }
        */

        // INIT variables
        targetTile = null;
        unitIsSelected = false;

        

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
            /*
            if(tileManager.list_publicSelectedUnits != null)
            {
                if (tileManager.list_publicSelectedUnits.Contains(this))
                {
                    NewTurn();
                }
            }
            */
            
            if(TileManager_Template.selectedUnit != null)
            {
                if(TileManager_Template.selectedUnit == this)
                {
                    NewTurn();
                }
            }
            
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

    public Tile_Template GetTargetTile(GameObject target)
    {
        // Debug.Log("<color=yellow> GetTargetTile </color>\n target = " + target + "\n", gameObject);

        RaycastHit hit;
        Tile_Template tile = null;

        if(Physics.Raycast(target.transform.position, -Vector3.up, out hit, 10))
        {
            tile = hit.collider.GetComponent<Tile_Template>();
        }

        return tile;
    }


    /// <summary>
    /// 
    /// </summary>
    public void FindSelectableTiles()
    {
        // Debug.Log("<color=yellow> FindSelectableTiles </color>\n\n",gameObject);

        foreach (Tile_Template tile in TileManager_Template.list_Tile)
        {
            tile.Reset();
        }

        if (currentTile == null)
        {
            GetCurrentTile();
        }

        currentTile.walkable = true;

        Queue<Tile_Template> process = new Queue<Tile_Template>();

        process.Enqueue(currentTile);

        currentTile.visited = true;

        while(process.Count > 0)
        {
            Tile_Template t = process.Dequeue();

            selectableTiles.Add(t);
            t.selectable = true;

            if(t.distance < remainingMoves)
            {
                foreach (Tile_Template tile in t.adjacencyList)
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tile"></param>
    public void MoveToTile(Tile_Template tile)
    {
        Debug.Log("<color=yellow> MoveToTile </color>\n tile = " + tile + "\n", gameObject);

        path.Clear();

        // moving = true;

        Tile_Template next = tile;

        while(next != null)
        {
            path.Add(next);
            next = next.parent;
        }

        targetTile = tile;

        // MOVE !!!
        isMoving = true;
        TileManager_Template.someUnitIsMoving = true;
    }

    public void MoveToTileGhost(Tile_Template tile)
    {
        pathGhost.Clear();

        TileManager_Template.ResetTilePath();

        // moving = true;

        Tile_Template next = tile;

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
        if (isMoving && TileManager_Template.someUnitIsMoving)
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
            Tile_Template t = path[path.Count - 1];

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
            TileManager_Template.someUnitIsMoving = false;

            RemoveSelectableTiles();

            currentTile = targetTile;
            targetTile = null;

            TileManager_Template.ResetTilePath();

            if (currentTile.unitsOnTile != null)
            {
                if (!currentTile.unitsOnTile.Contains(this))
                {
                    currentTile.UnitsOnTile_Add(this);
                }
            }

            // transform.forward = new Vector3( Mathf.Clamp(heading.x, 0, 1), 0 ,Mathf.Clamp(heading.z,0,1));

            // currentTile.walkable = false;


            TileManager_Template.selectedUnit = null;
            TileManager_Template.selectedTile = null;

            // update map color
            tileManager_Campaign.UpdateTileColor(false);

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
    /// reset currentTile , tile.Reset and clear selectable tiles
    /// </summary>
    protected void RemoveSelectableTiles()
    {
        if(currentTile != null)
        {
            currentTile.current = false;
            currentTile = null;
        }

        foreach(Tile_Template tile in selectableTiles)
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

        if (TileManager_Template.someUnitIsMoving)
        {
            return;
        }

        int shortestDistance =  remainingMoves;

        if (currentTile.unitsOnTile.Count == 1)
        {
            // only one unit is on tile
            // TileManager.selectedUnit.Clear();
            // TileManager.selectedUnit = null;
            TileManager_Template.SelectedUnit(null);
            // tileManager.list_publicSelectedUnits.Clear();
        }
        else
        {
            /*
            // more units are on tile, get shortest distance available for this group of units
            foreach(TacticsMove units in tileManager.list_publicSelectedUnits)
            {
                if(units.remainingMoves < shortestDistance)
                {
                    shortestDistance = units.remainingMoves;
                }
            }
            */
        }

        // TODO: if multiple units are selected, find smallest distance to walk and apply
        FindSelectableTiles();

        // TODO: add THIS unit or all units in THIS group to tileManager.selectedUnits
        // TileManager.SelectedUnit_Add(this);
        TileManager_Template.selectedUnit = this;
        TileManager_Template.SelectedUnit(this);

        tileManager_Campaign.UpdateTileColor(true);

        // TODO: do for all selected units
        unitIsSelected = true;

    }
    private void OnRightMouseClick()
    {
        Debug.Log("<color=yellow> OnRightMouseClick </color>\n TacticsMove " + this + "\n", gameObject);

        tileManager_Campaign.UpdateTileColor(false);

        // TODO: make all selected units unselected
        // check if do for all or only THIS
        TileManager_Template.selectedUnit = null;

        tileManager_Campaign.DeselectUnits();
    }

    #endregion
}
