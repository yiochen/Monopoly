using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Prisel.Common;
using UnityEngine.SceneManagement;
using Prisel.Protobuf;
using TMPro;

public class RoomUiControl : MonoBehaviour
{
    [SerializeField] private PlayerCardControl PlayerCardPrefab;
    [SerializeField] private GameObject PlayerCardGrid;
    [SerializeField] private Button BackButton;
    [SerializeField] private TextMeshProUGUI RoomNameLabel;
    [SerializeField] private TextMeshProUGUI RoomIdLabel;
    [SerializeField] private ButtonControl StartButton;
    private string HostId = "";
    private string StateToken = "";
    private PriselClient Client = PriselClient.Instance;
    private bool LeavingInProgress = false;
    private bool StartingInProgress = false;

    private void OnEnable()
    {

        BackButton.onClick.RemoveAllListeners();
        BackButton.onClick.AddListener(OnBackClicked);
        StartButton.SetCallback(StartGame);
    }

    private async void OnBackClicked()
    {
        if (!LeavingInProgress)
        {
            LeavingInProgress = true;
            var leaveResponse = await Client.Leave();
            if (leaveResponse.IsStatusOk())
            {
                Client.State().ClearRoomState();
                SceneManager.LoadScene("login", LoadSceneMode.Single);
            }
            else
            {
                Debug.LogError($"leaving room failed because {leaveResponse.StatusMessage()}");

            }
            LeavingInProgress = false;
        }
    }

    private void OnDisable()
    {
        Debug.Log("Room scene OnDisable called");
        BackButton.onClick.RemoveAllListeners();
        StartButton.ClearCallback();
    }

    private async void StartGame()
    {
        if (!StartingInProgress)
        {
            StartingInProgress = true;
            var response = await Client.Request(RequestUtils.NewSystemRequest(SystemActionType.GameStart, Client.NewId));
            if (response.IsStatusOk())
            {
                StartingInProgress = false;
                SceneManager.LoadScene("game", LoadSceneMode.Single);
            }
        }
    }

    async void Start()
    {
        Debug.Log("Room scene started");
        if (Application.isEditor && Application.isPlaying)
        {
            await TestLoginFlow.AttemptCreateRoom();
        }

        RoomNameLabel.text = Client.State().RoomName;
        RoomIdLabel.text = Client.State().RoomId;

        var roomStateResponse = await Client.GetRoomState();
        if (roomStateResponse.IsStatusOk())
        {
            InitializePlayerListModel(roomStateResponse.Payload.GetRoomStateResponse);
        }
        // // subscribe to room change
        Client.OnRoomStateChange += OnRoomStateChange;
    }

    private void InitializePlayerListModel(GetRoomStateResponse roomStateResponse)
    {

        HostId = roomStateResponse.HostId;
        StateToken = roomStateResponse.Token;
        foreach (var player in roomStateResponse.Players)
        {
            AddNewPlayer(player);
        }
        UpdateForHost();
    }

    void AddNewPlayer(PlayerInfo player)
    {
        var newPlayerCard = Instantiate(PlayerCardPrefab);
        newPlayerCard.transform.parent = PlayerCardGrid.transform;
        newPlayerCard.playerName = player.Name;
        newPlayerCard.playerId = player.Id;
        newPlayerCard.name = player.Id;
    }

    void RemovePlayer(string id)
    {
        var removed = PlayerCardGrid.transform.Find(id);
        if (removed != null)
        {
            Destroy(removed);
        }
    }

    private void OnRoomStateChange(Packet packet)
    {
        var changePayload = packet.Payload.RoomStateChangePayload;
        if (!changePayload.Token.PreviousToken.Equals(StateToken))
        {
            Debug.LogError($"room state token not matching. This means we lost a room state change packet. Current room state might not be accurate");
            return;
        }
        StateToken = changePayload.Token.Token;
        switch (changePayload.ChangeCase)
        {
            case RoomStateChangePayload.ChangeOneofCase.PlayerJoin:
                var playerJoin = changePayload.PlayerJoin;
                AddNewPlayer(playerJoin);
                Debug.Log($"player {playerJoin.Id} joined");
                break;
            case RoomStateChangePayload.ChangeOneofCase.PlayerLeave:
                var playerLeave = changePayload.PlayerLeave;
                RemovePlayer(playerLeave);
                Debug.Log($"player {playerLeave} left");
                break;
            case RoomStateChangePayload.ChangeOneofCase.HostLeave:
                var hostLeave = changePayload.HostLeave;
                Debug.Log($"host {hostLeave.HostId} left, new host Id {hostLeave.NewHostId}");
                RemovePlayer(hostLeave.HostId);
                HostId = hostLeave.NewHostId;
                UpdateForHost();
                break;
        }
    }

    void UpdateForHost()
    {
        if (HostId.Equals(PriselClient.Instance.State().UserId))
        {
            StartButton.gameObject.SetActive(true);
        }
        else
        {
            StartButton.gameObject.SetActive(false);
        }
    }

}
