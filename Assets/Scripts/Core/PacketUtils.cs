using Prisel.Protobuf;

namespace Prisel.Common
{
    public static class PacketUtils
    {
        public static bool IsAnySystemAction(this Packet packet) => packet.SystemAction != SystemActionType.Unspecified;

        public static bool IsAnyCustomAction(this Packet packet) => packet.Action != "";

        public static bool IsStatusOk(this Packet packet) => packet.Status.Code == Status.Types.Code.Ok;

        public static bool IsStatusFailed(this Packet packet) => packet.Status.Code == Status.Types.Code.Failed;

        public static string StatusMessage(this Packet packet) => packet.Status.Message;

        public static Packet NewSystemPacket(SystemActionType systemAction, Payload payload = null)
            => new Packet { Type = PacketType.Default, SystemAction = systemAction, Payload = payload };

        public static Packet NewPacket(string action, Payload payload = null)
            => new Packet { Type = PacketType.Default, Action = action, Payload = payload };
        
    }

}
