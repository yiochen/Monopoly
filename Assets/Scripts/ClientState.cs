using System;

using Prisel.Common;
public class ClientState
{
    public string Username { get; set; } = "";

    public string UserId { get; set; } = "";

    public string RoomName { get; set; } = "";

    public string RoomId { get; set; } = "";

    public string GamePlayerId { get; set; } = "";

    public bool GameStarted { get; set; } = false;

    public void ClearRoomState()
    {
        ClearGameState();
        RoomName = "";
        RoomId = "";

    }

    public void ClearGameState()
    {
        GamePlayerId = "";
        GameStarted = false;
    }
}

public static class ClientStateExtension
{
    public static ClientState State(this PriselClient priselClient)
    {
        ClientState state = priselClient.GetState<ClientState>() ?? new ClientState();
        priselClient.SetState(state);
        return state;
    }

    public static void ClearState(this PriselClient priselClient)
    {
        priselClient.SetState(new ClientState());
    }
}

