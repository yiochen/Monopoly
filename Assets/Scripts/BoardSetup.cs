using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Monopoly.Client
{
    [JsonObject(
    MemberSerialization.OptIn,
    NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class BoardSetup
    {
        [JsonProperty] public int Height { get; set; }
        [JsonProperty] public int Width { get; set; }
        [JsonProperty] public MonopolyWorld World { get; set; }
    }
}