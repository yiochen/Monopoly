using UnityEngine;
using UnityEngine.Tilemaps;

namespace Monopoly.Client
{

    class RoadTile : Tile
    {
        internal TileObject TileObject { get; set; }
        internal AssetReference AssetRef { get; set; }


        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.color = this.color;
            tileData.transform = this.transform;
            tileData.flags = this.flags;
            tileData.sprite = GetSprite();
        }

        Sprite GetSprite()
        {
            bool up = false;
            bool left = false;
            bool right = false;
            bool down = false;

            foreach (Ref<TileObject> prev in TileObject.Prev)
            {
                if (prev.Value.IsLeftOf(TileObject))
                {
                    left = true;
                }
                if (prev.Value.IsRightOf(TileObject))
                {
                    right = true;
                }
                if (prev.Value.IsUpOf(TileObject))
                {
                    up = true;
                }
                if (prev.Value.IsDownOf(TileObject))
                {
                    down = true;
                }
            }

            foreach (Ref<TileObject> next in TileObject.Next)
            {
                if (TileObject.IsLeftOf(next.Value))
                {
                    left = true;
                }
                if (TileObject.IsRightOf(next.Value))
                {
                    right = true;
                }
                if (TileObject.IsUpOf(next.Value))
                {
                    up = true;
                }
                if (TileObject.IsDownOf(next.Value))
                {
                    down = true;
                }
            }
            if (up && down && left && right)
            {
                return AssetRef.AllDirectionRoad;
            }
            if (up && down && left)
            {
                return AssetRef.TLeftRoad;
            }
            if (up && down && right)
            {
                return AssetRef.TRightRoad;
            }
            if (up && down)
            {
                return AssetRef.VerticalRoad;
            }
            if (left && right)
            {
                return AssetRef.HorizontalRoad;
            }
            if (left && up)
            {
                return AssetRef.LeftUpRoad;
            }
            if (left && down)
            {
                return AssetRef.LeftDownRoad;
            }
            if (right && up)
            {
                return AssetRef.RightUpRoad;
            }
            if (right && down)
            {
                return AssetRef.RightDownRoad;
            }
            return AssetRef.EmptyTile;
        }
    }
    static class TileUtils
    {
        internal static bool IsLeftOf(this TileObject thisTile, TileObject otherTile)
        {
            return thisTile.Position.Row == otherTile.Position.Row && thisTile.Position.Col < otherTile.Position.Col;
        }

        internal static bool IsRightOf(this TileObject thisTile, TileObject otherTile)
        {
            return thisTile.Position.Row == otherTile.Position.Row && thisTile.Position.Col > otherTile.Position.Col;
        }

        internal static bool IsUpOf(this TileObject thisTile, TileObject otherTile)
        {
            return thisTile.Position.Col == otherTile.Position.Col && thisTile.Position.Row < otherTile.Position.Row;
        }

        internal static bool IsDownOf(this TileObject thisTile, TileObject otherTile)
        {
            return thisTile.Position.Col == otherTile.Position.Col && thisTile.Position.Row > otherTile.Position.Row;
        }
    }
}