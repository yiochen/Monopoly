using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Monopoly.Client;
public class TileTest : MonoBehaviour
{
    public TileBase Tile;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetMouseButtonDown(0))
        // {
        //     Debug.Log($"Camera is {Camera.main}");
        //     Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //     Debug.Log($"mouse click at {pos.x}, {pos.y}, {pos.z}");
        //     Tilemap map = GetComponent<Tilemap>();
        //     Vector3Int tilePos = map.WorldToCell(pos);
        //     if (map.HasTile(tilePos))
        //     {
        //         map.SetTile(tilePos, null);
        //     }
        //     else
        //     {
        //         map.SetTile(tilePos, Tile);
        //     }
        // }
    }
}
