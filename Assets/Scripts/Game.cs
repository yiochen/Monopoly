using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prisel.Common;
using Monopoly.Protobuf;
using Prisel.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace Monopoly.Client
{
    public class Game : MonoBehaviour
    {
        private PriselClient Client = PriselClient.Instance;

        [SerializeField] private Camera WorldCamera;
        [SerializeField] private Board BoardPrefab;
        [SerializeField] private EventBus EventBus;
        [SerializeField] private AnimationDispatcher AnimationDispatcher;
        [SerializeField] private UnityEngine.GameObject PlayerPrefab;
        private List<Player> Players = new List<Player>();
        private Board Board;

        void Awake()
        {
            Board = Instantiate(BoardPrefab, Vector3.zero, Quaternion.identity);
            SetupMap();
        }
        // Start is called before the first frame update
        async void Start()
        {
            if (Application.isEditor && Application.isPlaying)
            {
                await TestLoginFlow.AttemptStartGame();
            }
            // Start listen for messages
            Client.AddAction("animation", (AnimationPayload anim, Packet packet) =>
            {
                _ = AnimationDispatcher.HandleAnimation(anim.Animation);
            });

            Client.AddAction("announce_start_turn", (AnnounceStartTurnPayload payload, Packet packet) =>
            {
                bool isCurrentPlayerTurn = payload.Player.Equals(Client.State().GamePlayerId);

                if (isCurrentPlayerTurn)
                {
                    EventBus.StartCurrentPlayerTurn?.Invoke();
                }
            });

            Client.AddAction("announce_roll", (AnnounceRollPayload payload, Packet packet) => { });

            Client.AddAction("announce_pay_rent", (AnnouncePayRentPayload payload, Packet packet) => { });

            Client.AddAction("prompt_purchase", (PromptPurchaseRequest request, Packet packet) =>
            {
                EventBus.PromptPurchase?.Invoke(request, packet);
            });

            Client.AddAction("announce_purchase", (AnnouncePurchasePayload payload, Packet packet) =>
            {
                EventBus.PropertyChange?.Invoke(payload.Property, Players.Find(player => player.Id.Equals(payload.Player)));
            });

            Client.AddAction("announce_end_turn", (AnnounceEndTurnPayload payload, Packet packet) => { });

            Client.AddAction("announce_bankrupt", (AnnounceBankruptPayload payload, Packet packet) => { });

            Client.AddAction("announce_game_over", (AnnounceGameOverPayload payload, Packet packet) => { });

            Client.AddAction("announce_player_left", (AnnouncePlayerLeftPayload payload, Packet packet) => { });

            Client.AddPacketAction("prompt_chance_confirmation", (Packet packet) =>
            {
                EventBus.PromptChanceConfirmation?.Invoke(packet);
            });

            EventBus.DiceRolled += OnDiceRolled;
            EventBus.LeaveRoom += OnLeaveRoom;
            EventBus.PurchaseDecision += OnPurchaseDecision;
            EventBus.ConfirmChance += OnConfirmChance;

            // Get initial game state
            Packet response = await Client.Request(RequestUtils.NewRequest("get_initial_state", Client.NewId));
            if (response.IsStatusOk() && response.Payload.ActionPayload.TryUnpack<GetInitialStateResponse>(out var payload))
            {
                SetupPlayers(payload);
            }

        }

        void SetupMap()
        {
            GetComponent<MapLoader>().InitializeWorld(Board);
        }

        void SetupPlayers(GetInitialStateResponse response)
        {
            foreach (var player in response.Players)
            {
                if (player.BoundPlayer.Id.Equals(Client.State().UserId))
                {
                    SetGamePlayerInfo(player);
                }
                var newPlayer = SetupPlayer(player);
                Players.Add(newPlayer);
            }
            // Tell server we are ready to start receiving game packets
            Client.Emit(PacketUtils.NewPacket("ready_to_start_game"));
        }

        Player SetupPlayer(GamePlayer playerData)
        {
            var player = Instantiate(PlayerPrefab).GetComponent<Player>();
            player.Initialize(playerData);

            return player;
        }

        void SetGamePlayerInfo(GamePlayer player)
        {
            Client.State().GamePlayerId = player.Id;
            EventBus.UpdateMyGamePlayerInfo?.Invoke(player);
        }

        async void OnDiceRolled()
        {
            var response = await Client.Request(RequestUtils.NewRequest("roll", Client.NewId));
            if (response.IsStatusOk() && response.Payload.ActionPayload.TryUnpack<RollResponse>(out var rollResponse))
            {
                EventBus.DiceRolledResponse?.Invoke(rollResponse.Steps);
            }
        }

        void OnLeaveRoom()
        {

        }

        void OnPurchaseDecision(bool purchased, Packet originalRequest)
        {
            Client.Respond(originalRequest.NewRespond(new Payload
            {
                ActionPayload = Any.Pack(new PromptPurchaseResponse
                {
                    Purchased = purchased
                })
            }));
            if (purchased && originalRequest.Payload.ActionPayload.TryUnpack<PromptPurchaseRequest>(out var promptPurchaseRequest))
            {
                EventBus.UpdateMyMoney?.Invoke(promptPurchaseRequest.MoneyAfterPurchase);
            }
        }

        void OnConfirmChance(Packet originalRequest)
        {
            Client.Respond(originalRequest.NewRespond());
        }



        // private void HandleAnnounce

        // Update is called once per frame
        void Update()
        {

        }

        void OnDestroy()
        {
            Client.ClearAllActions();
        }
    }
}
