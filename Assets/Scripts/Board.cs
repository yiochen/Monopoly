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

        private Vector3 CellSize = Vector3.zero;
        public Vector3 CharacterOffset = Vector3.zero;

        public Vector2Int TestCoordinate = Vector2Int.zero;

        void Awake()
        {
            CellSize = AboveGround.cellSize;
            // Move the character right and up. The character is originally at
            // the anchor of tile, which is bottom left corner.
            CharacterOffset = new Vector3(CellSize.x / 2, CellSize.y * 0.4f, 0);
        }
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

        public void MoveToPos(UnityEngine.GameObject gameObject, Coordinate pos)
        {
            gameObject.transform.position = GetCharacterPos(pos);
        }

        public Vector3 GetCharacterPos(Coordinate pos)
        {
            return AboveGround.CellToWorld(pos.ToTilemap()) + CharacterOffset;
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