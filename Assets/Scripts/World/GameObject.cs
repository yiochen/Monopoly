using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Runtime.Serialization;
namespace Monopoly.Client
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class GameObject
    {

        [JsonProperty]
        public string Id { get; set; }

        public abstract string Type { get; }

        public World World { get; set; }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            if (context.Context is DeserializationContext)
            {
                DeserializationContext deserializationContext = context.Context as DeserializationContext;
                if (deserializationContext != null && deserializationContext.World != null)
                {
                    deserializationContext.World.Add(this);
                }
            }
        }
    }

}