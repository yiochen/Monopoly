using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Monopoly.Protobuf;
using System.Collections.Generic;

namespace Monopoly.Client
{
    [JsonObject(MemberSerialization.OptIn)]
    class TileObject : GameObject
    {
        public override string Type
        {
            get => "tile";
        }
        [JsonProperty]
        public Coordinate Position { get; set; } =
        new Coordinate
        {
            Row = -1,
            Col = -1,

        };

        [JsonProperty]
        public List<Ref<TileObject>> Prev { get; set; } = new List<Ref<TileObject>>();

        [JsonProperty]
        public List<Ref<TileObject>> Next { get; set; } = new List<Ref<TileObject>>();

        [JsonProperty]
        public List<Ref<PropertyObject>> hasProperties { get; set; } = new List<Ref<PropertyObject>>();

        [JsonProperty]
        public bool IsStart { get; set; } = false;

        // chancePool and tileEffect are only for serverside use. So not included
    }
}
