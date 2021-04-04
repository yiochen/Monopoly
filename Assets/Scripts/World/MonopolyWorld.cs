using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace Monopoly.Client
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MonopolyWorld : World
    {
        [JsonProperty]
        List<TileObject> Tiles;

        [JsonProperty]
        List<PropertyObject> Properties;

        public Board Board;

        public int Height;
        public int Width;

        protected override void PopulateSerializedFields()
        {
            Tiles = GetAll<TileObject>();
            Properties = GetAll<PropertyObject>();
        }

        protected override void ClearSerializedFields()
        {
            Tiles = null;
            Properties = null;
        }

        public void AddToWorld(Board board)
        {
            board.Render(this, Width, Height);
        }
    }
}