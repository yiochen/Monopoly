using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Prisel.Common;
using System.Threading.Tasks;


// Helper class that handles login, creating room, and starting game in play mode
public static class TestLoginFlow
{
    private static string USERNAME = "player";

    public static async Task AttemptConnect()
    {
        PriselClient client = PriselClient.Instance;
        if (!client.IsConnected)
        {
            client.ServerUrl = "ws://localhost:3000";
            await client.Connect();
            client.SetState(new ClientState());
            Debug.Log("Connected!!!");
        }
    }
    public static async Task AttemptLogin()
    {
        await AttemptConnect();
        PriselClient client = PriselClient.Instance;

        var clientState = client.State();
        if (String.IsNullOrEmpty(clientState.UserId))
        {
            var response = await client.Login(USERNAME);
            if (response.IsStatusOk())
            {
                clientState.UserId = response.Payload.LoginResponse.UserId;
                clientState.Username = USERNAME;
                Debug.Log($"Successfully login with username {clientState.Username}, userId {clientState.UserId}");
            }
            else
            {
                Debug.Log($"login failed because {response.StatusMessage()}");
            }

        }
    }

    public static async Task AttemptCreateRoom()
    {
        await AttemptLogin();
        PriselClient client = PriselClient.Instance;
        var clientState = client.State();
        if (String.IsNullOrEmpty(clientState.RoomId))
        {
            var response = await client.CreateRoom("room");
            if (response.IsStatusOk())
            {
                clientState.RoomName = response.Payload.CreateRoomResponse.Room.Name;
                clientState.RoomId = response.Payload.CreateRoomResponse.Room.Id;
                Debug.Log($"Successfully create room with room name {clientState.RoomName}, roomId {clientState.RoomId}");
            }
            else
            {
                Debug.Log($"create room failed because {response.StatusMessage()}");
            }
        }

    }

    public static async Task AttemptStartGame()
    {
        await AttemptCreateRoom();
        PriselClient client = PriselClient.Instance;
        var clientState = client.State();
        if (!clientState.GameStarted)
        {
            var response = await client.StartGame();
            if (response.IsStatusOk())
            {
                client.State().GameStarted = true;
                Debug.Log("Successfully started game");
            }
            else
            {
                Debug.Log($"start game failed because {response.StatusMessage()}");
            }

        }
    }
}
