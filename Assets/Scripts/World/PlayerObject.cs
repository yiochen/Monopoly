using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
namespace Monopoly.Client
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PlayerObject : GameObject
    {
        public override string Type
        {
            get => "game_player";
        }
    }
}