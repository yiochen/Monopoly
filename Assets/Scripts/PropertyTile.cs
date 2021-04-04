using Monopoly.Protobuf;
namespace Monopoly.Client
{
    public static class PropertyTileUtils
    {
        public static string DEFAULT_ADDRESS = "Assets/Tileset/property.asset";

        public static string ToAddress(this PropertyInfo propertyInfo, Player player)
        {
            if (propertyInfo.CurrentLevel == -1)
            {
                return "Assets/Tileset/property.asset";
            }
            return GetOwnedAddress(propertyInfo.CurrentLevel, player.Color);
        }

        private static string GetOwnedAddress(int level, CharacterColor color)
        {
            switch (level)
            {
                case 0:
                    return $"Assets/Tileset/property-owned-{color.ToString().ToLower()}.asset";
                case 1:
                // fall through
                case 2:
                    return $"Assets/Tileset/property-level-{level + 1}-{color.ToString().ToLower()}.asset";

            }
            return DEFAULT_ADDRESS; // use as error asset for now
        }
    }
}