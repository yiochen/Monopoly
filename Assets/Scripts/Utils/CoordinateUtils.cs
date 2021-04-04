using Monopoly.Protobuf;
public static class CoordinateUtils
{
    public static string ToKey(this Coordinate coor)
    {
        return $"{coor.Row},{coor.Col}";
    }
}