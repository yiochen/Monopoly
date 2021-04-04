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

    public static class PlayerColorExtension
    {
        private static int CharacterColorCount = System.Enum.GetNames(typeof(CharacterColor)).Length;
        public static CharacterColor ToColor(this Monopoly.Protobuf.GamePlayer gamePlayer)
        {
            return (CharacterColor)(gamePlayer.Character % CharacterColorCount);
        }
    }

}