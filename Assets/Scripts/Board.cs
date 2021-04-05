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
        [SerializeField] private EventBus EventBus;
        public Tilemap Ground;
        public Tilemap AboveGround;

        private World World;

        internal int Width = 0;
        internal int Height = 0;

        private Vector3 CellSize = Vector3.zero;
        public Vector3 CharacterOffset = Vector3.zero;
        public Vector2Int TestCoordinate = Vector2Int.zero;

        private static Board Instance;

        public static Board current
        {
            get
            {

                return Instance;
            }
        }

        [SerializeField] private ChanceChest ChanceChestPrefab;

        private TilemapDict<ChanceChest> ChanceChests = new TilemapDict<ChanceChest>();
        void Awake()
        {
            Instance = this;
            CellSize = AboveGround.cellSize;
            // Move the character right and up. The character is originally at
            // the anchor of tile, which is bottom left corner.
            CharacterOffset = new Vector3(CellSize.x / 2, CellSize.y * 0.4f, 0);

            EventBus.PropertyChange += OnPropertyChange;

        }

        void OnDestroy()
        {
            Instance = null;
        }

        private void OnPropertyChange(PropertyInfo propertyInfo, Player player)
        {
            Addressables.LoadAssetAsync<TileBase>(propertyInfo.ToAddress(player)).Completed += (AsyncOperationHandle<TileBase> obj) =>
            {
                AboveGround.SetTile(propertyInfo.Pos.ToTilemap(), obj.Result);
            };
        }
        public void Render(World world, int width, int height)
        {
            World = world;
            Width = width;
            Height = height;
            EventBus.SetCameraPos?.Invoke(new Vector3(CellSize.x * width / 2, -CellSize.y * height / 2, 0));

            // go through all tiles and render
            foreach (TileObject tile in world.GetAll<TileObject>())
            {
                Addressables.LoadAssetAsync<TileBase>(tile.GetRoadDirection().ToAddress()).Completed += (AsyncOperationHandle<TileBase> obj) =>
                {
                    Ground.SetTile(tile.Position.ToTilemap(), obj.Result);
                };
                if (tile.ChancePool.Count > 0)
                {
                    // render a chance chest here
                    ChanceChest chest = Instantiate(ChanceChestPrefab, Vector3.zero, Quaternion.identity);
                    chest.Coordinate = tile.Position;
                    MoveToPos(chest.gameObject, tile.Position);
                    ChanceChests.Add(tile.Position, chest);
                }
            }

            foreach (PropertyObject property in world.GetAll<PropertyObject>())
            {
                Addressables.LoadAssetAsync<TileBase>(PropertyTileUtils.DEFAULT_ADDRESS).Completed += (AsyncOperationHandle<TileBase> obj) =>
                {
                    AboveGround.SetTile(property.Anchor.ToTilemap(), obj.Result);
                };

            }
        }

        public void MoveToPos(UnityEngine.GameObject gameObject, Coordinate pos)
        {
            gameObject.transform.position = GetCharacterPos(pos);
        }

        public Vector3 GetWorldPos(Coordinate pos)
        {
            return AboveGround.CellToWorld(pos.ToTilemap());
        }

        public Vector3 GetCharacterPos(Coordinate pos)
        {
            return GetWorldPos(pos) + CharacterOffset;
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