using UnityEngine;
using UnityEngine.SceneManagement;
using Prisel.Common;
using System.Threading.Tasks;
using System;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoginUiControl : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI DialogTitle;
    [SerializeField] private FieldControl Field;
    [SerializeField] private ButtonControl SecondaryButton;
    [SerializeField] private ButtonControl PrimaryButton;
    [SerializeField] private Button BackButton;
    private PriselClient Client;

    private UiState<LoginUiControl> NextState;
    private UiState<LoginUiControl> State;

    private async Task Connect()
    {
        PriselClient client = PriselClient.Instance;
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
            var client = PriselClient.Instance;
            var response = await client.Login(username);
            var userId = response.Payload.LoginResponse.UserId;
            ClientState clientState = client.State();
            clientState.Username = username;
            clientState.UserId = userId;
            Debug.Log($"Successfully login with username {username}, userId {userId}");
        }

    }

    private async Task Join(string roomId)
    {
        var response = await Client.Join(roomId);
        ClientState clientState = Client.State();
        clientState.RoomId = response.Payload.JoinResponse.Room.Id;
        clientState.RoomName = response.Payload.JoinResponse.Room.Name;
        Debug.Log($"Successfully join room with roomname {clientState.RoomName}, id {clientState.RoomId}");
        SceneManager.LoadScene("room", LoadSceneMode.Single);
    }
    private async Task CreateRoom(string roomname)
    {
        var response = await Client.CreateRoom(roomname);
        ClientState clientState = Client.State();
        clientState.RoomId = response.Payload.CreateRoomResponse.Room.Id;
        clientState.RoomName = response.Payload.CreateRoomResponse.Room.Name;
        Debug.Log($"Successfully join room with roomname {clientState.RoomName}, id {clientState.RoomId}");
        SceneManager.LoadScene("room", LoadSceneMode.Single);
    }



    // Start is called before the first frame update
    void Start()
    {
        Client = PriselClient.Instance;
        var clientState = Client.State();
        if (!Client.IsConnected)
        {
            NextState = new LoginState();
        }
        else if (String.IsNullOrEmpty(clientState.UserId))
        {
            NextState = new LoginState();
        }
        else if (String.IsNullOrEmpty(clientState.RoomId))
        {
            NextState = new JoinModeState();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (NextState != null)
        {
            NextState.Init(this);
            State = NextState;
        }
        if (State != null)
        {
            State.Render(this);
            NextState = State.TryTransition(this);
        }
    }

    abstract class LoginUiState : UiState<LoginUiControl>
    {
        protected bool BackPressed = false;
        public override void Init(LoginUiControl loginUi)
        {
            EventSystem.current.SetSelectedGameObject(null);
            loginUi.DialogTitle.gameObject.SetActive(true);
            loginUi.Field.gameObject.SetActive(true);
            loginUi.PrimaryButton.gameObject.SetActive(true);
            loginUi.SecondaryButton.gameObject.SetActive(true);
            loginUi.BackButton.gameObject.SetActive(true);
            loginUi.BackButton.onClick.RemoveAllListeners();
            loginUi.BackButton.onClick.AddListener(() =>
            {
                BackPressed = true;
            });
        }
    }

    class LoginState : LoginUiState
    {
        private bool EnterPressed = false;
        private bool LoginSuccess = false;
        public override void Init(LoginUiControl loginUi)
        {
            base.Init(loginUi);

            loginUi.BackButton.gameObject.SetActive(false);
            loginUi.DialogTitle.text = "PLAY AS";
            loginUi.Field.label = "name";
            loginUi.Field.placeholder = "Enter a cool name";
            loginUi.Field.text = "Superman";
            loginUi.SecondaryButton.gameObject.SetActive(false);
            loginUi.PrimaryButton.text = "ENTER";
            loginUi.PrimaryButton.SetCallback(async () =>
            {
                if (!EnterPressed)
                {
                    EnterPressed = true;
                    await loginUi.Connect();
                    await loginUi.Login(loginUi.Field.text);
                    LoginSuccess = true;
                    EnterPressed = false;
                }
            });
        }

        public override UiState<LoginUiControl> TryTransition(LoginUiControl loginUi)
        {
            if (LoginSuccess)
            {
                return new JoinModeState();
            }
            return null;
        }
    }

    class JoinModeState : LoginUiState
    {
        private bool joinPressed = false;
        private bool hostPressed = false;
        public override void Init(LoginUiControl loginUi)
        {
            base.Init(loginUi);
            loginUi.Field.gameObject.SetActive(false);
            loginUi.PrimaryButton.text = "JOIN";
            loginUi.PrimaryButton.SetCallback(() =>
            {
                if (CanTransit())
                {
                    return;
                }
                else
                {
                    joinPressed = true;
                }
            });

            loginUi.SecondaryButton.buttonType = ButtonControl.Type.Primary;
            loginUi.SecondaryButton.text = "HOST";
            loginUi.SecondaryButton.SetCallback(() =>
            {
                if (CanTransit())
                {
                    return;
                }
                else
                {
                    hostPressed = true;
                }
            });

            loginUi.DialogTitle.text = $"Welcome {loginUi.Client.State().Username}";
        }

        private bool CanTransit()
        {
            return hostPressed || joinPressed;
        }

        public override UiState<LoginUiControl> TryTransition(LoginUiControl loginUi)
        {
            if (BackPressed)
            {
                loginUi.Client.Exit();
                loginUi.Client.ClearState();
                return new LoginState();
            }
            if (joinPressed)
            {
                return new JoinRoomState();
            }
            if (hostPressed)
            {
                return new CreateRoomState();
            }
            return null;
        }
    }

    class CreateRoomState : LoginUiState
    {
        private bool createPressed = false;
        private bool createSuccess = false;
        public override void Init(LoginUiControl loginUi)
        {
            base.Init(loginUi);

            loginUi.DialogTitle.text = "CREATE A ROOM";
            loginUi.Field.label = "room name";
            loginUi.Field.placeholder = "Choose a name";
            loginUi.Field.text = "My Room";
            loginUi.SecondaryButton.gameObject.SetActive(false);
            loginUi.PrimaryButton.text = "CREATE!";
            loginUi.PrimaryButton.SetCallback(async () =>
            {
                if (!createPressed)
                {
                    createPressed = true;
                    await loginUi.CreateRoom(loginUi.Field.text);
                    createSuccess = true;
                }

            });
        }

        public override UiState<LoginUiControl> TryTransition(LoginUiControl monoBehavior)
        {
            if (BackPressed)
            {
                return new JoinModeState();
            }
            return null;
        }
    }
    class JoinRoomState : LoginUiState
    {
        private bool joinPressed = false;
        private bool joinSuccess = false;
        public override void Init(LoginUiControl loginUi)
        {
            base.Init(loginUi);

            loginUi.DialogTitle.text = "JOIN A ROOM";
            loginUi.Field.label = "room ID";
            loginUi.Field.placeholder = "room ID";
            loginUi.Field.text = "";
            loginUi.SecondaryButton.gameObject.SetActive(false);
            loginUi.PrimaryButton.text = "JOIN!";
            loginUi.PrimaryButton.SetCallback(async () =>
            {
                if (!joinPressed)
                {
                    joinPressed = true;
                    await loginUi.Join(loginUi.Field.text);
                    joinSuccess = true;
                }

            });
        }

        public override UiState<LoginUiControl> TryTransition(LoginUiControl monoBehavior)
        {
            if (BackPressed)
            {
                return new JoinModeState();
            }
            return null;
        }
    }

}
