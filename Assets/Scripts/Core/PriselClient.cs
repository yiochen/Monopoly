using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Prisel.Protobuf;
using Google.Protobuf;
using SimpleWebSocket;
using System;

namespace Prisel.Common
{
#nullable enable

    public class PriselClient
    {
        private static readonly System.Lazy<PriselClient> LazyInstance =
        new System.Lazy<PriselClient>(CreateSingleton);

        public static PriselClient Instance => LazyInstance.Value;

        private static PriselClient CreateSingleton()
        {
            return new PriselClient();
        }

        public delegate void OnEmit(Packet packet);
        public OnEmit? OnEmitCallback;

        private WebSocketClient? Client;

        private readonly RequestManager RequestManager = RequestManager.Instance;

        // System actions coming from server
        public Action<Packet>? OnWelcome;
        public Action<Packet>? OnGameStart;
        public Action<Packet>? OnRoomStateChange;
        public Action<Packet>? OnBroadcast;

        private object? _State;

        public void SetState<T>(T state)
        {
            _State = state;
        }
        public T? GetState<T>() where T : class => _State is T ? (T)_State : null;
        public Dictionary<string, Action<Packet>> OnActions = new Dictionary<string, Action<Packet>>();
        public bool IsConnected
        {
            get
            {
                return Client != null && Client.State == State.Open;
            }
        }
        public string ServerUrl { private get; set; } = "ws://echo.websocket.org";

        public void AddPacketAction(string action, Action<Packet> callback)
        {
            OnActions.TryGetValue(action, out Action<Packet> callbacks);
            callbacks += callback;
            OnActions.Add(action, callbacks);
        }

        public void AddAction<T>(string action, Action<T, Packet> callback) where T : Google.Protobuf.IMessage, new()
        {
            AddPacketAction(action, (packet) =>
            {
                Debug.Log($"Handling {action}");
                if (packet.Payload.ActionPayload.TryUnpack<T>(out var payload))
                {
                    callback.Invoke(payload, packet);
                }
            });
        }

        public void ClearAllActions()
        {
            OnActions.Clear();
        }
        public async Task<PriselClient> Connect()
        {
            if (!IsConnected)
            {
                // Create WebSocket instance
                Client = WebSocketClient.Create(ServerUrl);
                Debug.Log("Connecting to " + ServerUrl);

                Client.OnMessage += OnMessage;
                Client.OnError += OnError;
                Client.OnClose += OnClose;

                await Client.Connect();
                Debug.Log("Client connected");
            }
            return this;
        }

        public async Task Close()
        {
            if (Client != null)
            {
                await Client.Close();
            }
            Client = null;
        }

        public void OnError(string errMsg)
        {
            Debug.Log($"WS error: {errMsg}");
        }

        public void OnClose(System.Net.WebSockets.WebSocketCloseStatus closeStatus)
        {
            Debug.Log($"WS closed with code {closeStatus}");
        }

        public void OnMessage(byte[] msg)
        {
            Prisel.Protobuf.Packet packet = Prisel.Protobuf.Packet.Parser.ParseFrom(msg);
            ProcessPacket(packet);
            Debug.Log("WS received message" + packet.ToString());
        }

        private void ProcessPacket(Prisel.Protobuf.Packet packet)
        {
            if (packet.Type == PacketType.Response)
            {
                RequestManager.AddResponse(packet);
                return;
            }

            // Request or Packet

            if (packet.IsAnySystemAction())
            {
                switch (packet.SystemAction)
                {
                    case SystemActionType.AnnounceGameStart:
                        OnGameStart?.Invoke(packet);
                        break;
                    case SystemActionType.Broadcast:
                        OnBroadcast?.Invoke(packet);
                        break;
                    case SystemActionType.Welcome:
                        OnWelcome?.Invoke(packet);
                        break;
                    case SystemActionType.RoomStateChange:
                        OnRoomStateChange?.Invoke(packet);
                        break;
                    default:
                        Debug.Log("Unknown SystemActionType encountered" + packet.ToString());
                        break;
                }
                return;
            }

            if (packet.IsAnyCustomAction())
            {
                if (!OnActions.ContainsKey(packet.Action))
                {
                    Debug.LogWarning($"Unknown custom acton encountered {packet.Action}");
                }
                else
                {
                    OnActions[packet.Action]?.Invoke(packet);
                }
                return;
            }

        }

        public Task<Packet> Request(Packet request)
        {
            Emit(request);
            return RequestManager.AddRequest(request);
        }

        public void Respond(Packet response)
        {
            Emit(response);
        }

        public void Emit(Packet packet)
        {
            Debug.Log("emitting " + packet.ToString());
            Client?.Send(packet.ToByteArray());
            OnEmitCallback?.Invoke(packet);
        }

        public string NewId => RequestManager.NewId;

        public async Task<Packet> Login(string username) => await Request(RequestUtils.NewSystemRequest(SystemActionType.Login, NewId, new Payload
        {
            LoginRequest = new LoginRequest
            {
                Username = username,
            }
        }));

        public async Task<Packet> Join(string roomId) => await Request(RequestUtils.NewSystemRequest(SystemActionType.Join, NewId, new Payload
        {
            JoinRequest = new JoinRequest
            {
                RoomId = roomId,
            }
        }));

        public async Task<Packet> Leave() => await Request(RequestUtils.NewSystemRequest(SystemActionType.Leave, NewId));


        public void Exit()
        {
            Emit(PacketUtils.NewSystemPacket(SystemActionType.Exit));
        }

        public async Task<Packet> CreateRoom(string roomName) => await Request(RequestUtils.NewSystemRequest(SystemActionType.CreateRoom, NewId, new Payload
        {
            CreateRoomRequest = new CreateRoomRequest
            {
                RoomName = roomName
            }
        }));

        public async Task<Packet> StartGame() => await Request(RequestUtils.NewSystemRequest(SystemActionType.GameStart, NewId));

        public void Chat(string message)
        {
            Emit(PacketUtils.NewSystemPacket(SystemActionType.Chat, new Payload
            {
                ChatPayload = new ChatPayload
                {
                    Message = message
                }
            }));
        }

        public async Task<Packet> GetRoomState() => await Request(RequestUtils.NewSystemRequest(SystemActionType.GetRoomState, NewId));

        public async Task<Packet> GetLobbyState() => await Request(RequestUtils.NewSystemRequest(SystemActionType.GetLobbyState, NewId));
    }
#nullable disable

}

