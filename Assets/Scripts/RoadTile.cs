using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AddressableAssets;

namespace Monopoly.Client
{
    /// <summary>
    /// Enum representing the Road tile direction. The enum name should match
    /// the file name of the tile asset in Assts/Tileset when converting to
    /// lowercase and replacing "_" with "-"
    /// </summary>
    public enum RoadDirection
    {
        AllDirection,
        Vertical,
        Horizontal,
        TLeft,
        TRight,
        TUp,
        TDown,
        LeftUp,
        LeftDown,
        RightUp,
        RightDown,
        Empty_1
    }
    static class RoadTileUtils
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

        public static string ToAddress(this RoadDirection direction)
        {
            return $"Assets/Tileset/{direction.ToString().ToLowerKebabCase()}.asset";
        }

        public static RoadDirection GetRoadDirection(this TileObject tileObject)
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
                return RoadDirection.AllDirection;
            }
            if (up && down && left)
            {
                return RoadDirection.TLeft;
            }
            if (up && down && right)
            {
                return RoadDirection.TRight;
            }
            if (up && left && right)
            {
                return RoadDirection.TUp;
            }
            if (down && left && right)
            {
                return RoadDirection.TDown;
            }
            if (up && down)
            {
                return RoadDirection.Vertical;
            }
            if (left && right)
            {
                return RoadDirection.Horizontal;
            }
            if (left && up)
            {
                return RoadDirection.LeftUp;
            }
            if (left && down)
            {
                return RoadDirection.LeftDown;
            }
            if (right && up)
            {
                return RoadDirection.RightUp;
            }
            if (right && down)
            {
                return RoadDirection.RightDown;
            }
            return RoadDirection.Empty_1;

        }
    }
}