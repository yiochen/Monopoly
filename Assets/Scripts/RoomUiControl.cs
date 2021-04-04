using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Prisel.Common;
using UnityEngine.SceneManagement;
using Prisel.Protobuf;

public class RoomUiControl : MonoBehaviour
{
    private VisualElement UiRoot;
    private Label RoomNameLabel;
    private Label RoomIdLabel;
    private Button StartButton;

    private ListView PlayerList;

    private PlayerListModel ListModel;

    private Button LeaveRoomButton;

    private PriselClient Client = PriselClient.Instance;

    [SerializeField] private VisualTreeAsset playerListItemAsset;
    private void OnEnable()
    {
        UiRoot = GetComponent<UIDocument>().rootVisualElement;

        RoomNameLabel = UiRoot.Q<Label>("room-name-label");
        RoomIdLabel = UiRoot.Q<Label>("room-id-label");
        StartButton = UiRoot.Q<Button>("start-game-button");
        PlayerList = UiRoot.Q<ListView>("player-list");

        LeaveRoomButton = UiRoot.Q<Button>("leave-room-button");
        LeaveRoomButton.RegisterCallback<ClickEvent>(Leave);
        StartButton.RegisterCallback<ClickEvent>(StartGame);
    }

    private void OnDisable()
    {
        Debug.Log("Room scene OnDisable called");
        LeaveRoomButton.UnregisterCallback<ClickEvent>(Leave);
        StartButton.UnregisterCallback<ClickEvent>(StartGame);
    }

    private async void StartGame(ClickEvent _)
    {
        var response = await Client.Request(RequestUtils.NewSystemRequest(SystemActionType.GameStart, Client.NewId));
        if (response.IsStatusOk())
        {
            SceneManager.LoadScene("game", LoadSceneMode.Single);

        }
    }
    private async void Leave(ClickEvent _)
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
        RoomNameLabel.text = clientState.RoomName;
        RoomIdLabel.text = clientState.RoomId;
        var roomStateResponse = await client.GetRoomState();
        if (roomStateResponse.IsStatusOk())
        {
            InitializePlayerListModel(roomStateResponse.Payload.GetRoomStateResponse);

            PlayerList.itemsSource = ListModel.PlayerList;
            PlayerList.makeItem = () => playerListItemAsset.CloneTree();
            PlayerList.bindItem = (e, i) =>
            {
                e.Q<Label>("player-name").text = ListModel.PlayerList[i].Name;
            };

        }
        // subscribe to room change
        client.OnRoomStateChange += OnRoomStateChange;
    }

    private void InitializePlayerListModel(GetRoomStateResponse roomStateResponse)
    {
        ListModel = new PlayerListModel()
        {
            HostId = roomStateResponse.HostId,
            StateToken = roomStateResponse.Token,
            PlayerList = new List<PlayerListItemModel>(),
        };
        foreach (var player in roomStateResponse.Players)
        {
            ListModel.PlayerList.Add(new PlayerListItemModel()
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
        if (!changePayload.Token.PreviousToken.Equals(ListModel.StateToken))
        {
            Debug.LogError($"room state token not matching. This means we lost a room state change packet. Current room state might not be accurate");
            return;
        }
        ListModel.StateToken = changePayload.Token.Token;
        switch (changePayload.ChangeCase)
        {
            case RoomStateChangePayload.ChangeOneofCase.PlayerJoin:
                var playerJoin = changePayload.PlayerJoin;
                Debug.Log($"player {playerJoin.Id} joined");
                ListModel.PlayerList.Add(new PlayerListItemModel()
                {
                    Id = playerJoin.Id,
                    Name = playerJoin.Name,
                });
                break;
            case RoomStateChangePayload.ChangeOneofCase.PlayerLeave:
                var playerLeave = changePayload.PlayerLeave;
                Debug.Log($"player {playerLeave} left");
                ListModel.PlayerList.Remove(new PlayerListItemModel()
                {
                    Id = playerLeave
                });
                break;
            case RoomStateChangePayload.ChangeOneofCase.HostLeave:
                var hostLeave = changePayload.HostLeave;
                Debug.Log($"host {hostLeave.HostId} left, new host Id {hostLeave.NewHostId}");
                ListModel.PlayerList.Remove(new PlayerListItemModel()
                {
                    Id = hostLeave.HostId
                });
                ListModel.HostId = hostLeave.NewHostId;
                UpdateForHost();
                break;
        }
        PlayerList.Refresh();
    }

    void UpdateForHost()
    {
        if (ListModel.HostId.Equals(PriselClient.Instance.State().UserId))
        {
            StartButton.style.display = DisplayStyle.Flex;
        }
        else
        {
            StartButton.style.display = DisplayStyle.None;
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
