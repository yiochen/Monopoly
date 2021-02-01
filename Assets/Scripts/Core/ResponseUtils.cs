using System;

using Prisel.Protobuf;

namespace Prisel.Common
{
    public static class ResponseUtils
    {
        private static readonly Status OkStatus = new Status
        {
            Code = Status.Types.Code.Ok,
            Message = "OK",
        };

        public static bool IsResponse(this Packet packet) => packet.Type == PacketType.Response;

        public static Packet NewRespond(this Packet packet, Payload payload = null, Status status = null)
        {
            if (packet.IsAnySystemAction())
            {
                return new Packet
                {
                    Type = PacketType.Response,
                    SystemAction = packet.SystemAction,
                    RequestId = packet.RequestId,
                    Status = status ?? OkStatus,
                    Payload = payload

                };
            }
            return new Packet {
                Type = PacketType.Response,
                Action = packet.Action,
                RequestId = packet.RequestId,
                Status = status ?? OkStatus,
                Payload = payload
            };
        }
    }
}
