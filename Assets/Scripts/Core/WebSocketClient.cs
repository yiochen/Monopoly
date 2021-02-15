using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using Prisel.Protobuf;
using Google.Protobuf;


using HybridWebSocket;
using System;

namespace Prisel.Common
{
    public class WebSocketClient
    {
        private static readonly System.Lazy<WebSocketClient> LazyInstance =
        new System.Lazy<WebSocketClient>(CreateSingleton);

        public static WebSocketClient Instance => LazyInstance.Value;

        private static WebSocketClient CreateSingleton()
        {
            return new WebSocketClient();
        }

        public delegate void OnEmit(Packet packet);
        public OnEmit OnEmitCallback;

        private WebSocket Client;
        private Task ConnectionTask;
        private readonly RequestManager RequestManager = RequestManager.Instance;

        // System actions coming from server
        public Action<Packet> OnWelcome;
        public Action<Packet> OnGameStart;
        public Action<Packet> OnRoomStateChange;
        public Action<Packet> OnBroadcast;

        private dynamic _State;

        public void SetState<T>(T state)
        {
            _State = state;
        }

        public T GetState<T>() => _State is T ? _State: null ;

        public Dictionary<string, Action<Packet>> OnActions = new Dictionary<string, Action<Packet>>();

        private Task WrapConnect(WebSocket client)
        {
            var t = new TaskCompletionSource<bool>();
            client.OnOpen += () => { t.TrySetResult(true); };
            client.Connect();
            return t.Task;
        }

        public bool IsConnected { get; private set; }
        public string ServerUrl { private get; set; } = "ws://echo.websocket.org";

        public async Task<WebSocketClient> Connect()
        {
            if (!IsConnected)
            {
                // Create WebSocket instance
                Client = WebSocketFactory.CreateInstance(ServerUrl);
                Debug.Log("connecting to " + ServerUrl);



                // Add OnMessage event listener
                Client.OnMessage += OnMessage;

                // Add OnError event listener
                Client.OnError += OnError;

                // Add OnClose event listener
                Client.OnClose += OnClose;

                ConnectionTask = WrapConnect(Client);
                await ConnectionTask;
                if (ConnectionTask != null)
                {
                    IsConnected = true;
                    ConnectionTask = null; // remove the connectionTask, we don't need it anymore
                }
                else
                {
                    // client is closed through Close();
                    IsConnected = false;
                }
            }
            return this;
        }

        public void Close()
        {
            if (IsConnected)
            {
                Client.Close();
                IsConnected = false;

                ConnectionTask = null;

                return;
            }
            if (ConnectionTask != null)
            {
                // It is currently connecting
                Client.Close();
                ConnectionTask = null;
                IsConnected = false;
                return;
            }
            IsConnected = false;
        }

        public void OnError(string errMsg)
        {
            Debug.Log("WS error: " + errMsg);
        }

        public void OnClose(WebSocketCloseCode code)
        {
            Debug.Log("WS closed with code: " + code.ToString());
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
                OnActions[packet.Action]?.Invoke(packet);
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
            Client.Send(packet.ToByteArray());
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
}

