using System;

using Prisel.Protobuf;

namespace Prisel.Common
{
    public static class RequestUtils
    {
        public static bool IsRequest(this Packet packet) => packet.Type == PacketType.Request;

        public static Packet NewSystemRequest(SystemActionType systemAction, string requestId, Payload payload = null )
        {
            return new Packet
            {
                Type = PacketType.Request,
                SystemAction = systemAction,
                RequestId =  requestId,
                Payload = payload,
            };
        }
        public static Packet NewRequest(string action, string requestId, Payload payload = null)
        {
            return new Packet
            {
                Type = PacketType.Request,
                Action = action,
                RequestId = requestId,
                Payload = payload,
            };
        }
    }

}
