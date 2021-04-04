using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Monopoly.Protobuf;
namespace Monopoly.Client
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PropertyObject : GameObject
    {
        public override string Type
        {
            get => "property";
        }

        [JsonProperty]
        public Coordinate Anchor { get; set; } = new Coordinate { Row = -1, Col = -1 };

        [JsonProperty]
        public Size Size { get; set; } = new Size { Width = 0, Height = 0 };

        [JsonProperty]
        public int CurrentLevel { get; set; } = -1;

        [JsonProperty]
        public string Name { get; set; } = "";
    }

    [JsonObject(
    MemberSerialization.OptIn,
    NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Size
    {
        [JsonProperty]
        public int Width { get; set; }

        [JsonProperty]
        public int Height { get; set; }
    }

}