using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Prisel.Common;
using System.Threading.Tasks;
using System;

public class LoginUiControl : MonoBehaviour
{

    private void OnEnable()
    {
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
        var loginButton = rootVisualElement.Q<Button>("login-button");
        var usernameField = rootVisualElement.Q<TextField>("username-field");

        loginButton.RegisterCallback<ClickEvent>(async e => await Login(usernameField.value));
    }

    private async Task Connect()
    {
        WebSocketClient client = WebSocketClient.Instance;
        client.ServerUrl = "ws://localhost:3000";
        await client.Connect();
        client.SetState(new ClientState());
    }

    private async Task Login(string username)
    {
        if (String.IsNullOrEmpty(username))
        {
            Debug.Log("Cannot login with empty username");
        }
        else
        {
            var client = WebSocketClient.Instance;
            var response = await client.Login(username);
            var userId = response.Payload.LoginResponse.UserId;
            ClientState clientState = client.GetState<ClientState>() ?? new ClientState();
            client.SetState(clientState);
            clientState.Username = username;
            clientState.UserId = userId;
            Debug.Log($"Successfully login with username {username}, userId {userId}");
            SceneManager.LoadScene("lobby", LoadSceneMode.Single);
        }
    }

    // Start is called before the first frame update
    async Task Start()
    {
        await Connect();
        Debug.Log("Connected!!!");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
