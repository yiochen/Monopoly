using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

using Prisel.Common;
using System;

public class LoginScene : MonoBehaviour
{

    private void OnEnable()
    {
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
        var loginButton = rootVisualElement.Q<Button>("login_button");
        var usernameField = rootVisualElement.Q<TextField>("username_field");

        loginButton.RegisterCallback<ClickEvent>(e => Login(usernameField.value));
    }

    private async void Login(string username)
    {
        if (String.IsNullOrEmpty(username))
        {
            Debug.Log("Cannot login with empty username");
        } else
        {
            var client = WebSocketClient.Instance;
            var response = await client.Login(username);
            var userId = response.Payload.LoginResponse.UserId;
            ClientState clientState = client.GetState<ClientState>() ?? new ClientState();
            clientState.Username = username;
            clientState.UserId = userId;
            Debug.Log($"Successfully login with username {username}, userId {userId}");
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
