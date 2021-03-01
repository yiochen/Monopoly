using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Prisel.Common;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Prisel.Protobuf;
using UnityEditor;
public class RoomUiControl : MonoBehaviour
{
    private VisualElement uiRoot;
    private Label roomNameLabel;
    private Label roomIdLabel;
    private Button startButton;

    private ListView playerList;

    private PlayerListModel playerListModel;

    private VisualTreeAsset playerListItemAsset;
    private void OnEnable()
    {
        uiRoot = GetComponent<UIDocument>().rootVisualElement;

        roomNameLabel = uiRoot.Q<Label>("room-name-label");
        roomIdLabel = uiRoot.Q<Label>("room-id-label");
        startButton = uiRoot.Q<Button>("start-game-button");
        playerList = uiRoot.Q<ListView>("player-list");

        playerListItemAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/PlayerInfoItem.uxml");

        var leaveRoomButton = uiRoot.Q<Button>("leave-room-button");
        leaveRoomButton.RegisterCallback<ClickEvent>(async e => await Leave());
        // startButton.RegisterCallback<ClickEvent>(e => playerList.Refresh());
    }

    private async Task Leave()
    {
        PriselClient client = PriselClient.Instance;
        var leaveResponse = await client.Leave();
        if (leaveResponse.IsStatusOk())
        {
            client.State().ClearRoomState();
            SceneManager.LoadScene("lobby", LoadSceneMode.Single);
        }
        else
        {
            Debug.LogError($"leaving room failed because {leaveResponse.StatusMessage()}");
        }
    }

    async void Start()
    {
        Debug.Log("Room scene started");
        if (Application.isEditor && Application.isPlaying)
        {
            await TestLoginFlow.AttemptCreateRoom();
        }
        PriselClient client = PriselClient.Instance;
        var clientState = client.State();
        roomNameLabel.text = clientState.RoomName;
        roomIdLabel.text = clientState.RoomId;
        var roomStateResponse = await client.GetRoomState();
        if (roomStateResponse.IsStatusOk())
        {
            InitializePlayerListModel(roomStateResponse.Payload.GetRoomStateResponse);

            playerList.itemsSource = playerListModel.PlayerList;
            playerList.makeItem = () => playerListItemAsset.CloneTree();
            playerList.bindItem = (e, i) =>
            {
                e.Q<Label>("player-name").text = playerListModel.PlayerList[i].Name;
            };

        }
        // subscribe to room change
        client.OnRoomStateChange += OnRoomStateChange;
    }

    private void InitializePlayerListModel(GetRoomStateResponse roomStateResponse)
    {
        playerListModel = new PlayerListModel()
        {
            HostId = roomStateResponse.HostId,
            StateToken = roomStateResponse.Token,
            PlayerList = new List<PlayerListItemModel>(),
        };
        foreach (var player in roomStateResponse.Players)
        {
            playerListModel.PlayerList.Add(new PlayerListItemModel()
            {
                Id = player.Id,
                Name = player.Name,
            });
        }
        UpdateForHost();
    }

    private void OnRoomStateChange(Packet packet)
    {
        var changePayload = packet.Payload.RoomStateChangePayload;
        if (!changePayload.Token.PreviousToken.Equals(playerListModel.StateToken))
        {
            Debug.LogError($"room state token not matching. This means we lost a room state change packet. Current room state might not be accurate");
            return;
        }
        playerListModel.StateToken = changePayload.Token.Token;
        switch (changePayload.ChangeCase)
        {
            case RoomStateChangePayload.ChangeOneofCase.PlayerJoin:
                var playerJoin = changePayload.PlayerJoin;
                Debug.Log($"player {playerJoin.Id} joined");
                playerListModel.PlayerList.Add(new PlayerListItemModel()
                {
                    Id = playerJoin.Id,
                    Name = playerJoin.Name,
                });
                break;
            case RoomStateChangePayload.ChangeOneofCase.PlayerLeave:
                var playerLeave = changePayload.PlayerLeave;
                Debug.Log($"player {playerLeave} left");
                playerListModel.PlayerList.Remove(new PlayerListItemModel()
                {
                    Id = playerLeave
                });
                break;
            case RoomStateChangePayload.ChangeOneofCase.HostLeave:
                var hostLeave = changePayload.HostLeave;
                Debug.Log($"host {hostLeave.HostId} left, new host Id {hostLeave.NewHostId}");
                playerListModel.PlayerList.Remove(new PlayerListItemModel()
                {
                    Id = hostLeave.HostId
                });
                playerListModel.HostId = hostLeave.NewHostId;
                UpdateForHost();
                break;
        }
        playerList.Refresh();
    }

    void UpdateForHost()
    {
        if (playerListModel.HostId.Equals(PriselClient.Instance.State().UserId))
        {
            startButton.style.display = DisplayStyle.Flex;
        }
        else
        {
            startButton.style.display = DisplayStyle.None;
        }
    }


    struct PlayerListModel
    {
        internal List<PlayerListItemModel> PlayerList;
        internal string HostId { get; set; }
        internal string StateToken { get; set; }
    }

    class PlayerListItemModel : IEquatable<PlayerListItemModel>
    {
        internal string Name { get; set; }
        internal string Id { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            PlayerListItemModel objAsPart = obj as PlayerListItemModel;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }
        public bool Equals(PlayerListItemModel other)
        {
            return Id.Equals(other.Id);
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

}
