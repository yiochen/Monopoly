using UnityEngine;
using UnityEditor;
using Prisel.Common;

// ensure class initializer is called whenever scripts recompile
[InitializeOnLoadAttribute]
public static class DisconnectWebsocketWhenExitPlayMode
{
    static DisconnectWebsocketWhenExitPlayMode()
    {
        EditorApplication.playModeStateChanged += AttemptDisconnect;
    }

    // Update is called once per frame
    private static void AttemptDisconnect(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            WebSocketClient client = WebSocketClient.Instance;
            if (client.IsConnected)
            {
                Debug.Log("exiting play mode, force closing socket");
                client.Close();
            }
        }
    }
}
