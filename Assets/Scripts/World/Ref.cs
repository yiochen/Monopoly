using System;
using Newtonsoft.Json;

namespace Monopoly.Client
{
#nullable enable

    public interface IRef
    {
        public string Id { get; set; }
        public World? World { get; set; }
    }

    [JsonConverter(typeof(RefConverter))]
    public class Ref<T> : IRef where T : GameObject
    {
        public string Id { get; set; } = "";

        public World? World { get; set; }
        public static Ref<TObject> Of<TObject>(TObject gameObject) where TObject : GameObject
        {
            return new Ref<TObject>
            {
                Id = gameObject.Id,
                World = gameObject.World
            };
        }

        public T? Value
        {
            get
            {
                return (T?)World?.Get(Id);
            }

        }
#nullable disable

    }

    /// <summary>
    /// JsonConverter for Ref. This converter handles serializing Ref into a
    /// plain string of Id, and deserializing from string Id back to Ref object.
    /// It also handles setting the World reference in 
    /// </summary>
    public class RefConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((IRef)value).Id);
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var gameObjectType = objectType.GetGenericArguments()[0];
            var refType = typeof(Ref<>).MakeGenericType(gameObjectType);
            // TODO: it might be more performant to use Expression tree http://www.dougjenkinson.net/Article.aspx?id=article58_fastactivator
            var result = (IRef)Activator.CreateInstance(refType);

            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonToken.String)
            {
                var value = (string)reader.Value;
                result.Id = (string)reader.Value;
                if (serializer.Context.Context is DeserializationContext)
                {
                    DeserializationContext deserializationContext = serializer.Context.Context as DeserializationContext;
                    if (deserializationContext != null && deserializationContext.World != null)
                    {
                        result.World = deserializationContext.World;
                    }
                }
                return result;
            }

            return result;
        }
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType && (objectType.GetGenericTypeDefinition() == typeof(Ref<>));
        }
    }
}