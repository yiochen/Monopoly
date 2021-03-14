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

        [JsonProperty]
        List<PlayerObject> Players;

        public Board Board;

        public int Height;
        public int Width;

        protected override void PopulateSerializedFields()
        {
            Tiles = GetAll<TileObject>();
            Properties = GetAll<PropertyObject>();
            Players = GetAll<PlayerObject>();
        }

        protected override void ClearSerializedFields()
        {
            Tiles = null;
            Properties = null;
            Players = null;
        }

        public override void AddToWorld()
        {
            Board = Object.FindObjectOfType<Board>();
            Board.Render(this, Width, Height);
        }
    }
}