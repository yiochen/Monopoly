using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prisel.Protobuf;
using UnityEngine;

namespace Prisel.Common {

    public class RequestManager
    {
        public static RequestManager Instance => new RequestManager();


        private int NextId = 1;
        private Dictionary<string, TaskCompletionSource<Packet>> Requests = new Dictionary<string, TaskCompletionSource<Packet>>();

        public Task<Packet> AddRequest(Packet request)
        {
            var t = new TaskCompletionSource<Packet>();
            Requests.Add(request.RequestId, t);
            return t.Task;
        }

        public void AddResponse(Packet response)
        {
            if (Requests.ContainsKey(response.RequestId))
            {
                var task = Requests[response.RequestId];
                Requests.Remove(response.RequestId);
                task.TrySetResult(response);
            }
        }

        public string NewId => "" + (NextId++);

        public bool IsWaitingFor(string requestId) => Requests.ContainsKey(requestId);
    }
}