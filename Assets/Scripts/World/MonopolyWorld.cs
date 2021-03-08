using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Monopoly.Client
{
    [JsonObject(
    MemberSerialization.OptIn,
    NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class MonopolyWorld : World
    {
        [JsonProperty]
        List<TileObject> Tiles;

        [JsonProperty]
        List<PropertyObject> Properties;

        [JsonProperty]
        List<PlayerObject> Players;

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
    }
}