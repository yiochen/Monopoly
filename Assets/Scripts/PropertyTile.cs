using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEditor;
namespace Monopoly.Client
{
    public static class PropertyTileUtils
    {
        /// <summary>
        /// Property assets are named following "property-level-{level}-{color}" pattern.
        /// </summary>
        public static string ToAddress(this PropertyObject property)
        {
            if (property.CurrentLevel == -1)
            {
                return $"Assets/Tileset/property.asset";
            }
            return $"Assets/Tileset/property-level-{property.CurrentLevel}-{property.Owner.Value.ToColor()}";
        }
    }
}