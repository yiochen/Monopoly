using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Runtime.Serialization;
using Monopoly.Client;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MapLoader : MonoBehaviour
{
    public AssetReference Map;
    private Board Board;
    public void InitializeWorld(Board board)
    {
        Board = board;
        Map.LoadAssetAsync<TextAsset>().Completed += OnLoadComplete;
    }

    void OnLoadComplete(AsyncOperationHandle<TextAsset> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("demoMap Loaded!!");
            string json = handle.Result.ToString();
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
            world.AddToWorld(Board);
            foreach (TileObject tile in world.GetAll<TileObject>())
            {
                if (tile.ChancePool.Count > 0)
                {
                    Debug.Log($"found a chance!!! at {tile.Position.ToKey()}");
                }
            }
        }
        else
        {
            Debug.Log("Loading map failed");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

