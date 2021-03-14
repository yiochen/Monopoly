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
        string json = Resources.Load<TextAsset>("demoMap").text;
        DefaultContractResolver contractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };
        BoardSetup boardSetup = JsonConvert.DeserializeObject<BoardSetup>(json, new JsonSerializerSettings
        {
            ContractResolver = contractResolver,
            Context = new StreamingContext(StreamingContextStates.All, new DeserializationContext())
        });

        MonopolyWorld world = boardSetup.World;
        world.Width = boardSetup.Width;
        world.Height = boardSetup.Height;
        world.AddToWorld();

        // Debug.Log(JsonConvert.SerializeObject(boardSetup, new JsonSerializerSettings
        // {
        //     Formatting = Formatting.Indented,
        //     ContractResolver = contractResolver,
        // }));
    }

    // Update is called once per frame
    void Update()
    {

    }
}

