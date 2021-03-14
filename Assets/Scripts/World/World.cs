using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace Monopoly.Client
{
#nullable enable

    public class World
    {
        private Dictionary<string, GameObject> m_ObjectMap = new Dictionary<string, GameObject>();

        private Dictionary<Type, HashSet<string>> m_IdMap = new Dictionary<Type, HashSet<string>>();

        private string CreateId(string? specifiedId = null)
        {
            if (specifiedId != null && m_ObjectMap.ContainsKey(specifiedId))
            {

                throw new System.Exception($"Cannot create game object of id {specifiedId} because there is already one existing");

            }
            if (specifiedId != null)
            {
                return specifiedId;
            }
            return System.Guid.NewGuid().ToString();
        }
        public GameObject? Get(string id)
        {
            bool exists = this.m_ObjectMap.TryGetValue(id, out GameObject o);
            return o;
        }


        private HashSet<string> GetIdSet(GameObject o)
        {
            Type runtimeType = o.GetType();
            if (m_IdMap.TryGetValue(runtimeType, out HashSet<string> hashSet))
            {
                return hashSet;
            }
            var newHashSet = new HashSet<string>();
            m_IdMap.Add(runtimeType, newHashSet);
            return newHashSet;
        }

        private List<T> GetAllFromSet<T>(HashSet<string> idSet) where T : GameObject
        {
            List<T> result = new List<T>();
            foreach (string id in idSet)
            {

                if (m_ObjectMap.TryGetValue(id, out GameObject o))
                {
                    result.Add((T)o);

                }
            }
            return result;
        }

        public List<T> GetAll<T>() where T : GameObject
        {
            if (m_IdMap.TryGetValue(typeof(T), out HashSet<string> idSet))
            {
                return GetAllFromSet<T>(idSet);
            }
            return new List<T>();
        }

        public GameObject? Create(Type type, string? id = null)
        {
            if (!type.IsSubclassOf(typeof(GameObject)))
            {
                throw new Exception($"Cannot create object of type {type} in World. The type needs to be subtype of GameObject");
            }
            GameObject instance = (GameObject)type.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });

            instance.World = this;
            instance.Id = CreateId(id);
            m_ObjectMap.Add(instance.Id, instance);
            GetIdSet(instance).Add(instance.Id);
            return instance;
        }

        public Ref<T> MakeRef<T>(GameObject gameObject) where T : GameObject
        {
            return new Ref<T>
            {
                Id = gameObject.Id,
                World = this
            };
        }

        public void Add(GameObject gameObject)
        {
            var idSet = GetIdSet(gameObject);
            if (idSet.Contains(gameObject.Id))
            {
                throw new System.Exception($"object {gameObject.Type} with id {gameObject.Id} already exist");
            }
            idSet.Add(gameObject.Id);
            m_ObjectMap.Add(gameObject.Id, gameObject);
            gameObject.World = this;
        }
        public void Remove(string id)
        {
            if (m_ObjectMap.TryGetValue(id, out var gameObject))
            {
                Remove(gameObject);
            }
        }
        public void Remove(GameObject gameObject)
        {
            if (m_ObjectMap.Remove(gameObject.Type))
            {
                GetIdSet(gameObject).Remove(gameObject.Id);
            }
        }

        [OnSerializing]
        internal void OnSerializingMethod(StreamingContext context)
        {
            PopulateSerializedFields();
        }

        [OnSerialized]
        internal void OnSerializedMethod(StreamingContext context)
        {
            ClearSerializedFields();
        }

        [OnDeserializing]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            ClearSerializedFields();
            if (context.Context is DeserializationContext)
            {
                DeserializationContext? deserializationContext = context.Context as DeserializationContext;
                if (deserializationContext != null)
                {
                    deserializationContext.World = this;
                }
            }
            else
            {
                throw new Exception("Please set StreamingContext to DerializationContext");
            }
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            ClearSerializedFields();
        }

        /// <summary>
        /// Called during serialization. This is not used currently because
        /// World doesn't support serialization yet.
        /// </summary>
        protected virtual void PopulateSerializedFields()
        {
        }

        protected virtual void ClearSerializedFields() { }

        public virtual void AddToWorld()
        {
            // create tilemap
        }
    }
#nullable disable

    public class DeserializationContext
    {
        public World World { get; set; }
    }
}