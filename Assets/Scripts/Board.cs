using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Prisel.Common;
using System.Threading.Tasks;
using System;
using UnityEngine.Tilemaps;
using Monopoly.Protobuf;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Monopoly.Client
{
    /// <summary>
    /// Board should be added to the Grid object of Tilemap
    /// </summary>
    public class Board : MonoBehaviour
    {
        public Tilemap Ground;
        public Tilemap AboveGround;

        private World World;

        internal int Width = 0;
        internal int Height = 0;

        public void Render(World world, int width, int height)
        {
            World = world;
            Width = width;
            Height = height;

            // go through all tiles and render
            foreach (TileObject tile in world.GetAll<TileObject>())
            {
                Addressables.LoadAssetAsync<TileBase>(tile.GetRoadDirection().ToAddress()).Completed += (AsyncOperationHandle<TileBase> obj) =>
                {
                    Ground.SetTile(tile.Position.ToTilemap(), obj.Result);
                };
            }

            foreach (PropertyObject property in world.GetAll<PropertyObject>())
            {
                Addressables.LoadAssetAsync<TileBase>(property.ToAddress()).Completed += (AsyncOperationHandle<TileBase> obj) =>
                {
                    AboveGround.SetTile(property.Anchor.ToTilemap(), obj.Result);
                };

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