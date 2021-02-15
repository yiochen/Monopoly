using UnityEngine;
using UnityEngine.UIElements;
using System;
using Prisel.Common;
using System.Threading.Tasks;




public class LobbyScene : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        if (Application.isEditor && Application.isPlaying)
        {
            await TestLoginFlow.AttemptLogin();
        }
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
        var openCreateRoomDialogButton = rootVisualElement.Q<Button>("open-create-room-dialog-button");

        var createRoomDialogContainer = rootVisualElement.Q<VisualElement>("create-room-dialog-background");
        var roomnameField = rootVisualElement.Q<TextField>("roomname-field");
        var createRoomButton = rootVisualElement.Q<Button>("create-room-button");

        Debug.Log("lobby initialized" + createRoomButton);
        //createRoomDialogContainer.style.display = DisplayStyle.None;
        openCreateRoomDialogButton.RegisterCallback<ClickEvent>(e => createRoomDialogContainer.style.display = DisplayStyle.Flex);
        createRoomButton.RegisterCallback<ClickEvent>(async e => await CreateRoom(roomnameField.value));
    }

    private async Task CreateRoom(string roomname)
    {
        Debug.Log("Creating room");
        if (String.IsNullOrEmpty(roomname))
        {
            Debug.Log("Cannot create room with empty roomname");
        }
        else
        {
            var client = WebSocketClient.Instance;
            var response = await client.CreateRoom(roomname);
            ClientState clientState = client.GetState<ClientState>() ?? new ClientState();
            clientState.RoomId = response.Payload.CreateRoomResponse.Room.Id;
            clientState.RoomName = response.Payload.CreateRoomResponse.Room.Name;
            Debug.Log($"Successfully create room with roomname {clientState.RoomName}, id {clientState.RoomId}");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
