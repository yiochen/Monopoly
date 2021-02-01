using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prisel.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Prisel.Common;


public class LoginDebug : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {

        WebSocketClient client = WebSocketClient.Instance;
        client.ServerUrl = "ws://localhost:3000";
        await client.Connect();
        Debug.Log("Connected!!!");

        PlayerInfo playerInfo = new PlayerInfo
        {
            Id = "123",
            Name = "superman",
        };
        Prisel.Protobuf.Packet packet = new Prisel.Protobuf.Packet
        {
            Type = PacketType.Request,
            RequestId = "1",
            Action = "ACTION",
            Payload = null
        };

        Debug.Log("packet string is " + packet.ToString());
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Log()
    {
        Debug.Log("Working");
    }
}
