using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumTileType { Grass, Forest, Mountain, River}

public class Tile : MonoBehaviour
{
    public EnumTileType enumTileType;

    /// <summary>
    /// cna unit walk on this tile ?
    /// </summary>
    public bool walkable;

    /// <summary>
    /// how much distance will it take to cross this tile
    /// </summary>
    public int tileWalkingDistance;

    [Header("Graphics")]
    public Renderer render;
    public Material material;


    private void Awake()
    {
        name = "Tile_[" + Mathf.RoundToInt(transform.localPosition.x) + "," + Mathf.RoundToInt(transform.localPosition.z) + "]";

        render = GetComponent<Renderer>();
        material = render.material;

        walkable = true;
        tileWalkingDistance = 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!TileManager.list_Tile.Contains(this))
        {
            TileManager.list_Tile.Add(this);
        }

        SetTileEnum();
    }

    [ContextMenu("01 - SetTileEnum")]
    public void SetTileEnum()
    {
        enumTileType = EnumTileType.Grass;

        if (transform.localPosition.y < 0)
        {
            enumTileType = EnumTileType.River;
        }
        else if (transform.localPosition.y > 0.5 && transform.localPosition.y < 1.5)
        {
            enumTileType = EnumTileType.Forest;
        }
        else if (transform.localPosition.y >= 1.5)
        {
            enumTileType = EnumTileType.Mountain;
        }

        SetTileColor();
    }

    public void SetTileColor()
    {
        
        if(material == null)
        {
            render = GetComponent<Renderer>();
            material = render.material;
        }
        
        switch (enumTileType)
        {
            case EnumTileType.River:
                walkable = false;
                material.color = Color.blue;
                break;
            case EnumTileType.Grass:
                material.color = Color.green;
                break;
            case EnumTileType.Forest:
                tileWalkingDistance = 3;
                material.color = new Color(0.125f,0.5f,0.2f,1);
                break;
            case EnumTileType.Mountain:
                tileWalkingDistance = 10;
                material.color = Color.grey;
                break;
            default:
                walkable = false;
                tileWalkingDistance = 99;
                material.color = Color.white;
                break;
        }
    }

}
