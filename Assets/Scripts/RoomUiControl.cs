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

    private List<string> testIdList = new List<string>();

    private void OnEnable()
    {
        uiRoot = GetComponent<UIDocument>().rootVisualElement;

        roomNameLabel = uiRoot.Q<Label>("room-name-label");
        roomIdLabel = uiRoot.Q<Label>("room-id-label");
        startButton = uiRoot.Q<Button>("start-game-button");
        playerList = uiRoot.Q<ListView>("player-list");

        playerListItemAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/PlayerInfoItem.uxml");

        uiRoot.Q<Button>("leave-room-button").RegisterCallback<ClickEvent>(async e => await Leave());
    }

    private async Task Leave()
    {
        WebSocketClient client = WebSocketClient.Instance;
        var leaveResponse = await client.Leave();
        if (leaveResponse.IsStatusOk())
        {
            var clientState = client.GetState<ClientState>();
            clientState.ClearRoomState();
            SceneManager.LoadScene("lobby", LoadSceneMode.Single);
        }
        else
        {
            Debug.LogError($"leaving room failed because {leaveResponse.StatusMessage()}");
        }
    }

    async void Start()
    {
        if (Application.isEditor && Application.isPlaying)
        {
            await TestLoginFlow.AttemptCreateRoom();
        }
        WebSocketClient client = WebSocketClient.Instance;
        var clientState = client.GetState<ClientState>();
        roomNameLabel.text = clientState.RoomName;
        roomIdLabel.text = clientState.RoomId;
        var roomStateResponse = await client.GetRoomState();
        if (roomStateResponse.IsStatusOk())
        {
            InitializePlayerListModel(roomStateResponse.Payload.GetRoomStateResponse);
            Debug.Log($"player list is {playerList}, playerListMode {playerListModel}, model list {playerListModel.PlayerList}");

            // playerList.itemsSource = playerListModel.PlayerList;
            // playerList.makeItem = () => playerListItemAsset.CloneTree();
            // playerList.bindItem = (e, i) =>
            // {
            //     Debug.Log($"binding item at index {i}");
            //     e.Q<Label>("player-name").text = playerListModel.PlayerList[i].Name;
            // };
            playerList.itemsSource = testIdList;
            playerList.makeItem = () => playerListItemAsset.CloneTree();
            playerList.bindItem = (e, i) =>
            {
                Debug.Log($"binding item at index {i}");
                e.Q<Label>("player-name").text = testIdList[i];
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
        };
        foreach (var player in roomStateResponse.Players)
        {
            playerListModel.PlayerList.Add(new PlayerListItemModel()
            {
                Id = player.Id,
                Name = player.Name,
            });
            testIdList.Add(player.Id);
        }
        for (var i = 0; i < 20; i++)
        {
            testIdList.Add("" + i);
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
                testIdList.Add(playerJoin.Id);
                break;
            case RoomStateChangePayload.ChangeOneofCase.PlayerLeave:
                var playerLeave = changePayload.PlayerLeave;
                Debug.Log($"player {playerLeave} left");
                playerListModel.PlayerList.Remove(new PlayerListItemModel()
                {
                    Id = playerLeave
                });
                testIdList.Remove(playerLeave);
                break;
            case RoomStateChangePayload.ChangeOneofCase.HostLeave:
                var hostLeave = changePayload.HostLeave;
                Debug.Log($"host {hostLeave.HostId} left, new host Id {hostLeave.NewHostId}");
                playerListModel.PlayerList.Remove(new PlayerListItemModel()
                {
                    Id = hostLeave.HostId
                });
                testIdList.Remove(hostLeave.HostId);
                playerListModel.HostId = hostLeave.NewHostId;
                UpdateForHost();
                break;
        }
        Debug.Log($"refreshing playerList {playerListModel.PlayerList.Count}");
        // playerList.itemsSource
        playerList.Refresh();
    }

    void UpdateForHost()
    {
        ClientState clientState = WebSocketClient.Instance.GetState<ClientState>();
        if (playerListModel?.HostId?.Equals(clientState.UserId) ?? false)
        {
            startButton.style.display = DisplayStyle.Flex;
        }
        else
        {
            startButton.style.display = DisplayStyle.None;
        }
    }


    [Serializable]
    class PlayerListModel : ScriptableObject
    {
        [SerializeField]
        internal List<PlayerListItemModel> PlayerList = new List<PlayerListItemModel>();
        [SerializeField]
        internal string HostId { get; set; } = "";
        [SerializeField]
        internal string StateToken { get; set; } = "";
    }

    [Serializable]
    class PlayerListItemModel : IEquatable<PlayerListItemModel>
    {
        [SerializeField]
        internal string Name { get; set; } = "";
        [SerializeField]
        internal string Id { get; set; } = "";

        public bool Equals(PlayerListItemModel other)
        {
            if (other == null)
            {
                return false;
            }
            return Id.Equals(other.Id);
        }
    }

}
