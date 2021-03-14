using UnityEngine;
using UnityEngine.Tilemaps;
namespace Monopoly.Client
{
    public enum PropertyColor
    {
        YELLOW,
        BEIGE,
        PINK,
        BLUE,
        GREEN
    }
    public class PropertyTile : TileBase
    {
        public int Level = 0;
        public PropertyColor Color = PropertyColor.BEIGE;


    }
}