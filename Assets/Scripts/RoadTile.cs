using UnityEngine;
using UnityEngine.Tilemaps;

namespace Monopoly.Client
{
    public enum RoadDirection
    {
        ALL_DIRECTION,
        VERTICAL,
        HORIZONTAL,
        T_LEFT,
        T_RIGHT,
        T_UP,
        T_DOWN,
        LEFT_UP,
        LEFT_DOWN,
        RIGHT_UP,
        RIGHT_DOWN,
        EMPTY
    }
    [CreateAssetMenu(menuName = "tiles/RoadTile")]
    public class RoadTile : TileBase
    {
        public RoadDirection Direction;
        internal AssetReference AssetRef { get; set; }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = GetSprite(Direction);
        }

        Sprite GetSprite(RoadDirection direction)
        {
            switch (direction)
            {
                case RoadDirection.ALL_DIRECTION:
                    return AssetRef.AllDirectionRoad;
                case RoadDirection.T_LEFT:
                    return AssetRef.TLeftRoad;
                case RoadDirection.T_RIGHT:
                    return AssetRef.TRightRoad;
                case RoadDirection.T_UP:
                    return AssetRef.TUpRoad;
                case RoadDirection.T_DOWN:
                    return AssetRef.TDownRoad;
                case RoadDirection.VERTICAL:
                    return AssetRef.VerticalRoad;
                case RoadDirection.HORIZONTAL:
                    return AssetRef.HorizontalRoad;
                case RoadDirection.LEFT_UP:
                    return AssetRef.LeftUpRoad;
                case RoadDirection.LEFT_DOWN:
                    return AssetRef.LeftDownRoad;
                case RoadDirection.RIGHT_UP:
                    return AssetRef.RightUpRoad;
                case RoadDirection.RIGHT_DOWN:
                    return AssetRef.RightDownRoad;
                default:
                    return AssetRef.EmptyTile;
            }
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

        internal static RoadDirection GetDirection(this TileObject tileObject)
        {

            bool up = false;
            bool left = false;
            bool right = false;
            bool down = false;

            foreach (Ref<TileObject> prev in tileObject.Prev)
            {
                if (prev.Value.IsLeftOf(tileObject))
                {
                    left = true;
                }
                if (prev.Value.IsRightOf(tileObject))
                {
                    right = true;
                }
                if (prev.Value.IsUpOf(tileObject))
                {
                    up = true;
                }
                if (prev.Value.IsDownOf(tileObject))
                {
                    down = true;
                }
            }

            foreach (Ref<TileObject> next in tileObject.Next)
            {
                if (next.Value.IsLeftOf(tileObject))
                {
                    left = true;
                }
                if (next.Value.IsRightOf(tileObject))
                {
                    right = true;
                }
                if (next.Value.IsUpOf(tileObject))
                {
                    up = true;
                }
                if (next.Value.IsDownOf(tileObject))
                {
                    down = true;
                }
            }
            if (up && down && left && right)
            {
                return RoadDirection.ALL_DIRECTION;
            }
            if (up && down && left)
            {
                return RoadDirection.T_LEFT;
            }
            if (up && down && right)
            {
                return RoadDirection.T_RIGHT;
            }
            if (up && left && right)
            {
                return RoadDirection.T_UP;
            }
            if (down && left && right)
            {
                return RoadDirection.T_DOWN;
            }
            if (up && down)
            {
                return RoadDirection.VERTICAL;
            }
            if (left && right)
            {
                return RoadDirection.HORIZONTAL;
            }
            if (left && up)
            {
                return RoadDirection.LEFT_UP;
            }
            if (left && down)
            {
                return RoadDirection.LEFT_DOWN;
            }
            if (right && up)
            {
                return RoadDirection.RIGHT_UP;
            }
            if (right && down)
            {
                return RoadDirection.RIGHT_DOWN;
            }
            return RoadDirection.EMPTY;

        }
    }
}