using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Runtime.Serialization;
using Monopoly.Client;

public class SerializationTest : MonoBehaviour
{
    void Start()
    {
        string json = @"{
                ""tiles"": [
                    {
                        ""id"": ""mocked_id"",
                        ""position"": { ""row"": 0, ""col"": 0 },
                        ""next"": [""V11sapn7"", ""ssfsdf""],
                        ""prev"": [""3Ot2AEDN""],
                        ""hasProperties"": [],
                        ""isStart"": true
                    }
                ]
            }";
        World world = JsonConvert.DeserializeObject<MonopolyWorld>(json, new JsonSerializerSettings
        {
            Context = new StreamingContext(StreamingContextStates.All, new DeserializationContext())
        });

        Debug.Log(JsonConvert.SerializeObject(world));
    }

    // Update is called once per frame
    void Update()
    {

    }
}

