using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Prisel.Common;
using System.Threading.Tasks;
using System;
using UnityEngine.Tilemaps;
using Monopoly.Protobuf;

namespace Monopoly.Client
{
    /// <summary>
    /// Board should be added to the Grid object of Tilemap
    /// </summary>
    public class Board : MonoBehaviour
    {
        public Tilemap Tilemap;

        private World World;

        internal int Width = 0;
        internal int Height = 0;
        public AssetReference AssetRef;

        private Dictionary<RoadDirection, RoadTile> RoadTiles = new Dictionary<RoadDirection, RoadTile>();

        private TileBase LandTile;

        public void Awake()
        {
            // create Tiles in Runtime
            foreach (RoadDirection direction in System.Enum.GetValues(typeof(RoadDirection)))
            {
                var tile = ScriptableObject.CreateInstance<RoadTile>();
                tile.Direction = direction;
                tile.AssetRef = AssetRef;
                RoadTiles.Add(direction, tile);
            }

        }

        public void Render(World world, int width, int height)
        {
            World = world;
            Width = width;
            Height = height;

            // go through all tiles and render
            foreach (TileObject tile in world.GetAll<TileObject>())
            {
                if (RoadTiles.TryGetValue(tile.GetDirection(), out RoadTile roadTile))
                {
                    Tilemap.SetTile(tile.Position.ToTilemap(), roadTile);
                }
            }

            foreach (PropertyObject property in world.GetAll<PropertyObject>())
            {
                // Tilemap.SetTile(property.Anchor.ToTilemap(), AssetRef.LandTile);
            }
        }
    }

    public static class CoordinateConversion
    {
        /// <summary>
        /// Unity coordinate system is the normal cartesian system with +x
        /// toward the right and +y going up. Tile from server uses +x toward
        /// the right and +y going down
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public static Vector3Int ToTilemap(this Coordinate coordinate)
        {
            return new Vector3Int(coordinate.Col, -coordinate.Row, 0);
        }
    }
}