using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
namespace Monopoly.Client
{
    public enum CharacterColor
    {
        Yellow = 0,
        Beige = 1,
        Pink = 2,
        Blue = 3,
        Green = 4
    }
    [JsonObject(MemberSerialization.OptIn)]
    public class PlayerObject : GameObject
    {
        private int CharacterColorCount = System.Enum.GetNames(typeof(CharacterColor)).Length;
        public override string Type
        {
            get => "game_player";
        }

        [JsonProperty]
        public int Character = 0;

        public CharacterColor ToColor()
        {
            return (CharacterColor)(Character % CharacterColorCount);
        }
    }
}